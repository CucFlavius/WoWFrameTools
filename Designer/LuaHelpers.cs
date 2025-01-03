using System.Runtime.InteropServices;
using System.Text;
using LuaNET.Lua51;
using Spectre.Console;
using WoWFrameTools.Widgets;

namespace WoWFrameTools;

using static Lua;

public static class LuaHelpers
{
    /// <summary>
    ///     Serializes a Lua table represented as a Dictionary
    ///     to Lua table syntax.
    /// </summary>
    /// <param name="table">The Lua table as a dictionary.</param>
    /// <param name="indentLevel">The current indentation level.</param>
    /// <returns>A string representing the Lua table.</returns>
    public static string SerializeLuaTable(Dictionary<string, object> table, int indentLevel)
    {
        var indent = new string(' ', indentLevel * 4);
        var sb = new StringBuilder();
        sb.AppendLine("{");

        var count = table.Count;
        var current = 0;

        foreach (var kvp in table)
        {
            current++;
            sb.Append(indent + "    ");
            sb.Append($"[{SerializeLuaKey(kvp.Key)}] = {SerializeLuaValue(kvp.Value, indentLevel + 1)}");

            // Add a comma if it's not the last element
            if (current < count) sb.Append(",");

            sb.AppendLine();
        }

        sb.Append(indent + "}");
        return sb.ToString();
    }

    /// <summary>
    ///     Serializes a Lua key to Lua syntax.
    /// </summary>
    /// <param name="key">The key to serialize.</param>
    /// <returns>A string representing the Lua key.</returns>
    private static string SerializeLuaKey(string key)
    {
        return $"\"{EscapeLuaString(key)}\"";
    }

    private static string EscapeLuaString(string str)
    {
        return str
            .Replace("\\", "\\\\") // Escape backslashes
            .Replace("\"", "\\\"") // Escape double quotes
            .Replace("\n", "\\n") // Escape newlines
            .Replace("\r", "\\r") // Escape carriage returns
            .Replace("\t", "\\t"); // Escape tabs
    }

    /// <summary>
    ///     Serializes a Lua value to Lua syntax.
    /// </summary>
    /// <param name="value">The value to serialize.</param>
    /// <param name="indentLevel">The current indentation level.</param>
    /// <returns>A string representing the Lua value.</returns>
    private static string? SerializeLuaValue(object? value, int indentLevel)
    {
        switch (value)
        {
            case null:
                return "nil";
            case bool boolVal:
                return boolVal ? "true" : "false";
            case string strVal:
                return $"\"{EscapeLuaString(strVal)}\"";
            case double:
            case float:
            case int:
            case long:
                return value.ToString();
            case Dictionary<string, object> nestedTable:
                return SerializeLuaTable(nestedTable, indentLevel);
            default:
                // Handle other types as needed or throw an exception
                throw new NotSupportedException($"Unsupported value type: {value.GetType()}");
        }
    }

    /// <summary>
    ///     Serializes a Lua list (array) to Lua table syntax.
    /// </summary>
    /// <param name="list">The list to serialize.</param>
    /// <param name="indentLevel">The current indentation level.</param>
    /// <returns>A string representing the Lua list.</returns>
    private static string SerializeLuaList(IEnumerable<object> list, int indentLevel)
    {
        var indent = new string(' ', indentLevel * 4);
        var sb = new StringBuilder();
        sb.AppendLine("{");
        var listAsList = new List<object>(list);
        var count = listAsList.Count;
        var current = 0;
        foreach (var item in listAsList)
        {
            current++;
            sb.Append(indent + "    ");
            sb.Append($"{SerializeLuaValue(item, indentLevel + 1)}");

            // Add a comma if it's not the last element
            if (current < count) sb.Append(",");

            sb.AppendLine();
        }

        sb.Append(indent + "}");
        return sb.ToString();
    }

