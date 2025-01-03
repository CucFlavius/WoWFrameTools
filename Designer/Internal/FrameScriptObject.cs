using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Internal;

public static class FrameScriptObject
{
    public static int GetName(lua_State L)
    {
        var obj = GetThis(L, 1);
        var name = obj?.GetName();
        if (name == null)
        {
            lua_pushnil(L);
            return 1;
        }
        lua_pushstring(L, name);
        return 1;
    }
    
    public static int GetObjectType(lua_State L)
    {
        var obj = GetThis(L, 1);
        lua_pushstring(L, obj?.GetObjectType() ?? "");
        return 1;
    }
    
    public static int IsForbidden(lua_State L)
    {
        var obj = GetThis(L, 1);
        lua_pushboolean(L, (obj != null && obj.IsForbidden()) ? 1 : 0);
        return 1;
    }
    
    public static int IsObjectType(lua_State L)
    {
        var obj = GetThis(L, 1);
        var str = lua_tostring(L, 2) ?? "";
        var result = (obj != null && obj.IsObjectType(str));
        lua_pushboolean(L, result ? 1 : 0);
        return 1;
    }
    
    public static int SetForbidden(lua_State L)
    {
        var obj = GetThis(L, 1);
        if (obj != null)
        {
            var isF = (lua_toboolean(L, 2) != 0);
            obj.SetForbidden(isF);
        }
        return 0;
    }

    public static string GetMetatableName() => "FrameScriptObjectMetaTable";

    private static Widgets.FrameScriptObject? GetThis(lua_State L, int index)
    {
        // 1) Check the correct metatable
        // var metaName = GetMetatableName();
        // luaL_getmetatable(L, metaName);
        // lua_getmetatable(L, index);
        // var same = (lua_rawequal(L, -1, -2) != 0);
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

        var userdataPtr = (IntPtr)lua_touserdata(L, index);
        if (userdataPtr == IntPtr.Zero)
            return null;

        var handlePtr = Marshal.ReadIntPtr(userdataPtr);
        if (handlePtr == IntPtr.Zero)
            return null;

        var handle = GCHandle.FromIntPtr(handlePtr);
        if (!handle.IsAllocated)
            return null;

        return handle.Target as Widgets.FrameScriptObject;
    }
    
    public static void RegisterMetaTable(lua_State L)
    {
        // 1) Create a new metatable with the name from GetMetatableName()
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 2) __index = FrameScriptObjectMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 3) Bind FrameScriptObject-specific methods
        LuaHelpers.RegisterMethod(L, "GetName", Internal.FrameScriptObject.GetName);
        LuaHelpers.RegisterMethod(L, "GetObjectType", Internal.FrameScriptObject.GetObjectType);
        LuaHelpers.RegisterMethod(L, "IsForbidden", Internal.FrameScriptObject.IsForbidden);
        LuaHelpers.RegisterMethod(L, "IsObjectType", Internal.FrameScriptObject.IsObjectType);
        LuaHelpers.RegisterMethod(L, "SetForbidden", Internal.FrameScriptObject.SetForbidden);

        // 4) pop
        lua_pop(L, 1);
    }
}