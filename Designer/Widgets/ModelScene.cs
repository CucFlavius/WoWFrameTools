using System.Runtime.InteropServices;
using LuaNET.Lua51;
using WoWFrameTools.API;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets;

public class ModelScene : Frame
{
    public readonly List<ModelSceneActor> _actors;

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
    public ModelSceneActor CreateActor(string? name = null, string? template = null)
    {
        var actor = new ModelSceneActor(name, template);
        _actors.Add(actor);
        return actor;
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
    public void SetCameraOrientationByYawPitchRoll(float yaw, float pitch, float roll)
    {
        
    }
    
    // ModelScene:SetDrawLayer(layer)
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_ModelScene_SetLightAmbientColor
    /// ModelScene:SetLightAmbientColor(colorR, colorG, colorB)
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public void SetLightAmbientColor(float colorR, float colorG, float colorB)
    {
        
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_ModelScene_SetLightDiffuseColor
    /// ModelScene:SetLightDiffuseColor(colorR, colorG, colorB)
    /// </summary>
    /// <param name="colorR"></param>
    /// <param name="colorG"></param>
    /// <param name="colorB"></param>
    public void SetLightDiffuseColor(float colorR, float colorG, float colorB)
    {
        
    }
    
    // ModelScene:SetLightDirection(directionX, directionY, directionZ)
    // ModelScene:SetLightPosition(positionX, positionY, positionZ)
    // ModelScene:SetLightType(lightType)
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_ModelScene_SetLightVisible
    /// ModelScene:SetLightVisible([visible])
    /// </summary>
    /// <param name="visible"></param>
    public void SetLightVisible(bool visible)
    {
        
    }
    
    // ModelScene:SetPaused(paused [, affectsGlobalPause])
    // ModelScene:TakeActor()
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Model_ClearFog
    /// Model ModelScene:ClearFog()
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public void ClearFog()
    {
        
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
    public void SetCameraPosition(float positionX, float positionY, float positionZ)
    {
        
    }
    
    // Model ModelScene:SetDesaturation(strength)
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_ModelScene_SetFogColor
    /// Model ModelScene:SetFogColor(colorR, colorG, colorB)
    /// </summary>
    /// <param name="colorR"></param>
    /// <param name="colorG"></param>
    /// <param name="colorB"></param>
    public void SetFogColor(float colorR, float colorG, float colorB)
    {
        
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Model_SetFogFar
    /// Model ModelScene:SetFogFar(far)
    /// </summary>
    /// <param name="far"></param>
    public void SetFogFar(float far)
    {
        
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Model_SetFogNear
    /// Model ModelScene:SetFogNear(near)
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public void SetFogNear(float near)
    {

    }
    
    // Model ModelScene:SetViewInsets(left, right, top, bottom)
    // Model ModelScene:SetViewTranslation(translationX, translationY)
}