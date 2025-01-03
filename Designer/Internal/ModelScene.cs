using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Internal;

public static class ModelScene
{
    public static int internal_CreateActor(lua_State L)
    {
        var frame = GetThis(L, 1);

        var argc = lua_gettop(L);

        string? name = null;
        string? template = null;

        if (argc > 1) name = lua_tostring(L, 2);
        if (argc > 2) template = lua_tostring(L, 3);

        var actor = frame?.CreateActor(name, template);

        // Allocate a GCHandle to prevent the Frame from being garbage collected
        var handle = GCHandle.Alloc(actor);
        var handlePtr = GCHandle.ToIntPtr(handle);

        // Create userdata with the size of IntPtr
        var userdataPtr = (IntPtr)lua_newuserdata(L, (UIntPtr)IntPtr.Size);

        // Write the handlePtr into the userdata memory
        Marshal.WriteIntPtr(userdataPtr, handlePtr);

        // Set the metatable for the userdata
        luaL_getmetatable(L, "ModelSceneActorMetaTable");
        lua_setmetatable(L, -2);

        // Add the Frame to the registry for later retrieval
        API.UIObjects._actorRegistry[userdataPtr] = actor;

        // Assign the userdataPtr and LuaRegistryRef to the Frame instance
        actor.UserdataPtr = userdataPtr;

        // Create a reference to the userdata in the registry
        lua_pushvalue(L, -1); // Push the userdata
        var refIndex = luaL_ref(L, LUA_REGISTRYINDEX);
        actor.LuaRegistryRef = refIndex;

        Log.CreateActor(actor);
        
        return 1;
    }
    
    public static int internal_SetCameraOrientationByYawPitchRoll(lua_State L)
    {
        var frame = GetThis(L, 1);

        var argc = lua_gettop(L);
        if (argc != 5)
        {
            Log.ErrorL(L, "SetCameraOrientationByYawPitchRoll requires 3 arguments.");
            return 0;
        }
        
        var yaw = (float)lua_tonumber(L, 2);
        var pitch = (float)lua_tonumber(L, 3);
        var roll = (float)lua_tonumber(L, 4);
        
        frame?.SetCameraOrientationByYawPitchRoll(yaw, pitch, roll);
        
        return 0;
    }
    
    public static int internal_SetLightAmbientColor(lua_State L)
    {
        var frame = GetThis(L, 1);

        var argc = lua_gettop(L);
        if (argc != 5)
        {
            Log.ErrorL(L, "SetLightAmbientColor requires 3 arguments.");
            return 0;
        }
        
        var colorR = (float)lua_tonumber(L, 2);
        var colorG = (float)lua_tonumber(L, 3);
        var colorB = (float)lua_tonumber(L, 4);
        
        frame?.SetLightAmbientColor(colorR, colorG, colorB);
        
        return 0;
    }
    
    public static int internal_SetLightDiffuseColor(lua_State L)
    {
        var frame = GetThis(L, 1);

        var argc = lua_gettop(L);
        if (argc != 5)
        {
            Log.ErrorL(L, "SetLightDiffuseColor requires 3 arguments.");
            return 0;
        }
        
        var colorR = (float)lua_tonumber(L, 2);
        var colorG = (float)lua_tonumber(L, 3);
        var colorB = (float)lua_tonumber(L, 4);
        
        frame?.SetLightDiffuseColor(colorR, colorG, colorB);
        
        return 0;
    }
    
    public static int internal_SetLightVisible(lua_State L)
    {
        var frame = GetThis(L, 1);

        var argc = lua_gettop(L);
        if (argc != 3)
        {
            Log.ErrorL(L, "SetLightVisible requires 1 argument.");
            return 0;
        }
        
        var visible = lua_toboolean(L, 2);
        
        frame?.SetLightVisible(visible != 0);
        
        return 0;
    }
    
