using System.Numerics;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using WoWFrameTools.Widgets;

namespace WoWFrameTools;

public class UI
{
    private Designer _designer;
    private GL _gl;
    private ImGuiController? _controller;
    private readonly List<Menu> _menus;
    private readonly MainMenu? _mainMenu;
    private string _layoutFilePath = "imgui_layout.ini";
    private ScriptObject _selectedFrame;

    public UI(Designer designer)
    {
        _designer = designer;
        _menus = [];
        _mainMenu = new MainMenu(this);
    }

    public void Load(GL gl, IWindow window, IInputContext inputContext)
    {
        _gl = gl;
        _controller = new ImGuiController(gl, window, inputContext);
        
        // Enable docking
        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        
        SetImguiStyle();
        
        _mainMenu?.Load();
        
        LoadLayout();
    }
    
    private void SaveLayout()
    {
        // Save the current ImGui layout to a file
        ImGui.SaveIniSettingsToDisk(_layoutFilePath);
    }
    
    private void LoadLayout()
    {
        // Load the ImGui layout from a file
        ImGui.LoadIniSettingsFromDisk(_layoutFilePath);
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
        // Render UI components
        if (!_designer.addon.isLoaded)
        {
            // Show loading screen
            ImGui.SetWindowSize(new Vector2(_designer.window.Size.X, _designer.window.Size.Y));
            ImGui.Begin("Loading", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse);
            ImGui.Text("Loading...");
            ImGui.ProgressBar(_designer.addon.loadPercentage, new System.Numerics.Vector2(300, 0), "");
            ImGui.End();
        }
        
        // Docking
        CreateDockingSpaceAndMainMenu();
        
        // Rest of the UI
        if (_designer.addon.isLoaded)
        {
            ImGui.ShowDemoWindow();
            
            ImGui.Begin("Addon");
            RenderAddon();
            ImGui.End();

            ImGui.Begin("Hierarchy");
            
            // Temporarily adjust style to reduce vertical padding
            //var originalFramePadding = ImGui.GetStyle().FramePadding;
            //var originalItemSpacing = ImGui.GetStyle().ItemSpacing;

            //ImGui.GetStyle().FramePadding = new System.Numerics.Vector2(4.0f, 100.0f); // Adjust the vertical padding (y = 1.0f)
            //ImGui.GetStyle().ItemSpacing = new System.Numerics.Vector2(4.0f, 0.0f);  // Adjust spacing between items (y = 1.0f)
            
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(4.0f, 10.0f));
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(4.0f, 0.0f));
            
            RenderFrameHierarchy(API.UIObjects.UIParent);
            
            // Restore original style
            ImGui.PopStyleVar();
            //ImGui.GetStyle().FramePadding = originalFramePadding;
            //ImGui.GetStyle().ItemSpacing = originalItemSpacing;
            
            ImGui.End();
        }

        _controller?.Render();
    }

    private void RenderFrameHierarchy(Widgets.ScriptObject? frameScriptObject)
    {
        if (frameScriptObject == null)
            return;

        var frameName = frameScriptObject.GetName();
        if (string.IsNullOrEmpty(frameName))
            frameName = frameScriptObject.UserdataPtr.ToString();

        var frameType = frameScriptObject.GetType().ToString().Substring(22);

        int numChildren = 0;
        if (frameScriptObject is Widgets.Frame frame0)
        {
            numChildren = frame0.GetNumChildren();
        }

        var flags = ImGuiTreeNodeFlags.OpenOnDoubleClick | ImGuiTreeNodeFlags.OpenOnArrow;// | ImGuiTreeNodeFlags.Framed;

        if (_selectedFrame == frameScriptObject)
        {
            flags |= ImGuiTreeNodeFlags.Selected;
        }

        if (numChildren == 0)
        {
            flags |= ImGuiTreeNodeFlags.Leaf;
        }
        
        // Create a unique ID for the node to avoid conflicts
        ImGui.PushID(frameScriptObject.GetHashCode());

        // Check if the tree node is expanded
        var opened = ImGui.TreeNodeEx(frameName, flags);

        // Handle clicks on the tree node, even if it's not expanded
        if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
        {
            _selectedFrame = frameScriptObject; // Set the selected frame
        }

        // Render children if the node is expanded
        if (opened)
        {
            if (frameScriptObject is Widgets.Frame frame)
            {
                foreach (var child in frame.GetChildren())
                {
                    RenderFrameHierarchy(child);
                }
            }

            ImGui.TreePop();
        }

        ImGui.PopID();
    }

    
    private void RenderAddon()
    {
        ImGui.Text($"{_designer.addon.toc?.Title}");
    }
    
    private void CreateDockingSpaceAndMainMenu()
    {
        // Check if Docking is enabled
        var io = ImGui.GetIO();
        if ((io.ConfigFlags & ImGuiConfigFlags.DockingEnable) != 0)
        {
            var viewport = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(viewport.Pos);
            ImGui.SetNextWindowSize(viewport.Size);
            ImGui.SetNextWindowViewport(viewport.ID);

            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);

            ImGui.Begin("DockSpace", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse |
                                     ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove |
                                     ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoBringToFrontOnFocus |
                                     ImGuiWindowFlags.NoNavFocus);

            foreach (var menu in _menus)
            {
                menu.Render();
            }
            
            var dockspaceId = ImGui.GetID("MainDockSpace");
            ImGui.DockSpace(dockspaceId, Vector2.Zero, ImGuiDockNodeFlags.PassthruCentralNode);

            ImGui.End();
            ImGui.PopStyleVar(2);
        }
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
        SaveLayout();
        _controller?.Dispose();
    }
}
