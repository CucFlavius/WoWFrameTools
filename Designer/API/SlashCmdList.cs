using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.API;

public static class SlashCmdList
{
    private static readonly Dictionary<string, string> _slashCommands = new();
    
    public static void Register(lua_State L)
    {
        LuaHelpers.RegisterGlobalTable(L, "SlashCmdList", new Dictionary<string, lua_CFunction>
        {
            { "AddSlashCommand", internal_AddSlashCommand }
        });
    }

    private static int internal_AddSlashCommand(lua_State L)
    {
        lock (API._luaLock)
        {
            try
            {
                // Ensure there are exactly 3 arguments: name, command, handler
                var argc = lua_gettop(L);
                if (argc != 3)
                {
                    Log.ErrorL(L, "AddSlashCommand requires exactly 3 arguments: name, command, handler.");
                    return 0; // Unreachable
                }

                // Get the name (e.g., "HELLO")
                if (!LuaHelpers.IsString(L, 1))
                {
                    Log.ErrorL(L, "AddSlashCommand: 'name' must be a string.");
                    return 0;
                }

                var name = lua_tostring(L, 1);

                // Get the command string (e.g., "/hello")
                if (!LuaHelpers.IsString(L, 2))
                {
                    Log.ErrorL(L, "AddSlashCommand: 'command' must be a string.");
                    return 0;
                }

                var command = lua_tostring(L, 2);

                // Get the handler function
                if (lua_isfunction(L, 3) == 0)
                {
                    Log.ErrorL(L, "AddSlashCommand: 'handler' must be a function.");
                    return 0;
                }

                // Create a reference to the handler function
                lua_pushvalue(L, 3);
                var refIndex = luaL_ref(L, LUA_REGISTRYINDEX);

                // Register the slash command
                // Example: SLASH_HELLO1 = "/hello"
                var slashVar = $"SLASH_{name}1";
                lua_pushstring(L, slashVar);
                lua_setglobal(L, slashVar);
                if (command != null)
                {
                    lua_pushstring(L, command);
                    lua_setglobal(L, slashVar);

                    // Set the handler in SlashCmdList
                    lua_getglobal(L, "SlashCmdList"); // Push SlashCmdList table
                    if (name != null)
                    {
                        lua_pushstring(L, name);
                        lua_rawgeti(L, LUA_REGISTRYINDEX, refIndex); // Push handler function
                        lua_settable(L, -3); // SlashCmdList[name] = handler
                        lua_pop(L, 1); // Pop SlashCmdList table

                        // Optionally, store the mapping in C# if needed
                        _slashCommands[command] = name;

                        Log.Debug($"Slash command '{command}' registered with name '{name}'.");
                    }
                }

                // Push 'true' to indicate success
                lua_pushboolean(L, 1);
                return 1;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                Log.ErrorL(L, "AddSlashCommand encountered an error.");
                return 0;
            }
        }
    }
    
    public static void ExecuteSlashCommand(lua_State L, string command)
    {
        lock (API._luaLock)
        {
            try
            {
                if (_slashCommands.TryGetValue(command, out var name))
                {
                    // Get the handler function from SlashCmdList
                    lua_getglobal(L, "SlashCmdList");
                    lua_pushstring(L, name);
                    lua_gettable(L, -2); // Push SlashCmdList[name]

                    if (lua_isfunction(L, -1) == 0)
                    {
                        Log.Error($"Handler for slash command '{command}' is not a function.");
                        lua_pop(L, 2); // Pop non-function and SlashCmdList
                        return;
                    }

                    // Call the handler function with the command as argument
                    lua_pushvalue(L, -1); // Push handler
                    lua_pushnil(L); // self (could be Frame or nil)
                    lua_pushstring(L, command); // msg
                    if (lua_pcall(L, 2, 0, 0) != 0)
                    {
                        var error = lua_tostring(L, -1);
                        Log.Error($"Lua Error in SlashCmdList['{name}']: {error}");
                        lua_pop(L, 1);
                    }

                    lua_pop(L, 1); // Pop SlashCmdList
                }
                else
                {
                    Log.Warn($"No handler registered for slash command '{command}'.");
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}