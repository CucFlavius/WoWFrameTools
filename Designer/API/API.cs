﻿using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using LuaNET.Lua51;
using Spectre.Console;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.API;

public static class API
{
    public static readonly object _eventLock = new();
    public static readonly object _luaLock = new();
    
    public static Toc? _toc;
    
    public static Toc LoadToc(string path)
    {
        _toc = new Toc(path);
        return _toc;
    }
    
    public static int Print(lua_State L)
    {
        var nargs = lua_gettop(L);
        var sb = new StringBuilder();
        
        for (var i = 1; i <= nargs; i++)
        {
            LuaTypeToString(L, -1, sb);
            sb.AppendLine();
        }

        Log.Print(sb.ToString());
        return 0; // Number of return values
    }
    
    private static void LuaTypeToString(lua_State L, int depth, StringBuilder sb)
    {
        switch (lua_type(L, -1))
        {
            case LUA_TSTRING:
                sb.Append($"\"{lua_tostring(L, -1)}\",");
                break;
            case LUA_TNUMBER:
                sb.Append($"{lua_tonumber(L, -1)},");
                break;
            case LUA_TBOOLEAN:
                sb.Append($"{(lua_toboolean(L, -1) != 0)},");
                break;
            case LUA_TTABLE:
                sb.Append(TableToString(L, lua_gettop(L), depth + 1));
                break;
            case LUA_TUSERDATA:
                sb.Append($"userdata: {lua_touserdata(L, -1)},");
                break;
            case LUA_TLIGHTUSERDATA:
                sb.Append($"lightuserdata: {lua_touserdata(L, -1)},");
                break;
            case LUA_TNIL:
                sb.Append("nil,");
                break;
            case LUA_TFUNCTION:
                sb.Append("function(),");
                break;
            default:
                AnsiConsole.MarkupLine($"[yellow]TableToString: Unsupported type '{lua_typename(L, lua_type(L, -1))}'[/]");
                break;
        }
    }
    
    private static string TableToString(lua_State L, int index, int depth = 0)
    {
        var sb = new StringBuilder();
        var indent = new string(' ', depth * 2);
        sb.AppendLine($"{indent}{{");
        lua_pushnil(L); // First key
        while (lua_next(L, index) != 0)
        {
            var key = lua_tostring(L, -2);
            sb.Append($"{indent}  {key}=");

            LuaTypeToString(L, depth, sb);

            sb.AppendLine();

            lua_pop(L, 1); // Remove value, keep key for next iteration
        }
        if (sb.Length > 2)
        {
            sb.Length -= 2; // Remove trailing comma and newline
        }
        sb.AppendLine();
        sb.AppendLine($"{indent}}}");
        return sb.ToString();
    }
    
    public static void TriggerEvent(lua_State L, string eventName, string? param = null)
    {
        lock (API._eventLock)
        {
            if (UIObjects._eventToFrames.TryGetValue(eventName, out var frames))
            {
                foreach (var frame in frames.ToList()) // Use ToList to create a copy for safe iteration
                {
                    Log.EventTrigger(eventName, param, frame);
                    frame.TriggerEvent(eventName, param);
                }
            }
            else
            {
                AnsiConsole.WriteLine($"No frames registered for event '{eventName}'.");
            }
        }
    }
    
    public static int IsLoggedIn(lua_State L)
    {
        lua_pushboolean(L, 1);

        // Return the number of return values
        return 1;
    }
}