    /// <summary>
    ///     Retrieves a Lua table from the stack and converts it to a C# Dictionary.
    /// </summary>
    /// <param name="luaState">The Lua state.</param>
    /// <param name="index">The stack index of the table.</param>
    /// <param name="visited"></param>
    /// <returns>A Dictionary representing the Lua table.</returns>
    public static Dictionary<string, object> GetTable(lua_State luaState, int index, HashSet<IntPtr>? visited = null)
    {
        var table = new Dictionary<string, object>();
        visited ??= new HashSet<IntPtr>();

        // Compute absolute index for Lua 5.1
        var absIndex = index >= 0 ? index : lua_gettop(luaState) + index + 1;

        if (lua_istable(luaState, absIndex) == 0)
        {
            AnsiConsole.WriteLine($"GetTable: Value at index {index} (absolute index {absIndex}) is not a table.");
            return table;
        }

        // Get the address of the table to detect cycles
        var tableAddress = (IntPtr)lua_topointer(luaState, absIndex);
        if (tableAddress != IntPtr.Zero)
        {
            if (!visited.Add(tableAddress))
            {
                //AnsiConsole.WriteLine("GetTable: Detected cyclic reference. Skipping serialization of this table.");
                return table;
            }
        }

        lua_pushnil(luaState); // first key
        while (lua_next(luaState, absIndex) != 0)
        {
            // key at -2 and value at -1
            string? key = null;

            int keyType = lua_type(luaState, -2);
            if (keyType == LUA_TSTRING)
            {
                key = lua_tostring(luaState, -2);
            }
            else if (keyType == LUA_TNUMBER)
            {
                // Convert number keys to string to fit the Dictionary<string, object> structure
                key = lua_tonumber(luaState, -2).ToString();
            }
            else
            {
                AnsiConsole.WriteLine("GetTable: Encountered an unsupported key type. Skipping this key-value pair.");
                lua_pop(luaState, 1); // remove value, keep key for next iteration
                continue;
            }

            try
            {
                var value = GetLuaValue(luaState, -1, visited);
                table[key] = value;
                // AnsiConsole.WriteLine($"GetTable: Key '{key}' => Value '{value}'");
            }
            catch (Exception ex)
            {
                Log.Error($"GetTable: Exception while processing key '{key}': {ex.Message}");
            }

            lua_pop(luaState, 1); // remove value, keep key for next iteration
        }


        return table;
    }

    /// <summary>
    ///     Retrieves a Lua value from the stack and converts it to a C# object.
    /// </summary>
    /// <param name="luaState">The Lua state.</param>
    /// <param name="index">The stack index of the value.</param>
    /// <returns>A C# object representing the Lua value.</returns>
    private static object GetLuaValue(lua_State luaState, int index, HashSet<IntPtr> visited)
    {
        if (lua_isnil(luaState, index) != 0)
        {
            return null;
        }

        if (lua_isboolean(luaState, index) != 0)
        {
            return lua_toboolean(luaState, index) != 0;
        }

        if (lua_isnumber(luaState, index) != 0)
        {
            return lua_tonumber(luaState, index);
        }

        if (lua_isstring(luaState, index) != 0)
        {
            return lua_tostring(luaState, index);
        }

        if (lua_istable(luaState, index) != 0)
        {
            // Recursively retrieve nested tables
            return GetTable(luaState, index, visited);
        }

        if (lua_isuserdata(luaState, index) != 0)
        {
            // Handle userdata
            //return null;
            return GetUserdata(luaState, index);
        }

        // Handle other types like functions, threads, etc., if necessary
        int type = lua_type(luaState, index); // Retrieve the type code
        string typeName = lua_typename(luaState, type); // Get the type name as a string

        //AnsiConsole.MarkupLine($"[red]GetLuaValue:[/] Unsupported Lua type '{typeName}' at index {index}. Skipping.");
        return null;
    }

    private static object? GetUserdata(lua_State luaState, int index)
    {
        // Step 1: Retrieve the userdata pointer from Lua
        IntPtr userdataPtr = (IntPtr)lua_touserdata(luaState, index);

        if (userdataPtr == IntPtr.Zero)
        {
            AnsiConsole.MarkupLine($"[red]GetUserdata:[/] Userdata at index {index} is a light userdata with a null pointer.");
            return null;
        }

