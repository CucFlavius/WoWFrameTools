using System.Text;
using LuaNET.Lua51;
using Spectre.Console;

namespace WoWFrameTools;

using static Lua;

public static class LuaHelpers
{
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
        visited ??= [];

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
            if (!visited.Add(tableAddress))
            {
                AnsiConsole.WriteLine("GetTable: Detected cyclic reference. Skipping serialization of this table.");
                return table;
            }

        lua_pushnil(luaState); // first key
        while (lua_next(luaState, absIndex) != 0)
        {
            // key at -2 and value at -1
            var key = lua_tostring(luaState, -2);
            if (key == null)
            {
                AnsiConsole.WriteLine("GetTable: Encountered a non-string key. Skipping this key-value pair.");
                lua_pop(luaState, 1); // remove value, keep key for next iteration
                continue;
            }

            try
            {
                var value = GetLuaValue(luaState, -1, visited);
                table[key] = value;
                //AnsiConsole.WriteLine($"GetTable: Key '{key}' => Value '{value}'");
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"GetTable: Exception while processing key '{key}': {ex.Message}");
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
        if (lua_isnil(luaState, index) != 0) return null;

        if (lua_isboolean(luaState, index) != 0) return lua_toboolean(luaState, index) != 0;

        if (lua_isnumber(luaState, index) != 0) return lua_tonumber(luaState, index);

        if (lua_isstring(luaState, index) != 0) return lua_tostring(luaState, index);

        if (lua_istable(luaState, index) != 0)
            // Recursively retrieve nested tables
            return GetTable(luaState, index, visited);

        // Handle other types like functions, userdata, etc., if necessary
        AnsiConsole.WriteLine($"GetLuaValue: Unsupported Lua type at index {index}. Skipping.");
        return null;
    }
}