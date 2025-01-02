using System.Runtime.InteropServices;
using LuaNET.Lua51;
using WoWFrameTools.API;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets;

public class ModelScene : Frame
{
    private readonly List<ModelSceneActor> _actors;

    public ModelScene(string? name = null, Frame? parent = null, string? templateName = null, int id = 0)
        : base("ModelScene", name, parent, templateName, id)
    {
        _actors = [];
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_ModelScene_CreateActor
    /// ModelScene:CreateActor([name, template]) : actor
    /// </summary>
    /// <param name="name"></param>
    /// <param name="template"></param>
    private ModelSceneActor CreateActor(string? name = null, string? template = null)
    {
        var actor = new ModelSceneActor(name, template);
        _actors.Add(actor);
        return actor;
    }
    private int internal_CreateActor(lua_State L)
    {
        var frame = GetThis(L, 1) as ModelScene;

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
        UIObjects._actorRegistry[userdataPtr] = actor;

        // Assign the userdataPtr and LuaRegistryRef to the Frame instance
        actor.UserdataPtr = userdataPtr;

        // Create a reference to the userdata in the registry
        lua_pushvalue(L, -1); // Push the userdata
        var refIndex = luaL_ref(L, LUA_REGISTRYINDEX);
        actor.LuaRegistryRef = refIndex;

        Log.CreateActor(actor);
        
        return 1;
    }
    
    // ModelScene:GetActorAtIndex(index) : actor
    // ModelScene:GetCameraFarClip() : farClip
    // ModelScene:GetCameraFieldOfView() : fov
    // ModelScene:GetCameraForward() : forwardX, forwardY, forwardZ
    // ModelScene:GetCameraNearClip() : nearClip
    // ModelScene:GetCameraRight() : rightX, rightY, rightZ
    // ModelScene:GetCameraUp() : upX, upY, upZ
    // ModelScene:GetDrawLayer() : layer, sublevel
    // ModelScene:GetLightAmbientColor() : colorR, colorG, colorB
    // ModelScene:GetLightDiffuseColor() : colorR, colorG, colorB
    // ModelScene:GetLightDirection() : directionX, directionY, directionZ
    // ModelScene:GetLightPosition() : positionX, positionY, positionZ
    // ModelScene:GetLightType() : lightType
    // ModelScene:GetNumActors() : numActors
    // ModelScene:IsLightVisible() : isVisible
    // ModelScene:Project3DPointTo2D(pointX, pointY, pointZ) : point2DX, point2DY, depth - Converts a 3 dimensional point into clip space using the model scene camera properties.
    // ModelScene:SetCameraFarClip(farClip)
    // ModelScene:SetCameraFieldOfView(fov)
    // ModelScene:SetCameraNearClip(nearClip)
    // ModelScene:SetCameraOrientationByAxisVectors(forwardX, forwardY, forwardZ, rightX, rightY, rightZ, upX, upY, upZ)
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_ModelScene_SetCameraOrientationByYawPitchRoll
    /// ModelScene:SetCameraOrientationByYawPitchRoll(yaw, pitch, roll)
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    private void SetCameraOrientationByYawPitchRoll(float yaw, float pitch, float roll)
    {
        
    }
    private int internal_SetCameraOrientationByYawPitchRoll(lua_State L)
    {
        var frame = GetThis(L, 1) as ModelScene;

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
    
    // ModelScene:SetDrawLayer(layer)
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_ModelScene_SetLightAmbientColor
    /// ModelScene:SetLightAmbientColor(colorR, colorG, colorB)
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    private void SetLightAmbientColor(float colorR, float colorG, float colorB)
    {
        
    }
    private int internal_SetLightAmbientColor(lua_State L)
    {
        var frame = GetThis(L, 1) as ModelScene;

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
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_ModelScene_SetLightDiffuseColor
    /// ModelScene:SetLightDiffuseColor(colorR, colorG, colorB)
    /// </summary>
    /// <param name="colorR"></param>
    /// <param name="colorG"></param>
    /// <param name="colorB"></param>
    private void SetLightDiffuseColor(float colorR, float colorG, float colorB)
    {
        
    }
    private int internal_SetLightDiffuseColor(lua_State L)
    {
        var frame = GetThis(L, 1) as ModelScene;

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
    
    // ModelScene:SetLightDirection(directionX, directionY, directionZ)
    // ModelScene:SetLightPosition(positionX, positionY, positionZ)
    // ModelScene:SetLightType(lightType)
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_ModelScene_SetLightVisible
    /// ModelScene:SetLightVisible([visible])
    /// </summary>
    /// <param name="visible"></param>
    private void SetLightVisible(bool visible)
    {
        
    }
    private int internal_SetLightVisible(lua_State L)
    {
        var frame = GetThis(L, 1) as ModelScene;

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
    
    // ModelScene:SetPaused(paused [, affectsGlobalPause])
    // ModelScene:TakeActor()
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Model_ClearFog
    /// Model ModelScene:ClearFog()
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    private void ClearFog()
    {
        
    }
    private int internal_ClearFog(lua_State L)
    {
        var frame = GetThis(L, 1) as ModelScene;

        frame?.ClearFog();
        
        return 0;
    }
    
    // Model ModelScene:GetCameraPosition() : positionX, positionY, positionZ
    // Model ModelScene:GetFogColor() : colorR, colorG, colorB
    // Model ModelScene:GetFogFar() : far
    // Model ModelScene:GetFogNear() : near
    // Model ModelScene:GetViewInsets() : left, right, top, bottom
    // Model ModelScene:GetViewTranslation() : translationX, translationY
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Model_SetCameraPosition
    /// Model:SetCameraPosition(positionX, positionY, positionZ)
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    private void SetCameraPosition(float positionX, float positionY, float positionZ)
    {
        
    }
    private int internal_SetCameraPosition(lua_State L)
    {
        var frame = GetThis(L, 1) as ModelScene;

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
    
    // Model ModelScene:SetDesaturation(strength)
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_ModelScene_SetFogColor
    /// Model ModelScene:SetFogColor(colorR, colorG, colorB)
    /// </summary>
    /// <param name="colorR"></param>
    /// <param name="colorG"></param>
    /// <param name="colorB"></param>
    private void SetFogColor(float colorR, float colorG, float colorB)
    {
        
    }
    private int internal_SetFogColor(lua_State L)
    {
        var frame = GetThis(L, 1) as ModelScene;

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
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Model_SetFogFar
    /// Model ModelScene:SetFogFar(far)
    /// </summary>
    /// <param name="far"></param>
    private void SetFogFar(float far)
    {
        
    }
    private int internal_SetFogFar(lua_State L)
    {
        var frame = GetThis(L, 1) as ModelScene;

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
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Model_SetFogNear
    /// Model ModelScene:SetFogNear(near)
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    private void SetFogNear(float near)
    {

    }
    private int internal_SetFogNear(lua_State L)
    {
        var frame = GetThis(L, 1) as ModelScene;

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
    
    // Model ModelScene:SetViewInsets(left, right, top, bottom)
    // Model ModelScene:SetViewTranslation(translationX, translationY)
    
    // ----------- Virtual Registration ---------------
    
    public override string GetMetatableName() => "ModelSceneMetaTable";
        
    public override void RegisterMetaTable(lua_State L)
    {
        // 1) call base to register the "ModelSceneMetaTable"
        base.RegisterMetaTable(L);

        // 2) Now define "ModelSceneMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = ModelSceneMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("FrameMetaTable")
        var baseMetaName = base.GetMetatableName();
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