    public static int internal_ClearFog(lua_State L)
    {
        var frame = GetThis(L, 1);

        frame?.ClearFog();
        
        return 0;
    }
    
    public static int internal_SetCameraPosition(lua_State L)
    {
        var frame = GetThis(L, 1);

        var argc = lua_gettop(L);
        if (argc != 5)
        {
            Log.ErrorL(L, "SetCameraPosition requires 3 arguments.");
            return 0;
        }
        
        var positionX = (float)lua_tonumber(L, 2);
        var positionY = (float)lua_tonumber(L, 3);
        var positionZ = (float)lua_tonumber(L, 4);
        
        frame?.SetCameraPosition(positionX, positionY, positionZ);
        
        return 0;
    }
    
    public static int internal_SetFogColor(lua_State L)
    {
        var frame = GetThis(L, 1);

        var argc = lua_gettop(L);
        if (argc < 4)
        {
            Log.ErrorL(L, "SetFogColor requires 3 arguments.");
            return 0;
        }
        
        var colorR = (float)lua_tonumber(L, 2);
        var colorG = (float)lua_tonumber(L, 3);
        var colorB = (float)lua_tonumber(L, 4);
        
        frame?.SetFogColor(colorR, colorG, colorB);
        
        return 0;
    }
    
    public static int internal_SetFogFar(lua_State L)
    {
        var frame = GetThis(L, 1);

        var argc = lua_gettop(L);
        if (argc != 3)
        {
            Log.ErrorL(L, "SetFogFar requires 1 argument.");
            return 0;
        }
        
        var far = (float)lua_tonumber(L, 2);
        
        frame?.SetFogFar(far);
        
        return 0;
    }
    
    public static int internal_SetFogNear(lua_State L)
    {
        var frame = GetThis(L, 1);

        var argc = lua_gettop(L);
        if (argc != 3)
        {
            Log.ErrorL(L, "SetFogNear requires 1 argument.");
            return 0;
        }
        
        var near = (float)lua_tonumber(L, 2);
        
        frame?.SetFogNear(near);
        
        return 0;
    }
    
    public static string GetMetatableName() => "ModelSceneMetaTable";

    public static Widgets.ModelScene? GetThis(lua_State L, int index)
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

        return handle.Target as Widgets.ModelScene;
    }
    
    public static void RegisterMetaTable(lua_State L)
    {
        // 1) call base to register the "ModelSceneMetaTable"
        Frame.RegisterMetaTable(L);

        // 2) Now define "ModelSceneMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = ModelSceneMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("FrameMetaTable")
        var baseMetaName = Frame.GetMetatableName();
        luaL_getmetatable(L, baseMetaName);
        lua_setmetatable(L, -2); // Sets ModelSceneMetaTable's metatable to FrameMetaTable
        
        // 5) Bind Frame-specific methods
        LuaHelpers.RegisterMethod(L, "SetCameraOrientationByYawPitchRoll", internal_SetCameraOrientationByYawPitchRoll);
        LuaHelpers.RegisterMethod(L, "SetLightAmbientColor", internal_SetLightAmbientColor);
        LuaHelpers.RegisterMethod(L, "SetLightDiffuseColor", internal_SetLightDiffuseColor);
        LuaHelpers.RegisterMethod(L, "SetLightVisible", internal_SetLightVisible);
        LuaHelpers.RegisterMethod(L, "ClearFog", internal_ClearFog);
        LuaHelpers.RegisterMethod(L, "SetCameraPosition", internal_SetCameraPosition);
        LuaHelpers.RegisterMethod(L, "SetFogColor", internal_SetFogColor);
        LuaHelpers.RegisterMethod(L, "SetFogFar", internal_SetFogFar);
        LuaHelpers.RegisterMethod(L, "SetFogNear", internal_SetFogNear);
        LuaHelpers.RegisterMethod(L, "CreateActor", internal_CreateActor);
        
        // 6) pop
        lua_pop(L, 1);
    }
}