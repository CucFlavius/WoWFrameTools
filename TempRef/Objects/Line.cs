using System.Runtime.InteropServices;
using LuaNET.Lua51;

namespace WoWFrameTools;

public partial class Line
{
    private readonly lua_State _luaState;
    private readonly string? _name;
    private readonly string? _drawLayer;
    private readonly string? _templateName;
    private readonly int _subLevel;
    
    public IntPtr UserdataPtr { get; set; }
    public int LuaRegistryRef { get; set; }
    public GCHandle Handle { get; set; }
    
    public Line(lua_State luaState, string? name, string? drawLayer, string? templateName, int subLevel)
    {
        _luaState = luaState;
        _name = name;
        _drawLayer = drawLayer;
        _templateName = templateName;
        _subLevel = subLevel;
    }

    private void SetThickness(float thickness)
    {
        
    }

    private void SetTexture(int textureAssetInt, string? wrapModeHorizontal, string? wrapModeVertical, string? filterMode)
    {
        
    }

    private void SetTexture(string? textureAssetStr, string? wrapModeHorizontal, string? wrapModeVertical, string? filterMode)
    {
        
    }

    private void SetTexCoord(float left, float right, float top, float bottom)
    {
        
    }

    private void SetTexCoord(float uLx, float uLy, float lLx, float lLy, float uRx, float uRy, float lRx, float lRy)
    {
        
    }

    private void SetVertexColor(float colorR, float colorG, float colorB, float? colorA)
    {
        
    }

    private void Show()
    {
        
    }

    private void Hide()
    {
        
    }


}