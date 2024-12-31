using LuaNET.Lua51;

namespace WoWFrameTools;

public partial class FontString
{
    private readonly string? _drawLayer;
    private readonly lua_State _luaState;
    private readonly string? _name;
    private readonly string? _templateName;

    public FontString(lua_State luaState, string? name, string? drawLayer, string? templateName)
    {
        _luaState = luaState;
        _name = name;
        _drawLayer = drawLayer;
        _templateName = templateName;
    }

    // Property to store userdata pointer
    public IntPtr UserdataPtr { get; set; }

    // Property to store Lua registry reference index
    public int LuaRegistryRef { get; set; }

    public override string ToString()
    {
        return $"FontString: {_name ?? "nil"} - {_drawLayer ?? "nil"} - {_templateName ?? "nil"}";
    }

    public bool SetFont(string? fontFile, int height, string? flags)
    {
        return true;
    }

    public void SetPoint(string? point, string? relativeTo, string? relativePoint, float offsetX, float offsetY)
    {
    }

    public void SetText(string text)
    {
    }

    public void SetJustifyV(string justify)
    {
    }

    public void SetJustifyH(string justify)
    {
    }

    private void SetAllPoints(Frame? relativeToFrame, bool doResize)
    {
    }
}