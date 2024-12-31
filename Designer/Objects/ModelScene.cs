using LuaNET.Lua51;

namespace WoWFrameTools;

public partial class ModelScene : Frame
{
    public ModelScene(lua_State luaState) : base(luaState)
    {
    }

    public ModelScene(lua_State luaState, string? frameType, string? name, Frame? parent, string? template, int id) : base(luaState, frameType, name, parent, template, id)
    {
    }

    private void SetCameraPosition(float positionX, float positionY, float positionZ)
    {
        
    }

    private void SetCameraOrientationByYawPitchRoll(float yaw, float pitch, float roll)
    {
        
    }
}