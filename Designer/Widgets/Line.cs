using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets;

/// <summary>
/// https://warcraft.wiki.gg/wiki/UIOBJECT_Line
/// </summary>
public class Line : TextureBase
{
    public Line(string? name = null, string? drawLayer = null, string? templateName = null, int subLevel = 0, Frame? parent = null)
        : base("Line", name, drawLayer, templateName, subLevel, parent)
    {
    }
    
    // Line:ClearAllPoints()
    // Line:GetEndPoint() : relativePoint, relativeTo, offsetX, offsetY
    // Line:GetHitRectThickness() : thickness
    // Line:GetStartPoint() : relativePoint, relativeTo, offsetX, offsetY
    // Line:GetThickness() : thickness
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Line_SetEndPoint
    /// Line:SetEndPoint(relativePoint, relativeTo [, offsetX, offsetY])
    /// </summary>
    /// <param name="relativePoint"></param>
    /// <param name="relativeTo"></param>
    /// <param name="offsetX"></param>
    /// <param name="offsetY"></param>
    public void SetEndPoint(string? relativePoint, Frame? relativeTo, float offsetX, float offsetY)
    {
        
    }
    
    // Line:SetHitRectThickness(thickness)
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Line_SetStartPoint
    /// Line:SetStartPoint(relativePoint, relativeTo [, offsetX, offsetY])
    /// </summary>
    /// <param name="relativePoint"></param>
    /// <param name="relativeTo"></param>
    /// <param name="offsetX"></param>
    /// <param name="offsetY"></param>
    public void SetStartPoint(string? relativePoint, Region? relativeTo, float offsetX, float offsetY)
    {
        
    }

    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Line_SetThickness
    /// Line:SetThickness(thickness)
    /// </summary>
    /// <param name="thickness"></param>
    public void SetThickness(float thickness)
    {
        
    }
}