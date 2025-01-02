using LuaNET.Lua51;
using Spectre.Console;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.API;

public static class SavedVariables
{
    public static void RegisterSavedVariables(lua_State luaState, Toc toc)
    {
        try
        {
            // Define the directory path
            var directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "saved-variables");

            // Create the directory if it doesn't exist
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                AnsiConsole.WriteLine($"Created directory: {directoryPath}");
            }

            foreach (var savedVariable in toc.SavedVariables)
            {
                var savedVariablePath = Path.Combine(directoryPath, savedVariable + ".lua");

                if (File.Exists(savedVariablePath))
                {
                    //AnsiConsole.WriteLine($"Loading saved variables from '{savedVariablePath}'.");

                    // Execute the Lua file to load the saved variables
                    var status = luaL_dofile(luaState, savedVariablePath);
                    if (status != 0)
                    {
                        // Retrieve and log the error message from Lua
                        var error = lua_tostring(luaState, -1);
                        AnsiConsole.WriteLine($"Error loading '{savedVariablePath}': {error}");
                        lua_pop(luaState, 1); // Remove error message from the stack
                        continue; // Skip to the next saved variable
                    }

                    // Verify that the global variable is a table
                    lua_getglobal(luaState, savedVariable);
                    if (lua_istable(luaState, -1) != 0)
                    {
                        var savedVariableTable = LuaHelpers.GetTable(luaState, -1);
                        //AnsiConsole.WriteLine($"Successfully loaded saved variables for '{savedVariable}'.");
                    }
                    else
                    {
                        AnsiConsole.WriteLine($"Global '{savedVariable}' is not a table after loading.");
                    }

                    lua_pop(luaState, 1); // Remove the global variable from the stack
                }
                else
                {
                    AnsiConsole.WriteLine($"Saved variable file '{savedVariablePath}' does not exist. Initializing empty table.");

                    // Initialize an empty table for the saved variable
                    lua_newtable(luaState);
                    lua_setglobal(luaState, savedVariable);

                    AnsiConsole.WriteLine($"Initialized empty table for '{savedVariable}'.");
                }
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }

    public static void SaveSavedVariables(lua_State luaState, Toc toc)
    {
        try
        {
            var directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "saved-variables");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                AnsiConsole.WriteLine($"Created directory: {directoryPath}");
            }

            foreach (var savedVariable in toc.SavedVariables)
            {
                //AnsiConsole.WriteLine($"Saving variable '{savedVariable}'...");

                lua_getglobal(luaState, savedVariable);
                if (lua_istable(luaState, -1) != 0)
                {
                    var savedVariablePath = Path.Combine(directoryPath, savedVariable + ".lua");

                    var savedVariableTable = LuaHelpers.GetTable(luaState, -1);

                    using (var sw = new StreamWriter(savedVariablePath))
                    {
                        sw.WriteLine($"-- {savedVariable}.lua");
                        sw.WriteLine($"{savedVariable} = {LuaHelpers.SerializeLuaTable(savedVariableTable, 0)}");
                    }
                    //AnsiConsole.WriteLine($"Saved variable '{savedVariable}' to '{savedVariablePath}'.");
                }
                else
                {
                    AnsiConsole.WriteLine($"Global '{savedVariable}' is not a table. Skipping.");
                }

                lua_pop(luaState, 1);
            }

            // Verify stack balance
            var finalStack = lua_gettop(luaState);
            //AnsiConsole.WriteLine($"SaveSavedVariables: Final stack top: {finalStack}");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }
}