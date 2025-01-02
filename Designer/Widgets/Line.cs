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
    private void SetEndPoint(string? relativePoint, Frame? relativeTo, float offsetX, float offsetY)
    {
        
    }
    private int internal_SetEndPoint(lua_State L)
    {
        var line = GetThis(L, 1) as Line;

        var relativePoint = luaL_checkstring(L, 2);
        Frame? relativeTo = null;
        int offsetX = 0;
        int offsetY = 0;
        
        if (lua_isstring(L, 3) != 0)
        {
            relativeTo = GetThis(L, 2) as Frame;
        }
        else if (lua_isnumber(L, 3) != 0)
        {
            offsetX = (int)lua_tonumber(L, 3);
            offsetY = (int)lua_tonumber(L, 4);
        }

        line?.SetStartPoint(relativePoint, relativeTo, offsetX, offsetY);

        lua_pushboolean(L, 1);
        return 1;
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
    private void SetStartPoint(string? relativePoint, Frame? relativeTo, float offsetX, float offsetY)
    {
        
    }
    private int internal_SetStartPoint(lua_State L)
    {
        var line = GetThis(L, 1) as Line;

        var relativePoint = luaL_checkstring(L, 2);
        Frame? relativeTo = null;
        int offsetX = 0;
        int offsetY = 0;
        
        if (lua_isstring(L, 3) != 0)
        {
            relativeTo = GetThis(L, 2) as Frame;
        }
        else if (lua_isnumber(L, 3) != 0)
        {
            offsetX = (int)lua_tonumber(L, 3);
            offsetY = (int)lua_tonumber(L, 4);
        }

        line?.SetStartPoint(relativePoint, relativeTo, offsetX, offsetY);

        lua_pushboolean(L, 1);
        return 1;
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Line_SetThickness
    /// Line:SetThickness(thickness)
    /// </summary>
    /// <param name="thickness"></param>
    private void SetThickness(float thickness)
    {
        
    }
    private int internal_SetThickness(lua_State L)
    {
        var line = GetThis(L, 1) as Line;
        if (line == null) return 0;

        var thickness = (float)luaL_checknumber(L, 2);
        line.SetThickness(thickness);

        return 0;
    }
    
    // ----------- Virtual Registration ---------------
    
    public override string GetMetatableName() => "LineMetaTable";
        
    public override void RegisterMetaTable(lua_State L)
    {
        // 1) call base to register the "LineMetaTable"
        base.RegisterMetaTable(L);

        // 2) Now define "LineMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = LineMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("TextureBaseMetaTable")
        var baseMetaName = base.GetMetatableName();
        luaL_getmetatable(L, baseMetaName);
        lua_setmetatable(L, -2); // Sets LineMetaTable's metatable to TextureBaseMetaTable
        
        // 5) Bind Frame-specific methods
        LuaHelpers.RegisterMethod(L, "SetThickness", internal_SetThickness);
        LuaHelpers.RegisterMethod(L, "SetEndPoint", internal_SetEndPoint);
        LuaHelpers.RegisterMethod(L, "SetStartPoint", internal_SetStartPoint);

        // Optional __gc
        lua_pushcfunction(L, internal_ObjectGC);
        lua_setfield(L, -2, "__gc");

        // 6) pop
        lua_pop(L, 1);
    }
}