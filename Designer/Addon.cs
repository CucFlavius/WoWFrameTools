using System.Xml;
using LuaNET.Lua51;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Spectre.Console;
using WoWFrameTools.API;
using WoWFrameTools.Widgets;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools;

public class Addon
{
    private Toc? _toc;
    private readonly string _addonPath;
    private readonly string _tocFilePath;
    private volatile bool _isLoaded = false;
    private float _elapsedTime = 0.0f;
    
    public Addon(string path)
    {
        _addonPath = path;
        var tocFileName = Path.GetFileName(path) + ".toc";
        _tocFilePath = Path.Combine(path, tocFileName);
    }

    public async Task Load(lua_State L)
    {
        _toc = new Toc(_tocFilePath);
        API.API._toc = _toc;        // Assign the toc global

        var luaFiles = new List<string>();
        foreach (var codePath in _toc.CodePaths)
        {
            var absolutePath = Path.GetFullPath(Path.Combine(_addonPath, codePath));
            await ProcessFileRecursive(absolutePath, _addonPath, luaFiles);
        }

        var thread = new Thread(() =>
        {
            _isLoaded = false;
            
            ProcessLuaFile(L, "Compat.lua", "");
            RegisterMetatables(L);
            RegisterGlobalMethods(L);
            RegisterGlobalClasses(L);

            UIObjects.UIParent = new Widgets.Frame("Frame", "UIParent", null, null, 0);
            UIObjects.CreateGlobalFrame(L, UIObjects.UIParent);

            UIObjects.Minimap = new Minimap("Minimap", UIObjects.UIParent, null, 0);
            UIObjects.CreateGlobalFrame(L, UIObjects.Minimap);

            SavedVariables.RegisterSavedVariables(L, _toc);
            ProcessLuaFile(L, "hooksecurefunc.lua", "");

            foreach (var luaFile in luaFiles)
            {
                // get path relative to addonPath
                var relativePath = Path.GetRelativePath(_addonPath, luaFile);

                if (ProcessLuaFile(L, luaFile, relativePath)) break;
            }

            // Start the events
            // https://warcraft.wiki.gg/wiki/AddOn_loading_process
            API.API.TriggerEvent(L, "ADDON_LOADED", "scenemachine"); // → addOnName
            API.API.TriggerEvent(L, "PLAYER_LOGIN");
            API.API.TriggerEvent(L, "PLAYER_ENTERING_WORLD"); // → isInitialLogin, isReloadingUi
            
            _isLoaded = true;
        });
        
        thread.Start();
    }
    
    private void RegisterMetatables(lua_State L)
    {
        Internal.Button.RegisterMetaTable(L);
        Internal.EditBox.RegisterMetaTable(L);
        Internal.FontString.RegisterMetaTable(L);
        Internal.Frame.RegisterMetaTable(L);
        Internal.GameTooltip.RegisterMetaTable(L);
        Internal.Line.RegisterMetaTable(L);
        Internal.Minimap.RegisterMetaTable(L);
        Internal.Texture.RegisterMetaTable(L);
        Internal.Model.RegisterMetaTable(L);
        Internal.ModelScene.RegisterMetaTable(L);
        Internal.PlayerModel.RegisterMetaTable(L);
        Internal.DressUpModel.RegisterMetaTable(L);
        Internal.ModelSceneActor.RegisterMetaTable(L);
    }
    
    private void RegisterGlobalMethods(lua_State L)
    {
        LuaHelpers.RegisterGlobalMethod(L, "CreateFrame", UIObjects.CreateFrame);
        LuaHelpers.RegisterGlobalMethod(L, "GetTime", Game.GetTime);
        LuaHelpers.RegisterGlobalMethod(L, "GetLocale", Game.GetLocale);
        LuaHelpers.RegisterGlobalMethod(L, "SendChatMessage", Game.SendChatMessage);
        LuaHelpers.RegisterGlobalMethod(L, "SendAddonMessage", Game.SendAddonMessage);
        LuaHelpers.RegisterGlobalMethod(L, "UnitName", Game.UnitName);
        LuaHelpers.RegisterGlobalMethod(L, "print", API.API.Print);
        LuaHelpers.RegisterGlobalMethod(L, "IsLoggedIn", API.API.IsLoggedIn);
        LuaHelpers.RegisterGlobalMethod(L, "strsplit", API.API.strsplit);
        LuaHelpers.RegisterGlobalMethod(L, "GetFramerate", API.API.GetFramerate);
    }
    
