using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;

namespace WoWFrameTools;

public class UI
{
    private readonly GL _gl;
    private readonly ImGuiController? _controller;
    private readonly List<Menu> _menus;
    
    public UI(GL gl, IWindow window, IInputContext inputContext)
    {
        _gl = gl;
        _controller = new ImGuiController(gl, window, inputContext);
        _menus = [];
        SetImguiStyle();
    }

    public void AddMenu(Menu menu)
    {
        _menus.Add(menu);
    }
    
    public void Update(float deltaTime)
    {
        // Feed the input events to our ImGui controller, which passes them through to ImGui.
        _controller?.Update(deltaTime);
    }
    
    public void Render()
    {
        foreach (var menu in _menus)
        {
            menu.Render();
        }

        // Render UI components
        ImGui.ShowDemoWindow();
        
        _controller?.Render();
    }
    
    private void SetImguiStyle()
    {
        // Access the ImGui style
        ImGui.StyleColorsClassic();
        var style = ImGui.GetStyle();
        
        // Change the spacing between items
        style.ItemSpacing = new System.Numerics.Vector2(1.0f, 5.0f);
        style.ItemInnerSpacing = new System.Numerics.Vector2(1.0f, 1.0f);
        style.FramePadding = new System.Numerics.Vector2(10.0f, 6.0f);
        style.ScrollbarSize = 20.0f;
        style.ScrollbarRounding = 2.0f;
        style.WindowRounding = 3.0f;
        style.TabRounding = 2.0f;
        style.GrabRounding = 2.0f;
        style.FrameRounding = 2.0f;
        
        var colors = style.Colors;

        // Base Colors (Brighter Background)
        colors[(int)ImGuiCol.WindowBg] = new System.Numerics.Vector4(0.18f, 0.18f, 0.18f, 1.00f);
        colors[(int)ImGuiCol.ChildBg] = new System.Numerics.Vector4(0.22f, 0.22f, 0.22f, 1.00f);
        colors[(int)ImGuiCol.FrameBg] = new System.Numerics.Vector4(0.28f, 0.28f, 0.28f, 1.00f);
        colors[(int)ImGuiCol.FrameBgHovered] = new System.Numerics.Vector4(0.35f, 0.30f, 0.30f, 1.00f);
        colors[(int)ImGuiCol.FrameBgActive] = new System.Numerics.Vector4(0.40f, 0.35f, 0.35f, 1.00f);
        colors[(int)ImGuiCol.TitleBg] = new System.Numerics.Vector4(0.15f, 0.15f, 0.15f, 1.00f);
        colors[(int)ImGuiCol.TitleBgActive] = new System.Numerics.Vector4(0.20f, 0.18f, 0.18f, 1.00f);
        colors[(int)ImGuiCol.TitleBgCollapsed] = new System.Numerics.Vector4(0.12f, 0.12f, 0.12f, 1.00f);
        colors[(int)ImGuiCol.MenuBarBg] = new System.Numerics.Vector4(0.25f, 0.25f, 0.25f, 1.00f);

        // Accent Colors (Brighter Buttons)
        colors[(int)ImGuiCol.Button] = new System.Numerics.Vector4(0.60f, 0.20f, 0.20f, 1.00f); // Brighter Red
        colors[(int)ImGuiCol.ButtonHovered] = new System.Numerics.Vector4(0.75f, 0.25f, 0.25f, 1.00f); // Even Brighter on Hover
        colors[(int)ImGuiCol.ButtonActive] = new System.Numerics.Vector4(0.85f, 0.30f, 0.30f, 1.00f); // Brightest when Active

        colors[(int)ImGuiCol.Header] = new System.Numerics.Vector4(0.50f, 0.15f, 0.15f, 1.00f);
        colors[(int)ImGuiCol.HeaderHovered] = new System.Numerics.Vector4(0.65f, 0.20f, 0.20f, 1.00f);
        colors[(int)ImGuiCol.HeaderActive] = new System.Numerics.Vector4(0.75f, 0.25f, 0.25f, 1.00f);

        colors[(int)ImGuiCol.Tab] = new System.Numerics.Vector4(0.40f, 0.20f, 0.20f, 1.00f);
        colors[(int)ImGuiCol.TabHovered] = new System.Numerics.Vector4(0.60f, 0.25f, 0.25f, 1.00f);
        colors[(int)ImGuiCol.TabActive] = new System.Numerics.Vector4(0.75f, 0.30f, 0.30f, 1.00f);
        colors[(int)ImGuiCol.TabUnfocused] = new System.Numerics.Vector4(0.35f, 0.18f, 0.18f, 1.00f);
        colors[(int)ImGuiCol.TabUnfocusedActive] = new System.Numerics.Vector4(0.50f, 0.22f, 0.22f, 1.00f);

        colors[(int)ImGuiCol.CheckMark] = new System.Numerics.Vector4(0.90f, 0.30f, 0.30f, 1.00f);
        colors[(int)ImGuiCol.SliderGrab] = new System.Numerics.Vector4(0.80f, 0.25f, 0.25f, 1.00f);
        colors[(int)ImGuiCol.SliderGrabActive] = new System.Numerics.Vector4(0.90f, 0.30f, 0.30f, 1.00f);

        colors[(int)ImGuiCol.Separator] = new System.Numerics.Vector4(0.35f, 0.18f, 0.18f, 1.00f);
        colors[(int)ImGuiCol.SeparatorHovered] = new System.Numerics.Vector4(0.50f, 0.22f, 0.22f, 1.00f);
        colors[(int)ImGuiCol.SeparatorActive] = new System.Numerics.Vector4(0.65f, 0.28f, 0.28f, 1.00f);

        // Scrollbar
        colors[(int)ImGuiCol.ScrollbarBg] = new System.Numerics.Vector4(0.20f, 0.20f, 0.20f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrab] = new System.Numerics.Vector4(0.45f, 0.20f, 0.20f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrabHovered] = new System.Numerics.Vector4(0.60f, 0.25f, 0.25f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrabActive] = new System.Numerics.Vector4(0.75f, 0.30f, 0.30f, 1.00f);
    }
    
    public void Dispose()
    {
        _controller?.Dispose();
    }
}
