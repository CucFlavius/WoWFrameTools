using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Internal;

public static class Region
{
    public static int internal_GetEffectiveScale(lua_State L)
    {
        var frame = GetThis(L, 1);
        var scale = frame?.GetEffectiveScale() ?? 1;

        lua_pushnumber(L, scale);
        return 1;
    }
    
    public static int internal_GetScale(lua_State L)
    {
        var frame = GetThis(L, 1);
        var scale = frame?.GetScale() ?? 1;

        lua_pushnumber(L, scale);
        return 1;
    }
    
    public static int internal_GetVertexColor(lua_State L)
    {
        var region = GetThis(L, 1);

        var color = region?.GetVertexColor();

        lua_pushnumber(L, color?[0] ?? 1);
        lua_pushnumber(L, color?[1] ?? 1);
        lua_pushnumber(L, color?[2] ?? 1);
        lua_pushnumber(L, color?[3] ?? 1);

        return 4;
    }
    
    public static int internal_SetAlpha(lua_State L)
    {
        var frame = GetThis(L, 1);
        var alpha = (float)lua_tonumber(L, 2);

        frame?.SetAlpha(alpha);

        return 0;
    }
    
    public static int internal_SetScale(lua_State L)
    {
        var frame = GetThis(L, 1);
        var scale = (float)lua_tonumber(L, 2);

        frame?.SetScale(scale);

        return 0;
    }
    
    public static int internal_SetVertexColor(lua_State L)
    {
        var region = GetThis(L, 1);

        var argc = lua_gettop(L);
        if (argc < 3)
        {
            Log.ErrorL(L, "SetVertexColor requires at least 3 arguments: colorR, colorG, colorB.");
            return 0; // Unreachable
        }

        var colorR = (float)lua_tonumber(L, 2);
        var colorG = (float)lua_tonumber(L, 3);
        var colorB = (float)lua_tonumber(L, 4);
        float? colorA = null;

        if (argc >= 5)
        {
            colorA = (float)lua_tonumber(L, 5);
        }

        region?.SetVertexColor(colorR, colorG, colorB, colorA);

        lua_pushboolean(L, 1);
        return 1;
    }
    
    public static string GetMetatableName() => "RegionMetaTable";

    public static Widgets.Region? GetThis(lua_State L, int index)
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

        return handle.Target as Widgets.Region;
    }
    
    public static void RegisterMetaTable(lua_State L)
    {
        // 1) Register the base class's metatable first
        ScriptRegion.RegisterMetaTable(L);

        // 2) Define "RegionMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = RegionMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("ScriptRegionMetaTable")
        var baseMetaName = ScriptRegion.GetMetatableName();
        luaL_getmetatable(L, baseMetaName);
        lua_setmetatable(L, -2); // Sets RegionMetaTable's metatable to ScriptRegionMetaTable

        // 5) Bind Region-specific methods
        LuaHelpers.RegisterMethod(L, "GetVertexColor", Internal.Region.internal_GetVertexColor);
        LuaHelpers.RegisterMethod(L, "SetVertexColor", Internal.Region.internal_SetVertexColor);
        LuaHelpers.RegisterMethod(L, "SetScale", Internal.Region.internal_SetScale);
        LuaHelpers.RegisterMethod(L, "GetScale", Internal.Region.internal_GetScale);
        LuaHelpers.RegisterMethod(L, "GetEffectiveScale", Internal.Region.internal_GetEffectiveScale);
        LuaHelpers.RegisterMethod(L, "SetAlpha", Internal.Region.internal_SetAlpha);

        // 6) pop
        lua_pop(L, 1);
    }
}