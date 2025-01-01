using System.Runtime.InteropServices;
using LuaNET.Lua51;
using Spectre.Console;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.API;

public static class C_Addons
{
    public static void Register(lua_State L)
    {
        LuaHelpers.RegisterGlobalTable(L, "C_AddOns", new Dictionary<string, lua_CFunction>
        {
            { "GetAddOnMetadata", internal_GetAddOnMetadata }
        });
    }

    private static int internal_GetAddOnMetadata(lua_State L)
    {
        // Ensure there are exactly 2 arguments: addonName, metadataKey
        var argc = lua_gettop(L);
        if (argc < 2)
        {
            Log.ErrorL(L, "GetAddOnMetadata requires exactly 2 arguments: addonName, metadataKey.");
            return 0; // Unreachable
        }

        // Retrieve addonName and metadataKey from Lua
        if (!LuaHelpers.IsString(L, 1) || !LuaHelpers.IsString(L, 2))
        {
            Log.ErrorL(L, "GetAddOnMetadata: Both 'addonName' and 'metadataKey' must be strings.");
            return 0; // Unreachable
        }

        var addonName = lua_tostring(L, 1);
        var metadataKey = lua_tostring(L, 2);

        // Verify that a TOC is loaded
        if (API._toc == null)
        {
            Log.ErrorL(L, "GetAddOnMetadata: No TOC loaded.");
            return 0; // Unreachable
        }

        // Retrieve the metadata value from Toc
        // Assuming Toc has a method GetMetadata(string key)
        switch (metadataKey)
        {
            case "Title":
                lua_pushstring(L, API._toc.Title);
                break;
            case "Version":
                lua_pushstring(L, API._toc.Version);
                break;
            case "Author":
                lua_pushstring(L, API._toc.Author);
                break;
            default:
                AnsiConsole.MarkupLine($"[yellow]GetAddOnMetadata: Metadata key '{metadataKey}' not found.[/]");
                lua_pushnil(L); // Push nil if metadataKey not found
                break;
        }

        return 1; // Number of return values
    }
}