    private void RegisterGlobalClasses(lua_State L)
    {
        C_Addons.Register(L);
        C_ChatInfo.Register(L);
        C_CVar.Register(L);
        SlashCmdList.Register(L);
    }

    private bool ProcessLuaFile(lua_State L, string luaFile, string relativePath)
    {
        try
        {
            // 1) Load the file
            int loadStatus = luaL_loadfile(L, luaFile);
            if (loadStatus != 0)
            {
                // There's a syntax error or file not found, etc.
                string loadErr = lua_tostring(L, -1);
                AnsiConsole.MarkupLine("[red]Error loading file:[/] " + relativePath);
                AnsiConsole.MarkupLine($"[red]{loadErr}[/]");
                lua_pop(L, 1); // pop the error message
                return true;    // or handle error
            }

            // 2) Now do a protected call
            int pcallStatus = lua_pcall(L, 0, LUA_MULTRET, 0);
            if (pcallStatus != 0)
            {
                // Now we can read the actual error message from the top of the stack
                string errorMsg = lua_tostring(L, -1);
                AnsiConsole.MarkupLine("[red]-------------------------------------------[/]");
                AnsiConsole.MarkupLine($"[red]Error in file:[/] {relativePath}");
                AnsiConsole.MarkupLine($"[red]{errorMsg}[/]");
                AnsiConsole.MarkupLine("[red]-------------------------------------------[/]");
                lua_pop(L, 1); // pop the error message
                return true;    // or handle error
            }

            // success
            Log.ProcessFile(relativePath);
            return false; // no error

        }
        catch (Exception e)
        {
            Log.Error("Error processing file: " + relativePath);
            AnsiConsole.WriteException(e);
            return false;
        }
    }

    private async Task ProcessFileRecursive(string filePath, string addonPath, List<string> luaFiles, bool log = false)
    {
        var relativePath = Path.GetRelativePath(addonPath, filePath);

        if (!File.Exists(filePath))
        {
            AnsiConsole.MarkupLine($"[red]File not found:[/] {filePath}");
            return;
        }

        var ext = Path.GetExtension(filePath).ToLower();
        if (ext == ".lua")
        {
            // If it's a Lua file, add it to the list
            luaFiles.Add(filePath);
            if (log)
                AnsiConsole.MarkupLine($"[yellow]{relativePath}[/]");
        }
        else if (ext == ".xml")
        {
            if (log)
                AnsiConsole.MarkupLine($"[blue]{relativePath} -> [/]");

            var xml = new XmlDocument();
            xml.Load(filePath);

            var relativeToPath = Path.GetDirectoryName(filePath) ?? string.Empty;

            // Check if the XML has a default namespace
            var root = xml.DocumentElement;
            XmlNodeList? nodes;

            if (root != null && !string.IsNullOrEmpty(root.NamespaceURI))
            {
                // Handle XML with namespaces
                var nsmgr = new XmlNamespaceManager(xml.NameTable);
                nsmgr.AddNamespace("ns", root.NamespaceURI);

                nodes = xml.SelectNodes("//ns:Script | //ns:Include", nsmgr);
            }
            else
            {
                // Handle XML without namespaces
                nodes = xml.SelectNodes("//Script | //Include");
            }

            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    var file = node.Attributes?["file"]?.Value;
                    if (!string.IsNullOrEmpty(file))
                    {
                        // Normalize the file path to remove ../
                        var absolutePath = Path.GetFullPath(Path.Combine(relativeToPath, file));

                        // Ensure the path is within the addon directory for safety
                        if (!absolutePath.StartsWith(addonPath))
                        {
                            AnsiConsole.MarkupLine($"[red]Skipping invalid path:[/] {absolutePath}");
                            continue;
                        }

                        // Recursively process the file
                        await ProcessFileRecursive(absolutePath, addonPath, luaFiles, log);
                    }
                }
            }
        }
    }

    public void SaveVariables(lua_State L)
    {
        if (_toc != null)
            SavedVariables.SaveSavedVariables(L, _toc);
    }

    public void Update(float deltaTimeF)
    {
        if (!_isLoaded)
            return;
        
        API.API._frameRate = 1.0f / deltaTimeF;
        _elapsedTime += deltaTimeF;
        
        foreach (var (ptr, frame) in API.UIObjects._frameRegistry)
        {
            // number - The time in seconds since the last OnUpdate dispatch,
            // but excluding time when the user interface was not being drawn such as while zoning into the game world
            frame.OnUpdate(_elapsedTime);
        }
    }
}