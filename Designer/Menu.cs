using ImGuiNET;

namespace WoWFrameTools;

public class Menu
{
    private readonly List<MenuItem> _items;

    public Menu(List<MenuItem> items)
    {
        _items = items;
    }
    
    public void Render()
    {
        // Temporarily change item spacing
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new System.Numerics.Vector2(15.0f, 5.0f)); // Horizontal = 20, Vertical = 5
        
        if (ImGui.BeginMainMenuBar())
        {
            foreach (var item in _items)
            {
                item.Render();
            }
            ImGui.EndMainMenuBar();
        }
        
        ImGui.PopStyleVar();
    }
}