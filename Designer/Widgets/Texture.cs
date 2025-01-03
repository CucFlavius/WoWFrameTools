using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets;

/// <summary>
/// https://warcraft.wiki.gg/wiki/UIOBJECT_Texture
/// </summary>
public class Texture : TextureBase
{
    public Texture(string? name = null, string? drawLayer = null, string? templateName = null, int subLevel = 0, Frame? parent = null)
        : base("Texture", name, drawLayer, templateName, subLevel, parent)
    {
    }
    
    // Texture:AddMaskTexture(mask)
    // Texture:GetMaskTexture(index) : mask
    // Texture:GetNumMaskTextures() : count
    // Texture:RemoveMaskTexture(mask)
    
    public override string ToString()
    {
        return $"Texture: {GetName() ?? "nil"} - {_drawLayer ?? "nil"}";
    }
}