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
    public void SetDesaturation(float strength)
    {
        
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
    public void SetPitch(float pitch)
    {
        
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Model_SetPosition
    /// Model:SetPosition(positionX, positionY, positionZ) - Positions a model relative to the bottom-left corner.
    /// </summary>
    /// <param name="fileID"></param>
    /// <param name="useMips"></param>
    /// <returns></returns>
    public void SetPosition(float positionX, float positionY, float positionZ)
    {
        
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Model_SetRoll
    /// Model:SetRoll(roll)
    /// </summary>
    /// <param name="roll"></param>
    public void SetRoll(float roll)
    {
        
    }
    
    // Model:SetSequence(sequence) - Sets the animation-sequence to be played.
    // Model:SetSequenceTime(sequence, timeOffset)
    // Model:SetShadowEffect(strength)
    // Model:SetTransform([translation, rotation, scale])
    // Model:SetViewInsets(left, right, top, bottom)
    // Model:SetViewTranslation(x, y)
    // Model:TransformCameraSpaceToModelSpace(cameraPosition) : modelPosition
    // Model:UseModelCenterToTransform(useCenter)
}