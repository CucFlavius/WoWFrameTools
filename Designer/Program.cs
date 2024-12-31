using System.Xml;
using LuaNET.Lua51;
using Spectre.Console;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var addonPath = "D:\\Games\\World of Warcraft\\_retail_\\Interface\\AddOns\\scenemachine";
        var tocPath = Path.Combine(addonPath, "scenemachine.toc");
        var toc = new Toc(tocPath);
        //AnsiConsole.MarkupLine($"[purple]{tocPath} -> [/]");

        var luaFiles = new List<string>();

        foreach (var codePath in toc.CodePaths)
        {
            var absolutePath = Path.GetFullPath(Path.Combine(addonPath, codePath));
            await ProcessFileRecursive(absolutePath, addonPath, luaFiles);
        }

        var L = luaL_newstate();
        luaL_openlibs(L);
        ProcessLuaFile(L, "Compat.lua", "");
        Frame.RegisterMetaTable(L);
        FontString.RegisterMetatable(L);
        Texture.RegisterMetaTable(L);
        Line.RegisterMetatable(L);
        Global.SetToc(toc);
        Global.RegisterBindings(L);
        Global.RegisterSavedVariables(L, toc);
        ProcessLuaFile(L, "hooksecurefunc.lua", "");
        
        foreach (var luaFile in luaFiles)
        {
            // get path relative to addonPath
            var relativePath = Path.GetRelativePath(addonPath, luaFile);

            if (ProcessLuaFile(L, luaFile, relativePath)) break;
        }
        
        // Start the events
        // https://warcraft.wiki.gg/wiki/AddOn_loading_process
        Global.TriggerEvent(L, "ADDON_LOADED", "scenemachine"); // → addOnName
        Global.TriggerEvent(L, "PLAYER_LOGIN");
        Global.TriggerEvent(L, "PLAYER_ENTERING_WORLD"); // → isInitialLogin, isReloadingUi
        
        // Save the saved variables
        Global.SaveSavedVariables(L, toc);

        // Close Lua state
        lua_close(L);

        AnsiConsole.WriteLine("Lua state closed. Application exiting.");
    }

    private static bool ProcessLuaFile(lua_State L, string luaFile, string relativePath)
    {
        try
        {
            var status = luaL_dofile(L, luaFile);
            if (status != 0)
            {
                AnsiConsole.MarkupLine("[red]-------------------------------------------[/]");
                AnsiConsole.MarkupLine($"[red]Error in file:[/] {relativePath}");
                AnsiConsole.MarkupLine($"[red]{lua_tostring(L, -1)}[/]");
                AnsiConsole.MarkupLine("[red]-------------------------------------------[/]");
                lua_pop(L, 1);
                return true;
            }

            Log.ProcessFile(relativePath);
        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e);
            throw;
        }

        return false;
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