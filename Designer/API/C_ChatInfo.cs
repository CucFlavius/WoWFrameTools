using System.Runtime.InteropServices;
using LuaNET.Lua51;
using Spectre.Console;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.API;

public static class C_ChatInfo
{
    private static readonly object _prefixLock = new();
    private static readonly HashSet<string> _registeredPrefixes = new();

    public static void Register(lua_State L)
    {
        LuaHelpers.RegisterGlobalTable(L, "C_ChatInfo", new Dictionary<string, lua_CFunction>
        {
            { "RegisterAddonMessagePrefix", internal_RegisterAddonMessagePrefix }
        });
    }

    private static int internal_RegisterAddonMessagePrefix(lua_State L)
    {
        // Check if the first argument is a string
        if (!LuaHelpers.IsString(L, 1))
        {
            Log.ErrorL(L, "RegisterAddonMessagePrefix requires a string argument.");
            return 0; // Unreachable
        }

        var prefix = lua_tostring(L, 1);

        if (string.IsNullOrEmpty(prefix))
        {
            Log.ErrorL(L, "RegisterAddonMessagePrefix: prefix cannot be empty.");
            return 0; // Unreachable
        }

        bool added = _registeredPrefixes.Add(prefix);

        /*
        if (added)
            AnsiConsole.MarkupLine($"[green]Registered Addon Message Prefix: '{prefix}'[/]");
        else
            AnsiConsole.MarkupLine($"[yellow]Addon Message Prefix '{prefix}' is already registered.[/]");
        */
        return 0; // No return values
    }

}