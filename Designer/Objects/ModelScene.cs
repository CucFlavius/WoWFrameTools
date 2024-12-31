using LuaNET.Lua51;

namespace WoWFrameTools;

public partial class ModelScene : Frame
{
    public ModelScene(lua_State luaState, string? frameType, string? name, Frame? parent, string? template, int id) : base(luaState, frameType, name, parent, template, id)
    {
    }

    private void SetCameraPosition(float positionX, float positionY, float positionZ)
    {
        
    }

    private void SetCameraOrientationByYawPitchRoll(float yaw, float pitch, float roll)
    {
        
    }

    private void SetLightAmbientColor(float colorR, float colorG, float colorB)
    {
        
    }

    private void SetLightDiffuseColor(float colorR, float colorG, float colorB)
    {
        
    }

    private void SetLightVisible(bool visible)
    {
        
    }

    private void SetFogColor(float colorR, float colorG, float colorB)
    {
        
    }

    private void SetFogFar(float far)
    {
        
    }

    private void SetFogNear(float near)
    {

    }

    private void ClearFog()
    {
        
    }
}