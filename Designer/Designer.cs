using System.Xml;
using LuaNET.Lua51;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Spectre.Console;
using WoWFrameTools.API;
using WoWFrameTools.Widgets;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools;

public class Designer
{
    private lua_State L;
    public readonly Addon addon;
    public readonly IWindow window;
    private GL? _gl;
    private readonly UI _ui;
    private IInputContext? _inputContext;
    private readonly Vector4D<float> _clearColor = new(0.45f * 255, 0.55f * 255, 0.6f * 255, 1.0f * 255);
    
    public Designer()
    {
        const string addonPath = @"D:\Games\World of Warcraft\_retail_\Interface\AddOns\scenemachine";
        
        var options = WindowOptions.Default with
        {
            Size = new Vector2D<int>(1600, 900),
            Title = "WoW Frame Tools"
        };
        
        window = Window.Create(options);
        window.Load += OnLoad;
        window.Update += OnUpdate;
        window.Render += OnRender;
        window.FramebufferResize += OnFrameBufferResize;
        window.Closing += OnWindowClosing;
        
        _ui = new UI(this);
        addon = new Addon(addonPath);
    }
    
    public void Run()
    {
        // !!! Don't enter the infinite loop, use the other methods !!! //
        window?.Run();
        window?.Dispose();
    }
    
    private void OnLoad()
    {
        if (window == null)
            throw new InvalidOperationException("Window is not initialized.");
        
        window.Center();
        
        _gl = window.CreateOpenGL();
        _inputContext = window.CreateInput();
        
        _ui?.Load(_gl, window, _inputContext);
        
        L = luaL_newstate();
        luaL_openlibs(L);
        
        var task = Task.Run(() => addon.Load(L));
        task.Wait();
        
        foreach (var iKeyboard in _inputContext.Keyboards)
            iKeyboard.KeyDown += KeyDown;
        
        _gl.ClearColor(_clearColor);
    }

    private void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        if (key == Key.Escape)
            window?.Close();
    }

    private void OnUpdate(double deltaTime)
    {
        var deltaTimeF = (float)deltaTime;
        addon.Update(deltaTimeF);
        _ui.Update(deltaTimeF);
    }

    private void OnRender(double deltaTime)
    {
        if (_gl == null)
            throw new InvalidOperationException("OpenGL is not initialized.");
        
        _gl.ClearColor(_clearColor);
        _gl.Clear(ClearBufferMask.ColorBufferBit);
        
        _ui?.Render();
    }
    
    private void OnFrameBufferResize(Vector2D<int> size)
    {
        if (_gl == null)
            throw new InvalidOperationException("OpenGL is not initialized.");
        
        _gl.Viewport(size);
    }
    
    private void OnWindowClosing()
    {
        // Save the saved variables
        addon?.SaveVariables(L);

        // Close Lua state
        lua_close(L);

        AnsiConsole.WriteLine("Lua state closed. Application exiting.");
        
        _ui?.Dispose();
        _inputContext?.Dispose();
        _gl?.Dispose();
    }
}