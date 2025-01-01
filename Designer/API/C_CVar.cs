using LuaNET.Lua51;
using Spectre.Console;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.API;

public static class C_CVar
{
    public static void Register(lua_State L)
    {
        LuaHelpers.RegisterGlobalTable(L, "C_CVar", new Dictionary<string, lua_CFunction>
        {
            { "GetCVar", internal_GetCVar }
        });
        
        LuaHelpers.RegisterGlobalMethod(L, "GetCVar", internal_GetCVar);
    }

    private static int internal_GetCVar(lua_State L)
    {
        // Ensure there is exactly 1 argument: cvarName
        var argc = lua_gettop(L);
        if (argc != 1)
        {
            Log.ErrorL(L, "GetCVar requires exactly 1 argument: cvarName.");
            return 0; // Unreachable
        }

        // Retrieve the cvarName from Lua
        var cvarName = lua_tostring(L, 1);

        // Retrieve the value of the specified CVar
        string cvarValue;
        switch (cvarName)
        {
            case "gxWindowedResolution":
                cvarValue = "1024x768";
                // TODO: Return actual resolution
                break;
            default:
                cvarValue = string.Empty;
                AnsiConsole.MarkupLine($"[red]GetCVar: Unsupported CVar '{cvarName}'[/]");
                break;
        }

        // Push the CVar value as a string onto the Lua stack
        lua_pushstring(L, cvarValue);

        // Return the number of return values
        return 1;
    }
}