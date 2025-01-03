using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Internal;

public static class ModelSceneActor
{
    public static int internal_SetModelByFileID(lua_State L)
    {
        var argc = lua_gettop(L);

        var actor = GetThis(L, 1);
        
        var useMips = false;
        var fileID = (int)lua_tonumber(L, 2);
        if (argc > 2) useMips = lua_toboolean(L, 3) != 0;

        var success = actor.SetModelByFileID(fileID, useMips);
        lua_pushboolean(L, success ? 1 : 0);
        return 1;
    }
    
    public static int internal_SetUseCenterForOrigin(lua_State L)
    {
        var argc = lua_gettop(L);

        var actor = GetThis(L, 1);
        
        var x = false;
        var y = false;
        var z = false;
        if (argc > 1) x = lua_toboolean(L, 2) != 0;
        if (argc > 2) y = lua_toboolean(L, 3) != 0;
        if (argc > 3) z = lua_toboolean(L, 4) != 0;

        actor?.SetUseCenterForOrigin(x, y, z);
        return 0;
    }
    
    public static int internal_SetYaw(lua_State L)
    {
        var argc = lua_gettop(L);

        var actor = GetThis(L, 1);
        
        var yaw = (float)lua_tonumber(L, 2);

        actor?.SetYaw(yaw);
        return 0;
    }
    
    public static string GetMetatableName() => "ModelSceneActorMetaTable";

    public static Widgets.ModelSceneActor? GetThis(lua_State L, int index)
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

        return handle.Target as Widgets.ModelSceneActor;
    }
    
    public static void RegisterMetaTable(lua_State L)
    {
        // 1) call base to register the "ModelSceneActorMetaTable"
        DressUpModel.RegisterMetaTable(L);

        // 2) Now define "ModelSceneActorMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = ModelSceneActorMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("DressUpModel")
        var baseMetaName = DressUpModel.GetMetatableName();
        luaL_getmetatable(L, baseMetaName);
        lua_setmetatable(L, -2); // Sets ModelSceneActorMetaTable's metatable to DressUpModel
        
        // 5) Bind Frame-specific methods
        LuaHelpers.RegisterMethod(L, "SetModelByFileID", internal_SetModelByFileID);
        LuaHelpers.RegisterMethod(L, "SetUseCenterForOrigin", internal_SetUseCenterForOrigin);
        LuaHelpers.RegisterMethod(L, "SetYaw", internal_SetYaw);

        // 6) pop
        lua_pop(L, 1);
    }
}