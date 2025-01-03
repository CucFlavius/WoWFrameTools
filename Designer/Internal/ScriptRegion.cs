using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Internal;

public static class ScriptRegion
{
    public static int internal_EnableMouse(lua_State L)
    {
        var frame = GetThis(L, 1);

        var argc = lua_gettop(L);
        if (argc < 2)
        {
            Log.ErrorL(L, "EnableMouse requires exactly 1 argument: enable.");
            return 0; // Unreachable
        }

        var enable = lua_toboolean(L, 2) != 0;
        frame?.EnableMouse(enable);

        return 0;
    }
    
    public static int internal_GetHeight(lua_State L)
    {
        var frame = GetThis(L, 1);
        var height = frame?.GetHeight() ?? 0;

        lua_pushnumber(L, height);
        return 1;
    }
    
    public static int internal_GetWidth(lua_State L)
    {
        var frame = GetThis(L, 1);
        var height = frame?.GetWidth() ?? 0;

        lua_pushnumber(L, height);
        return 1;
    }
    
    public static int internal_Hide(lua_State L)
    {
        var frame = GetThis(L, 1);
        frame?.Hide();
        return 0;
    }

    public static int internal_IsVisible(lua_State L)
    {
        var frame = GetThis(L, 1);
        var isVisible = frame?.IsVisible() ?? false;

        lua_pushboolean(L, isVisible ? 1 : 0);
        return 1;
    }
    
    public static int internal_SetParent(lua_State L)
    {
        var frame = GetThis(L, 1);
        var parent = GetThis(L, 2);
        frame?.SetParent(parent);
        return 0;
    }
    
    public static int internal_Show(lua_State L)
    {
        var frame = GetThis(L, 1);
        frame?.Show();
        return 0;
    }
    
    public static int internal_ClearAllPoints(lua_State L)
    {
        var frame = GetThis(L, 1);

        frame?.ClearAllPoints();

        return 0;
    }
    
    public static int internal_SetAllPoints(lua_State L)
    {
        var frame = GetThis(L, 1);
        if (frame == null)
        {
            lua_pushboolean(L, 0);
            return 1;
        }

        // Retrieve arguments: relativeTo (optional), doResize (optional)
        Widgets.Frame? relativeTo = null;
        var doResize = false;

        if (lua_gettop(L) >= 2 && lua_isnil(L, 2) != 0)
        {
            relativeTo = GetThis(L, 2) as Widgets.Frame;
        }

        if (lua_gettop(L) >= 3) doResize = lua_toboolean(L, 3) != 0;

        // Set all points relative to another frame
        frame.SetAllPoints(relativeTo, doResize);

        lua_pushboolean(L, 1);
        return 1;
    }
    
    public static int internal_SetHeight(lua_State L)
    {
        var frame = GetThis(L, 1);

        var argc = lua_gettop(L);
        if (argc < 2)
        {
            Log.ErrorL(L, "SetHeight requires exactly 1 argument: height.");
            return 0; // Unreachable
        }

        var height = (float)lua_tonumber(L, 2);

        frame?.SetHeight(height);

        lua_pushboolean(L, 1);
        return 1;
    }
    
    public static int internal_SetPoint(lua_State L)
    {
        var region = GetThis(L, 1);

        var argc = lua_gettop(L);
        if (argc < 2)
        {
            Log.ErrorL(L, "SetPoint requires at least 1 argument: point.");
            return 0; // Unreachable
        }

        var point = lua_tostring(L, 2);

        Widgets.Frame? relativeTo = null;
        string? relativePoint = null;
        float offsetX = 0;
        float offsetY = 0;
            
        if (argc >= 3) relativeTo = LuaHelpers.GetFrame(L, 3);
            
        if (argc >= 4) relativePoint = lua_tostring(L, 4);

        if (argc >= 6)
        {
            offsetX = (float)lua_tonumber(L, 5);
            offsetY = (float)lua_tonumber(L, 6);
        }
            
        region?.SetPoint(point!, relativeTo, relativePoint, offsetX, offsetY);

        lua_pushboolean(L, 1);
        return 1;
    }
    
