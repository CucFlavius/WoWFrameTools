using System.Runtime.InteropServices;
using LuaNET.Lua51;

namespace WoWFrameTools;

public delegate void ScriptHandler(Frame frame, string eventName, string? extraParam = null);

public partial class Frame
{
    private readonly List<FontString> _fontStrings;
    private readonly string? _frameType;
    private readonly lua_State _luaState;
    private readonly string? _name;
    private readonly int _id;
    private Frame? _parent;
    private readonly HashSet<string> _registeredEvents;

    // Dictionary to store script type names and their Lua references
    public readonly Dictionary<string, int> _scriptRefs;
    private readonly Dictionary<string, List<ScriptHandler>?> _scripts;
    public Dictionary<string, object?> Properties { get; private set; } = new Dictionary<string, object?>();
    private readonly string? _template;
    private readonly List<Texture> _textures;
    private List<Line> _lines;
    private float _height;
    private float _offsetX;
    private float _offsetY;
    private string? _point;
    private string? _relativePoint;
    private string? _relativeTo;
    private string? _strata;
    private float _width;
    private bool _visible;

    public Frame(lua_State luaState)
    {
        _luaState = luaState;
        _scripts = new Dictionary<string, List<ScriptHandler>?>();
        _scriptRefs = new Dictionary<string, int>();
        _registeredEvents = [];
        _visible = true; // Frames are visible by default
        LuaRegistryRef = -1; // Initialize to invalid reference
        _textures = [];
        _fontStrings = [];
        _lines = [];
    }

    public Frame(lua_State luaState, string? frameType, string? name, Frame? parent, string? template, int id)
    {
        _luaState = luaState;
        _scripts = new Dictionary<string, List<ScriptHandler>?>();
        _scriptRefs = new Dictionary<string, int>();
        _registeredEvents = [];
        _visible = true; // Frames are visible by default
        LuaRegistryRef = -1; // Initialize to invalid reference
        _textures = [];
        _fontStrings = [];
        _lines = [];

        _frameType = frameType;
        _name = name;
        _parent = parent;
        _template = template;
        _id = id;
    }

    public IntPtr UserdataPtr { get; set; }
    public int LuaRegistryRef { get; set; }
    public GCHandle Handle { get; set; }

    public override string ToString()
    {
        return $"{_name} {_frameType} {_parent} {_template}";
    }

    public bool RegisterEvent(string eventName)
    {
        if (string.IsNullOrWhiteSpace(eventName)) throw new ArgumentException("Event name cannot be null or empty.", nameof(eventName));

        if (!_registeredEvents.Add(eventName)) return false; // Already registered

        return true;
    }

    public void SetScript(string scriptTypeName, ScriptHandler script, int? refIndex = null)
    {
        if (!_scripts.ContainsKey(scriptTypeName)) _scripts[scriptTypeName] = [];

        if (_scripts[scriptTypeName]!.Count > 0)
            // Replace the first (primary) handler
            _scripts[scriptTypeName]![0] = script;
        else
            _scripts[scriptTypeName]?.Add(script);
    }

    public void HookScript(string scriptTypeName, ScriptHandler script, int refIndex)
    {
        if (!_scripts.ContainsKey(scriptTypeName)) _scripts[scriptTypeName] = [];

        _scripts[scriptTypeName]?.Add(script);
        _scriptRefs[scriptTypeName] = refIndex;

        Log.HookScript(scriptTypeName, this);
    }

    public void TriggerEvent(string eventName, string? param = null)
    {
        // Handle 'OnEvent' script type
        if (_scripts.TryGetValue("OnEvent", out var eventHandlers))
            // Make a copy to prevent modification during iteration
            if (eventHandlers != null)
            {
                var handlersCopy = new List<ScriptHandler>(eventHandlers);
                foreach (var handler in handlersCopy)
                    //AnsiConsole.MarkupLine($"[yellow]Invoking OnEvent handler for event '{eventName}'[/]");
                    handler(this, eventName, param);
            }

        switch (eventName)
        {
            // Handle 'OnShow' script type
            case "Show" when _scripts.TryGetValue("OnShow", out var showHandlers):
            {
                if (showHandlers != null)
                {
                    var handlersCopy = new List<ScriptHandler>(showHandlers);
                    foreach (var handler in handlersCopy) handler(this, eventName, param);
                }

                break;
            }
            // Handle 'OnHide' script type
            case "Hide" when _scripts.TryGetValue("OnHide", out var hideHandlers):
            {
                if (hideHandlers != null)
                {
                    var handlersCopy = new List<ScriptHandler>(hideHandlers);
                    foreach (var handler in handlersCopy) handler(this, eventName, param);
                }

                break;
            }
        }
    }

    public void Show()
    {
        if (!_visible)
        {
            _visible = true;
            TriggerEvent("Show");
        }
    }

    public void Hide()
    {
        if (_visible)
        {
            _visible = false;
            TriggerEvent("Hide");
        }
    }

    public bool UnregisterEvent(string? eventName)
    {
        if (string.IsNullOrWhiteSpace(eventName)) throw new ArgumentException("Event name cannot be null or empty.", nameof(eventName));

        if (!_registeredEvents.Contains(eventName)) return false; // Not registered

        _registeredEvents.Remove(eventName);
        Log.UnregisterEvent(eventName, this);

        return true;
    }

