using ImGuiNET;

namespace WoWFrameTools;

public abstract class MenuItem
{
    internal string? _name;
    internal Action? _onClick;
    
    public abstract void Render();
}

public class SubMenuItem : MenuItem
{
    private readonly List<MenuItem>? _items;
    
    public SubMenuItem(string name, List<MenuItem>? items = null)
    {
        _name = name;
        _items = items;
    }
    
    public override void Render()
    {
        if (ImGui.BeginMenu(_name))
        {
            if (_items != null)
            {
                foreach (var item in _items)
                {
                    item.Render();
                }
            }
            ImGui.EndMenu();
        }
    }
}

public class Item : MenuItem
{
    public Item (string name, Action? onClick)
    {
        _name = name;
        _onClick = onClick;
    }
    
    public override void Render()
    {
        if (ImGui.MenuItem(_name))
        {
            _onClick?.Invoke();
        }
    }
}