using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Internal;

public static class Something
{
    private static string GetMetatableName() => "SomethingMetaTable";

    private static Widgets.ScriptObject? GetThis(lua_State L, int index)
    {
        // 1) Check the correct metatable
        var metaName = GetMetatableName();
        luaL_getmetatable(L, metaName);
        lua_getmetatable(L, index);
        bool same = (lua_rawequal(L, -1, -2) != 0);
        lua_pop(L, 2);

        if (!same)
            return null;

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

        return handle.Target as Widgets.ScriptObject;
    }
}