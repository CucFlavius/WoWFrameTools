using System.Xml;
using LuaNET.Lua51;
using Spectre.Console;
using WoWFrameTools.API;
using WoWFrameTools.Widgets;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools;

internal class Program
{
    private static lua_State L { get; set; }
    
    private static async Task Main(string[] args)
    {
        const string addonPath = @"D:\Games\World of Warcraft\_retail_\Interface\AddOns\scenemachine";
        var tocPath = Path.Combine(addonPath, "scenemachine.toc");
        var toc = API.API.LoadToc(tocPath);

        var luaFiles = new List<string>();
        foreach (var codePath in toc.CodePaths)
        {
            var absolutePath = Path.GetFullPath(Path.Combine(addonPath, codePath));
            await ProcessFileRecursive(absolutePath, addonPath, luaFiles);
        }

        L = luaL_newstate();
        luaL_openlibs(L);
        ProcessLuaFile(L, "Compat.lua", "");

        // new Button().RegisterMetaTable(L);
        // new EditBox().RegisterMetaTable(L);
        // new FontString().RegisterMetaTable(L);
        // new Frame().RegisterMetaTable(L);
        // new GameTooltip().RegisterMetaTable(L);
        // new Line().RegisterMetaTable(L);
        // new Minimap().RegisterMetaTable(L);
        // new Texture().RegisterMetaTable(L);
        // new Model().RegisterMetaTable(L);
        // new ModelScene().RegisterMetaTable(L);
        // new PlayerModel().RegisterMetaTable(L);
        // new DressUpModel().RegisterMetaTable(L);
        // new ModelSceneActor().RegisterMetaTable(L);
        
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
        
        LuaHelpers.RegisterGlobalMethod(L, "CreateFrame", UIObjects.CreateFrame);
        LuaHelpers.RegisterGlobalMethod(L, "GetTime", Game.GetTime);
        LuaHelpers.RegisterGlobalMethod(L, "GetLocale", Game.GetLocale);
        LuaHelpers.RegisterGlobalMethod(L, "SendChatMessage", Game.SendChatMessage);
        LuaHelpers.RegisterGlobalMethod(L, "SendAddonMessage", Game.SendAddonMessage);
        LuaHelpers.RegisterGlobalMethod(L, "print", API.API.Print);
        LuaHelpers.RegisterGlobalMethod(L, "IsLoggedIn", API.API.IsLoggedIn);
        C_Addons.Register(L);
        C_ChatInfo.Register(L);
        C_CVar.Register(L);
        SlashCmdList.Register(L);

        //SlashCmdList.ExecuteSlashCommand(L, "/dump GetTime()");
        
        ////////////////////
        // Run some tests //
        // var success = luaL_dostring(L, "print(CreateFrame(\"Frame\", \"FrameA\"))");
        // if (success != 0)
        // {
        //     AnsiConsole.WriteLine($"{lua_tostring(L, -1)}");
        //     lua_pop(L, 1);
        // }
        //
        // return;
        // var success = luaL_dofile(L, "test.lua");
        // if (success != 0)
        // {
        //     AnsiConsole.WriteLine($"{lua_tostring(L, -1)}");
        //     lua_pop(L, 1);
        // }
        //
        // return;
        ////////////////////
        
        UIObjects.UIParent = new Widgets.Frame("Frame", "UIParent", null, null, 0);
        //UIObjects.UIParent.RegisterMetaTable(L);
        UIObjects.CreateGlobalFrame(L, UIObjects.UIParent);
        
        UIObjects.Minimap = new Minimap("Minimap", UIObjects.UIParent, null, 0);
        //UIObjects.Minimap.RegisterMetaTable(L);
        UIObjects.CreateGlobalFrame(L, UIObjects.Minimap);
        
        SavedVariables.RegisterSavedVariables(L, toc);
        ProcessLuaFile(L, "hooksecurefunc.lua", "");
        
        foreach (var luaFile in luaFiles)
        {
            // get path relative to addonPath
            var relativePath = Path.GetRelativePath(addonPath, luaFile);

            if (ProcessLuaFile(L, luaFile, relativePath)) break;
        }
        
        // Start the events
        // https://warcraft.wiki.gg/wiki/AddOn_loading_process
        //API.API.TriggerEvent(L, "ADDON_LOADED", "scenemachine"); // → addOnName
        //API.API.TriggerEvent(L, "PLAYER_LOGIN");
        //API.API.TriggerEvent(L, "PLAYER_ENTERING_WORLD"); // → isInitialLogin, isReloadingUi
        try
        {
            var result = luaL_dostring(L, "SceneMachine.Start()");
            if (result != 0) // Lua error
            {
                var errorMessage = lua_tostring(L, -1);
                AnsiConsole.WriteLine($"Lua Error: {errorMessage}");
                lua_pop(L, 1); // Remove the error from the stack
            }
        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e);
        }

        // Save the saved variables
        SavedVariables.SaveSavedVariables(L, toc);

        // Close Lua state
        lua_close(L);

        AnsiConsole.WriteLine("Lua state closed. Application exiting.");
    }
    
    private static bool ProcessLuaFile(lua_State L, string luaFile, string relativePath)
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

    private static async Task ProcessFileRecursive(string filePath, string addonPath, List<string> luaFiles, bool log = false)
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