        // Step 2: Look up the C# object in the registry
        if (API.UIObjects._frameRegistry.TryGetValue(userdataPtr, out var frame))
        {
            return frame;
        }
        else if (API.UIObjects._textureRegistry.TryGetValue(userdataPtr, out var texture))
        {
            return texture;
        }
        else if (API.UIObjects._fontStringRegistry.TryGetValue(userdataPtr, out var fontString))
        {
            return fontString;
        }
        else if (API.UIObjects._actorRegistry.TryGetValue(userdataPtr, out var actor))
        {
            return actor;
        }
        /*
        else if (API.UIObjects._nameToFrameRegistry.TryGetValue(userdataPtr, out var namedFrame))
        {
            return namedFrame;
        }
        */

        // Step 3: Handle unknown userdata types
        // Optionally, retrieve the metatable to determine the userdata type
        lua_getmetatable(luaState, index);
        if (lua_isnil(luaState, -1) == 0)
        {
            try
            {
                // Assuming the metatable name is stored as a string in the registry
                // Alternatively, you might have to retrieve specific fields to determine the type
                lua_pushstring(luaState, "__name"); // Push the key to get the metatable name
                lua_gettable(luaState, -2); // Get metatable.__name
                string metaName = "unknown";
                if (lua_isstring(luaState, -1) != 0)
                    lua_tostring(luaState, -1);
                lua_pop(luaState, 2); // Pop the value and the metatable
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        else
        {
            lua_pop(luaState, 1); // Pop the nil
            AnsiConsole.MarkupLine($"[yellow]GetUserdata:[/] Unknown userdata with no metatable at index {index}.");
        }

        return null;
    }

    /// <summary>
    /// Pushes a given FrameScriptObject-derived instance onto the Lua stack 
    /// as a userdata, setting the appropriate metatable from obj.GetMetatableName().
    /// 
    /// If obj is null, pushes nil.
    /// </summary>
    public static void PushFrameScriptObject(lua_State L, FrameScriptObject? obj)
    {
        // 1) If the object is null, just push nil
        if (obj == null)
        {
            lua_pushnil(L);
            return;
        }

        // 2) Allocate a GCHandle to pin 'obj'
        GCHandle handle = GCHandle.Alloc(obj);
        IntPtr handlePtr = GCHandle.ToIntPtr(handle);

        // 3) Create a new userdata block large enough for IntPtr
        // In Lua 5.1, we can do: 
        IntPtr userdataPtr = (IntPtr)lua_newuserdata(L, (UIntPtr)IntPtr.Size);

        // 4) Write the handle pointer into that memory
        Marshal.WriteIntPtr(userdataPtr, handlePtr);

        // 5) Retrieve the class-specific metatable name, e.g. "UIObjectMetaTable" or "ScriptRegionMetaTable"
        string metaName = Internal.FrameScriptObject.GetMetatableName();

        // 6) Get that metatable
        luaL_getmetatable(L, metaName);

        // 7) Set it as the userdata's metatable
        // stack: [ ..., userData, metaTable ]
        lua_setmetatable(L, -2);
        // stack: [ ..., userData ]

        // Done! userData with the correct metatable is on top of the stack
        // The caller can now do e.g. 
        // local scriptObj = ...  -- from C# code
        // scriptObj:GetName() in Lua, etc.
    }

    /// <summary>
    ///     Converts the result of a Lua C API 'is' function to a C# bool.
    /// </summary>
    /// <param name="result">The integer result from the Lua function.</param>
    /// <returns>True if result is non-zero, otherwise false.</returns>
    private static bool ToBool(int result)
    {
        return result != 0;
    }

    /// <summary>
    ///     Checks if the value at the given index is a number.
    /// </summary>
    /// <param name="L">The Lua state.</param>
    /// <param name="index">The stack index.</param>
    /// <returns>True if the value is a number, otherwise false.</returns>
    public static bool IsNumber(lua_State L, int index)
    {
        return ToBool(lua_isnumber(L, index));
    }

    /// <summary>
    ///     Checks if the value at the given index is a string.
    /// </summary>
    /// <param name="L">The Lua state.</param>
    /// <param name="index">The stack index.</param>
    /// <returns>True if the value is a string, otherwise false.</returns>
    public static bool IsString(lua_State L, int index)
    {
        return ToBool(lua_isstring(L, index));
    }

    public static void RegisterMethod(lua_State L, string methodName, lua_CFunction function)
    {
        lua_pushstring(L, methodName);
        lua_pushcfunction(L, function);
        lua_settable(L, -3); // metatable.__index[methodName] = function
    }

    public static void RegisterGlobalMethod(lua_State L, string methodName, lua_CFunction function)
    {
        lua_pushcfunction(L, function);
        lua_setglobal(L, methodName);
    }

    public static void RegisterGlobalTable(lua_State L, string tableName, Dictionary<string, lua_CFunction>? methods = null)
    {
        lua_newtable(L); // Create a new table
        if (methods != null)
        {
            foreach (var (fName, function) in methods)
            {
                lua_pushcfunction(L, function);
                lua_setfield(L, -2, fName); // Set table[fName] = function
            }
        }

        lua_setglobal(L, tableName); // Set the table as a global
    }

    public static Frame? GetFrame(lua_State L, int index)
    {
        Frame? relativeTo;
        if (lua_isuserdata(L, index) != 0)
        {
            // Handle userdata parent
            var parentUserdataPtr = (IntPtr)lua_touserdata(L, index);
            if (parentUserdataPtr != IntPtr.Zero)
            {
                if (!API.UIObjects._frameRegistry.TryGetValue(parentUserdataPtr, out var foundFrame))
                {
                    return null;
                    //throw new ArgumentException("Invalid frame specified.");
                }

                relativeTo = foundFrame;
            }
            else
            {
                throw new ArgumentException("Frame is not userdata.");
            }
        }
        else if (lua_isstring(L, index) != 0)
        {
            // Handle string parent (name of the frame)
            var parentName = lua_tostring(L, index) ?? "";
            if (string.IsNullOrEmpty(parentName))
            {
                throw new ArgumentException("Frame name is empty.");
            }

            if (!API.UIObjects._nameToFrameRegistry.TryGetValue(parentName, out var namedFrame))
            {
                return null;
                //throw new ArgumentException($"No frame found with the name '{parentName}'.");
            }

            relativeTo = namedFrame;
        }
        else if (lua_isnil(L, index) != 0)
        {
            // Parent is nil; default to UIParent if applicable
            relativeTo = API.UIObjects.UIParent;
        }
        else if (lua_istable(L, index) != 0)
        {
            // Handle table parent
            var table = GetTable(L, index);
            if (table.TryGetValue("__frame", out var frameObj))
            {
                relativeTo = frameObj as Frame;
            }
            else
            {
                throw new ArgumentException("Invalid frame specified.");
            }
        }
        else
        {
            throw new ArgumentException("Frame must be userdata, a string, or nil.");
        }

        return relativeTo;
    }
    
    public static void PushExistingFrameToLua(lua_State L, Frame child)
    {
        // If you're storing the table in child.LuaRegistryRef, do:
        lua_rawgeti(L, LUA_REGISTRYINDEX, child.LuaRegistryRef);
        // Now the child's table/userdata is on top of the stack.
        // If child has no registry ref, you might need to create one or log an error.
    }

    public static void PrintLuaArguments(lua_State L, int argc)
    {
        for (var i = 1; i <= argc; i++)
        {
            int type = lua_type(L, i);
            string typeName = GetLuaTypeName(type);
            Console.WriteLine($"Argument {i}: Type = {typeName}");
        }
    }
    
    private static string GetLuaTypeName(int luaType)
    {
        switch (luaType)
        {
            case LUA_TNONE:
                return "none";
            case LUA_TNIL:
                return "nil";
            case LUA_TBOOLEAN:
                return "boolean";
            case LUA_TLIGHTUSERDATA:
                return "lightuserdata";
            case LUA_TNUMBER:
                return "number";
            case LUA_TSTRING:
                return "string";
            case LUA_TTABLE:
                return "table";
            case LUA_TFUNCTION:
                return "function";
            case LUA_TUSERDATA:
                return "userdata";
            case LUA_TTHREAD:
                return "thread";
            default:
                return "unknown";
        }
    }
}