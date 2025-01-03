using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Internal;

public static class Model
{
    public static int internal_SetDesaturation(lua_State L)
    {
        var argc = lua_gettop(L);

        var actor = GetThis(L, 1);
        
        var strength = (float)lua_tonumber(L, 2);

        actor?.SetDesaturation(strength);
        return 0;
    }
    
    public static int internal_SetPitch(lua_State L)
    {
        var argc = lua_gettop(L);

        var actor = GetThis(L, 1);
        
        var pitch = (float)lua_tonumber(L, 2);

        actor?.SetPitch(pitch);
        return 0;
    }
    
    public static int internal_SetPosition(lua_State L)
    {
        var argc = lua_gettop(L);

        var actor = GetThis(L, 1);
        
        var positionX = (float)lua_tonumber(L, 2);
        var positionY = (float)lua_tonumber(L, 3);
        var positionZ = (float)lua_tonumber(L, 4);

        actor?.SetPosition(positionX, positionY, positionZ);
        return 0;
    }
    
    public static int internal_SetRoll(lua_State L)
    {
        var argc = lua_gettop(L);

        var actor = GetThis(L, 1);
        
        var roll = (float)lua_tonumber(L, 2);

        actor?.SetRoll(roll);
        return 0;
    }
    
    public static string GetMetatableName() => "ModelMetaTable";

    public static Widgets.Model? GetThis(lua_State L, int index)
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

        return handle.Target as Widgets.Model;
    }
    
    public static void RegisterMetaTable(lua_State L)
    {
        // 1) call base to register the "ModelMetaTable"
        Frame.RegisterMetaTable(L);

        // 2) Now define "ModelMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = ModelMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("FrameMetaTable")
        var baseMetaName = Frame.GetMetatableName();
        luaL_getmetatable(L, baseMetaName);
        lua_setmetatable(L, -2); // Sets ModelMetaTable's metatable to FrameMetaTable
        
        // 5) Bind Frame-specific methods
        LuaHelpers.RegisterMethod(L, "SetPosition", internal_SetPosition);
        LuaHelpers.RegisterMethod(L, "SetRoll", internal_SetRoll);
        LuaHelpers.RegisterMethod(L, "SetPitch", internal_SetPitch);
        LuaHelpers.RegisterMethod(L, "SetDesaturation", internal_SetDesaturation);
        
        // 6) pop
        lua_pop(L, 1);
    }
}