    public void UnregisterAllEvents()
    {
        Log.UnregisterEvent("All", this);

        if (_registeredEvents.Count > 0) _registeredEvents.Clear();
    }

    public List<string> GetRegisteredEvents()
    {
        lock (_registeredEvents)
        {
            return [.._registeredEvents];
        }
    }

    public void SetSize(float width, float height)
    {
        _width = width;
        _height = height;
    }

    public void SetPoint(string? point, string? relativeTo, string? relativePoint, float offsetX, float offsetY)
    {
        _point = point;
        _relativeTo = relativeTo;
        _relativePoint = relativePoint;
        _offsetX = offsetX;
        _offsetY = offsetY;
    }

    public void SetFrameStrata(string? strata)
    {
        _strata = strata;
    }

    public Texture CreateTexture(string? name, string? drawLayer, string? templateName, int subLevel)
    {
        var texture = new Texture(_luaState, name, drawLayer, templateName, subLevel);
        _textures.Add(texture);
        return texture;
    }

    public void SetAllPoints(Frame relativeTo, bool doResize)
    {
    }

    public void SetVertexOffset(int vertexIndex, float offsetX, float offsetY)
    {
    }

    public FontString CreateFontString(string? name, string? drawLayer, string? templateName)
    {
        var fontString = new FontString(_luaState, name, drawLayer, templateName);
        _fontStrings.Add(fontString);
        return fontString;
    }

    public void SetHeight(float height)
    {
        _height = height;
    }

    public void SetWidth(float width)
    {
        _width = width;
    }

    public void SetMovable(bool movable)
    {
    }

    public void EnableMouse(bool enable)
    {
    }

    public void SetClampedToScreen(bool clamped)
    {
    }

    public void RegisterForDrag(params string[] buttons)
    {
        foreach (var button in buttons)
            if (!string.IsNullOrEmpty(button))
                //registeredButtons.Add(button);
                Console.WriteLine($"Registered button: {button}");
    }

    private void SetNormalTexture(Texture texture)
    {
    }

    private void SetHighlightTexture(Texture texture)
    {
    }

    private void SetPushedTexture(Texture texture)
    {
    }

    private double GetWidth()
    {
        return _width;
    }

    // Method to handle property setting
    public bool SetProperty(string propertyName, object? value)
    {
        switch (propertyName)
        {
            case "Width":
                if (value is double width)
                {
                    this._width = (float)width;
                    // Apply the width to the underlying UI element
                    // Example: this.UIElement.SetWidth(this.Width);
                    Properties["Width"] = this._width;
                    return true;
                }
                break;

            case "Height":
                if (value is double height)
                {
                    this._height = (float)height;
                    // Apply the height to the underlying UI element
                    // Example: this.UIElement.SetHeight(this.Height);
                    Properties["Height"] = this._height;
                    return true;
                }
                break;

            case "Visible":
                if (value is bool visible)
                {
                    this._visible = visible;
                    // Apply visibility to the underlying UI element
                    // Example: this.UIElement.SetVisible(this.Visible);
                    Properties["Visible"] = this._visible;
                    return true;
                }
                break;

            case "TexturePath":
                if (value is string texturePath)
                {
                    //this.TexturePath = texturePath;
                    // Apply the texture to the frame
                    // Example: this.UIElement.SetTexture(this.TexturePath);
                    //Properties["TexturePath"] = this.TexturePath;
                    return true;
                }
                break;

            // Add cases for other properties...

            default:
                // Handle unknown properties or delegate to a default handler
                // Example: Log.Warn($"Unknown property '{propertyName}'");
                return false;
        }

        // If the property name exists but the value type is incorrect
        // Example: Log.Warn($"Invalid value type for property '{propertyName}'");
        return false;
    }

    private void SetResizable(bool resizable)
    {
        
    }

    private void SetResizeBounds(float minWidth, float minHeight, float? maxWidth, float? maxHeight)
    {
        
    }

    private void SetVertexColor(float colorR, float colorG, float colorB, float? colorA)
    {
        
    }

    private void SetScale(float scale)
    {
        
    }

    private void ClearAllPoints()
    {
        
    }

    private void SetFrameLevel(int level)
    {
        
    }

    private double GetEffectiveScale()
    {
        return 1.0f;
    }

    private double GetHeight()
    {
        return _height;
    }

    private int GetFrameLevel()
    {
        return 1;
    }

    private void SetAlpha(float alpha)
    {
        
    }

    private void SetClipsChildren(bool clips)
    {
        
    }

    private void SetUserPlaced(bool placed)
    {
        
    }

    private void RegisterForClicks(string[] toArray)
    {
        
    }

    private bool SetFont(string? fontFile, int height, string? flags)
    {
        // EditBox has this
        return true;
    }

    private void SetText(string text)
    {
        // EditBox has this
    }

    private void SetAutoFocus(bool autoFocus)
    {
        // EditBox has this
    }

    private void SetMaxLetters(int maxLetters)
    {
        // EditBox has this
    }

    private void SetParent(Frame? parent)
    {
        this._parent = parent;
    }

    private List<Frame> GetChildren()
    {
        return new List<Frame>();
    }

    private Frame GetParent()
    {
        return this._parent!;
    }
    
    public Line CreateLine(string? name, string? drawLayer, string? templateName, int subLevel)
    {
        var line = new Line(_luaState, name, drawLayer, templateName, subLevel);
        _lines.Add(line);
        return line;
    }

}