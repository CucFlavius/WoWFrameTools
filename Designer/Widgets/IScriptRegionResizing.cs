using System;
using System.Collections.Generic;

namespace WoWFrameTools.Widgets
{
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/UIOBJECT_ScriptRegion
    /// </summary>
    public partial interface IScriptRegionResizing
    {
        Dictionary<string, Point> _points { get; set; }
        float _width { get; set; }
        float _height { get; set; }
        
        // ScriptRegionResizing:AdjustPointsOffset(x, y) #secureframe - Adjusts the x and y offset of the region.
        void ClearAllPoints();
        // ScriptRegionResizing:ClearPoint(point) - Removes an anchor point from the region by name.
        // ScriptRegionResizing:ClearPointsOffset() #secureframe - Resets the x and y offset on the region to zero.
        // ScriptRegionResizing:GetNumPoints() : numPoints - Returns the number of anchor points for the region.
        // ScriptRegionResizing:GetPoint([anchorIndex [, resolveCollapsed]]) : point, relativeTo, relativePoint, offsetX, offsetY #restrictedframe - Returns an anchor point for the region.
        // ScriptRegionResizing:GetPointByName(point [, resolveCollapsed]) : point, relativeTo, relativePoint, offsetX, offsetY - Returns an anchor point by name for the region.
        void SetAllPoints(Frame? relativeTo = null, bool doResize = true);
        void SetHeight(float height);
        void SetPoint(string point, Frame? relativeTo = null, string? relativePoint = null, float offsetX = 0, float offsetY = 0);
        void SetSize(float x, float y);
        void SetWidth(float width);
    }
}