    public static int internal_SetSize(lua_State L)
    {
        // Ensure there are exactly 3 arguments: frame, width, height
        var argc = lua_gettop(L);
        if (argc != 3)
        {
            Log.ErrorL(L, "SetSize requires exactly 3 arguments: frame, width, height.");
            return 0; // Unreachable
        }

        // Retrieve the Frame object
        var frame = GetThis(L, 1);
        if (frame == null)
        {
            Log.ErrorL(L, "SetSize: Invalid Frame object.");
            return 0; // Unreachable
        }

        // Retrieve width and height
        if (!LuaHelpers.IsNumber(L, 2) || !LuaHelpers.IsNumber(L, 3))
        {
            Log.ErrorL(L, "SetSize: 'width' and 'height' must be numbers.");
            return 0; // Unreachable
        }

        var width = (float)lua_tonumber(L, 2);
        var height = (float)lua_tonumber(L, 3);

        // Set the size
        frame.SetSize(width, height);

        // Log the action
        //AnsiConsole.WriteLine($"SetSize called on Frame. Width: {width}, Height: {height}");

        // No return values
        return 0;
    }
    
    public static int internal_SetWidth(lua_State L)
    {
        var frame = GetThis(L, 1);

        var argc = lua_gettop(L);
        if (argc < 2)
        {
            Log.ErrorL(L, "SetWidth requires exactly 1 argument: width.");
            return 0; // Unreachable
        }

        var width = (float)lua_tonumber(L, 2);

        frame?.SetWidth(width);

        lua_pushboolean(L, 1);
        return 1;
    }
    
    public static string GetMetatableName() => "ScriptRegionMetaTable";

    public static Widgets.ScriptRegion? GetThis(lua_State L, int index)
    {
        // 1) Check the correct metatable
        // var metaName = GetMetatableName();
        // luaL_getmetatable(L, metaName);
        // lua_getmetatable(L, index);
        // bool same = (lua_rawequal(L, -1, -2) != 0);
        // lua_pop(L, 2);
        //
        // if (!same)
        //     return null;

        // If it's a table, retrieve the __frame key
        if (lua_istable(L, index) != 0)
        {
            lua_pushstring(L, "__frame");
            lua_gettable(L, index); // Pushes table["__frame"]
            index = -1; // Update index to point to __frame value
        }

        IntPtr userdataPtr = (IntPtr)lua_touserdata(L, index);
        if (userdataPtr == IntPtr.Zero)
            return null;

        IntPtr handlePtr = Marshal.ReadIntPtr(userdataPtr);
        if (handlePtr == IntPtr.Zero)
            return null;

        var handle = GCHandle.FromIntPtr(handlePtr);
        if (!handle.IsAllocated)
            return null;

        return handle.Target as Widgets.ScriptRegion;
    }

    public static void RegisterMetaTable(lua_State L)
    {
        ScriptObject.RegisterMetaTable(L);
        
        // 2) Define "ScriptRegionMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = ScriptRegionMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("UIObjectMetaTable")
        var baseMetaName = ScriptObject.GetMetatableName();
        luaL_getmetatable(L, baseMetaName);
        lua_setmetatable(L, -2); // Sets ScriptRegionMetaTable's metatable to UIObjectMetaTable

        // 5) Bind ScriptRegion-specific methods
        // ScriptRegion
        LuaHelpers.RegisterMethod(L, "Hide", Internal.ScriptRegion.internal_Hide);
        LuaHelpers.RegisterMethod(L, "Show", Internal.ScriptRegion.internal_Show);
        LuaHelpers.RegisterMethod(L, "EnableMouse", Internal.ScriptRegion.internal_EnableMouse);
        LuaHelpers.RegisterMethod(L, "SetParent", Internal.ScriptRegion.internal_SetParent);
        LuaHelpers.RegisterMethod(L, "GetHeight", Internal.ScriptRegion.internal_GetHeight);
        LuaHelpers.RegisterMethod(L, "GetWidth", Internal.ScriptRegion.internal_GetWidth);
        LuaHelpers.RegisterMethod(L, "IsVisible", Internal.ScriptRegion.internal_IsVisible);

        // IScriptRegionResizing
        LuaHelpers.RegisterMethod(L, "SetPoint", Internal.ScriptRegion.internal_SetPoint);
        LuaHelpers.RegisterMethod(L, "SetSize", Internal.ScriptRegion.internal_SetSize);
        LuaHelpers.RegisterMethod(L, "SetAllPoints", Internal.ScriptRegion.internal_SetAllPoints);
        LuaHelpers.RegisterMethod(L, "SetHeight", Internal.ScriptRegion.internal_SetHeight);
        LuaHelpers.RegisterMethod(L, "SetWidth", Internal.ScriptRegion.internal_SetWidth);
        LuaHelpers.RegisterMethod(L, "ClearAllPoints", Internal.ScriptRegion.internal_ClearAllPoints);

        // 6) pop
        lua_pop(L, 1);
    }
}

