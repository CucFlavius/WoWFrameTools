namespace WoWFrameTools.Widgets;

public struct Point
{
    public string point { get; set; }
    public Frame? relativeTo { get; set; }
    public string? relativePoint { get; set; }
    public float offsetX { get; set; }
    public float offsetY { get; set; }
    
    public Point(string point, Frame? relativeTo = null, string? relativePoint = null, float offsetX = 0, float offsetY = 0)
    {
        this.point = point;
        this.relativeTo = relativeTo;
        this.relativePoint = relativePoint;
        this.offsetX = offsetX;
        this.offsetY = offsetY;
    }
}