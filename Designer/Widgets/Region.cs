using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets;

public class Region : ScriptRegion
{
    protected string _drawLayer { get; set; }

    protected Region(string objectType, string? name, string? drawLayer, Region? parent) : base(objectType, name, parent)
    {
        _drawLayer = drawLayer ?? "MEDIUM";
    }
    
    // Region:GetAlpha() : alpha - Returns the region's opacity.
    // Region:GetDrawLayer() : layer, sublayer - Returns the layer in which the region is drawn.

    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Region_GetScale
    /// Region:GetEffectiveScale() : effectiveScale - Returns the scale of the region after propagating from its parents.
    /// </summary>
    /// <returns></returns>
    public double GetEffectiveScale()
    {
        return 1.0f;
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Region_GetScale
    /// Region:GetScale() : scale - Returns the scale of the region.
    /// </summary>
    /// <returns></returns>
    public double GetScale()
    {
        return 1.0f;
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Region_GetVertexColor
    /// Region:GetVertexColor() : colorR, colorG, colorB, colorA - Returns the vertex color shading of the region.
    /// </summary>
    /// <returns></returns>
    public int[] GetVertexColor()
    {
        return [1, 1, 1, 1];
    }
    
    // Region:IsIgnoringParentAlpha() : isIgnoring - Returns true if the region is ignoring parent alpha.
    // Region:IsIgnoringParentScale() : isIgnoring - Returns true if the region is ignoring parent scale.
    // Region:IsObjectLoaded() : isLoaded - Returns true if the region is fully loaded.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Region_SetAlpha
    /// Region:SetAlpha(alpha) - Sets the opacity of the region.
    /// </summary>
    /// <param name="alpha"></param>
    public void SetAlpha(float alpha)
    {
        
    }
    
    // Region:SetDrawLayer(layer [, sublevel]) - Sets the layer in which the region is drawn.
    // Region:SetIgnoreParentAlpha(ignore) - Sets whether the region should ignore its parent's alpha.
    // Region:SetIgnoreParentScale(ignore) - Sets whether the region should ignore its parent's scale.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Region_SetScale
    /// Region:SetScale(scale) - Sets the size scaling of the region.
    /// </summary>
    /// <param name="scale"></param>
    public void SetScale(float scale)
    {
        
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Region_SetVertexColor
    /// Region:SetVertexColor(colorR, colorG, colorB [, a]) - Sets the vertex shading color of the region.
    /// </summary>
    /// <param name="colorR"></param>
    /// <param name="colorG"></param>
    /// <param name="colorB"></param>
    /// <param name="colorA"></param>
    public void SetVertexColor(float colorR, float colorG, float colorB, float? colorA)
    {
        
    }
}