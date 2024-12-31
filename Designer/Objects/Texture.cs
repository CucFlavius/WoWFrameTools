using LuaNET.Lua51;

namespace WoWFrameTools;

public partial class Texture
{
    private readonly lua_State _luaState;
    private readonly Frame _parent;

    public Texture(Frame parent, lua_State luaState, string? name, string? drawLayer, string? templateName, int subLevel)
    {
        _parent = parent;
        _luaState = luaState;
        _name = name;
        _drawLayer = drawLayer;
        _templateName = templateName;
        _subLevel = subLevel;
    }

    protected Texture(Frame parent)
    {
        _parent = parent;
    }

    // Property to store userdata pointer
    public IntPtr UserdataPtr { get; set; }

    // Property to store Lua registry reference index
    public int LuaRegistryRef { get; set; }

    public string? _name { get; set; }
    public string? _drawLayer { get; set; }
    public string? _templateName { get; set; }
    public int _subLevel { get; set; }

    public override string ToString()
    {
        return $"Texture: {_name ?? "nil"} - {_drawLayer ?? "nil"} - {_templateName ?? "nil"} - {_subLevel}";
    }

    public void SetColorTexture(float colorR, float colorG, float colorB, float colorA)
    {
    }

    public void SetAllPoints(string? relativeTo, bool doResize)
    {
    }

    public void SetVertexOffset(int vertexIndex, float offsetX, float offsetY)
    {
    }
    
    private void SetTexture(int fileID, string? wrapModeHorizontal, string? wrapModeVertical, string? filterMode)
    {

    }

    private void SetTexture(string? filePath, string? wrapModeHorizontal, string? wrapModeVertical, string? filterMode)
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

    private void SetRotation(float radians, string? normalizedRotationPoint)
    {
        
    }

    private void SetSize(float width, float height)
    {
        
    }

    private void SetPoint(string? point, string? relativeTo, string? relativePoint, float offsetX, float offsetY)
    {
        
    }

    private int[] GetVertexColor()
    {
        return [1, 1, 1, 1];
    }

    private Frame GetParent()
    {
        return _parent;
    }
}