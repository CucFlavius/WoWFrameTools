using System.Runtime.InteropServices;
using LuaNET.Lua51;
using WoWFrameTools.API;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets;

/// <summary>
/// https://warcraft.wiki.gg/wiki/UIOBJECT_Model
/// </summary>
public class Model : Frame
{
    public Model(string objectType = "Model", string? name = null, Frame? parent = null, string? template = null, int id = 0) : base(objectType, name, parent, template, id)
    {
        
    }
    
    // Model:AdvanceTime()
    // Model:ClearFog()
    // Model:ClearModel()
    // Model:ClearTransform()
    // Model:GetCameraDistance() : distance
    // Model:GetCameraFacing() : radians
    // Model:GetCameraPosition() : positionX, positionY, positionZ
    // Model:GetCameraRoll() : radians
    // Model:GetCameraTarget() : targetX, targetY, targetZ
    // Model:GetDesaturation() : strength
    // Model:GetFacing() : facing - Returns the offset of the rotation angle.
    // Model:GetFogColor() : colorR, colorG, colorB, colorA
    // Model:GetFogFar() : fogFar
    // Model:GetFogNear() : fogNear
    // Model:GetLight() : enabled, light
    // Model:GetModelAlpha() : alpha
    // Model:GetModelDrawLayer() : layer, sublayer
    // Model:GetModelFileID() : modelFileID - Returns the file ID associated with the currently displayed model.
    // Model:GetModelScale() : scale
    // Model:GetPaused() : paused
    // Model:GetPitch() : pitch
    // Model:GetPosition() : positionX, positionY, positionZ
    // Model:GetRoll() : roll
    // Model:GetShadowEffect() : strength
    // Model:GetViewInsets() : left, right, top, bottom
    // Model:GetViewTranslation() : x, y
    // Model:GetWorldScale() : worldScale
    // Model:HasAttachmentPoints() : hasAttachmentPoints
    // Model:HasCustomCamera() : hasCustomCamera
    // Model:IsUsingModelCenterToTransform() : useCenter
    // Model:MakeCurrentCameraCustom()
    // Model:ReplaceIconTexture(asset)
    // Model:SetCamera(cameraIndex) - Selects a predefined camera.
    // Model:SetCameraDistance(distance)
    // Model:SetCameraFacing(radians)
    // Model:SetCameraPosition(positionX, positionY, positionZ)
    // Model:SetCameraRoll(radians)
    // Model:SetCameraTarget(targetX, targetY, targetZ)
    // Model:SetCustomCamera(cameraIndex)
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Model_SetDesaturation
    /// Model:SetDesaturation(strength)
    /// </summary>
    /// <param name="strength"></param>
    private void SetDesaturation(float strength)
    {
        
    }
    private int internal_SetDesaturation(lua_State L)
    {
        var argc = lua_gettop(L);

        var actor = GetThis(L, 1) as Model;
        
        var strength = (float)lua_tonumber(L, 2);

        actor?.SetDesaturation(strength);
        return 0;
    }
    
    // Model:SetFacing(facing) - Rotates the displayed model for the given angle in counter-clockwise direction.
    // Model:SetFogColor(colorR, colorG, colorB [, a]) - Sets the color used for the fogging in the model frame.
    // Model:SetFogFar(fogFar) - Sets the far clipping plane for fogging.
    // Model:SetFogNear(fogNear) - Sets the near clipping plane for fogging.
    // Model:SetGlow(glow)
    // Model:SetLight(enabled, light) - Specifies model lighting.
    // Model:SetModel(asset [, noMip]) - Sets the model to display a certain mesh.
    // Model:SetModelAlpha(alpha)
    // Model:SetModelDrawLayer(layer)
    // Model:SetModelScale(scale)
    // Model:SetParticlesEnabled(enabled)
    // Model:SetPaused(paused)
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Model_SetPitch
    /// Model:SetPitch(pitch)
    /// </summary>
    /// <param name="pitch"></param>
    private void SetPitch(float pitch)
    {
        
    }
    private int internal_SetPitch(lua_State L)
    {
        var argc = lua_gettop(L);

        var actor = GetThis(L, 1) as Model;
        
        var pitch = (float)lua_tonumber(L, 2);

        actor?.SetPitch(pitch);
        return 0;
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Model_SetPosition
    /// Model:SetPosition(positionX, positionY, positionZ) - Positions a model relative to the bottom-left corner.
    /// </summary>
    /// <param name="fileID"></param>
    /// <param name="useMips"></param>
    /// <returns></returns>
    private void SetPosition(float positionX, float positionY, float positionZ)
    {
        
    }
    private int internal_SetPosition(lua_State L)
    {
        var argc = lua_gettop(L);

        var actor = GetThis(L, 1) as Model;
        
        var positionX = (float)lua_tonumber(L, 2);
        var positionY = (float)lua_tonumber(L, 3);
        var positionZ = (float)lua_tonumber(L, 4);

        actor?.SetPosition(positionX, positionY, positionZ);
        return 0;
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Model_SetRoll
    /// Model:SetRoll(roll)
    /// </summary>
    /// <param name="roll"></param>
    private void SetRoll(float roll)
    {
        
    }
    private int internal_SetRoll(lua_State L)
    {
        var argc = lua_gettop(L);

        var actor = GetThis(L, 1) as Model;
        
        var roll = (float)lua_tonumber(L, 2);

        actor?.SetRoll(roll);
        return 0;
    }
    
    // Model:SetSequence(sequence) - Sets the animation-sequence to be played.
    // Model:SetSequenceTime(sequence, timeOffset)
    // Model:SetShadowEffect(strength)
    // Model:SetTransform([translation, rotation, scale])
    // Model:SetViewInsets(left, right, top, bottom)
    // Model:SetViewTranslation(x, y)
    // Model:TransformCameraSpaceToModelSpace(cameraPosition) : modelPosition
    // Model:UseModelCenterToTransform(useCenter)
    
    // ----------- Virtual Registration ---------------
    
    public override string GetMetatableName() => "ModelMetaTable";
        
    public override void RegisterMetaTable(lua_State L)
    {
        // 1) call base to register the "ModelMetaTable"
        base.RegisterMetaTable(L);

        // 2) Now define "ModelMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = ModelMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("FrameMetaTable")
        var baseMetaName = base.GetMetatableName();
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