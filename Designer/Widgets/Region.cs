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
    private double GetEffectiveScale()
    {
        return 1.0f;
    }
    private int internal_GetEffectiveScale(lua_State L)
    {
        var frame = GetThis(L, 1) as Region;
        var scale = frame?.GetEffectiveScale() ?? 1;

        lua_pushnumber(L, scale);
        return 1;
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Region_GetScale
    /// Region:GetScale() : scale - Returns the scale of the region.
    /// </summary>
    /// <returns></returns>
    private double GetScale()
    {
        return 1.0f;
    }
    private int internal_GetScale(lua_State L)
    {
        var frame = GetThis(L, 1) as Region;
        var scale = frame?.GetScale() ?? 1;

        lua_pushnumber(L, scale);
        return 1;
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Region_GetVertexColor
    /// Region:GetVertexColor() : colorR, colorG, colorB, colorA - Returns the vertex color shading of the region.
    /// </summary>
    /// <returns></returns>
    private int[] GetVertexColor()
    {
        return [1, 1, 1, 1];
    }
    private int internal_GetVertexColor(lua_State L)
    {
        var region = GetThis(L, 1) as Region;

        var color = region?.GetVertexColor();

        lua_pushnumber(L, color?[0] ?? 1);
        lua_pushnumber(L, color?[1] ?? 1);
        lua_pushnumber(L, color?[2] ?? 1);
        lua_pushnumber(L, color?[3] ?? 1);

        return 4;
    }
    
    // Region:IsIgnoringParentAlpha() : isIgnoring - Returns true if the region is ignoring parent alpha.
    // Region:IsIgnoringParentScale() : isIgnoring - Returns true if the region is ignoring parent scale.
    // Region:IsObjectLoaded() : isLoaded - Returns true if the region is fully loaded.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Region_SetAlpha
    /// Region:SetAlpha(alpha) - Sets the opacity of the region.
    /// </summary>
    /// <param name="alpha"></param>
    private void SetAlpha(float alpha)
    {
        
    }
    private int internal_SetAlpha(lua_State L)
    {
        var frame = GetThis(L, 1) as Region;
        var alpha = (float)lua_tonumber(L, 2);

        frame?.SetAlpha(alpha);

        return 0;
    }
    
    
    // Region:SetDrawLayer(layer [, sublevel]) - Sets the layer in which the region is drawn.
    // Region:SetIgnoreParentAlpha(ignore) - Sets whether the region should ignore its parent's alpha.
    // Region:SetIgnoreParentScale(ignore) - Sets whether the region should ignore its parent's scale.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Region_SetScale
    /// Region:SetScale(scale) - Sets the size scaling of the region.
    /// </summary>
    /// <param name="scale"></param>
    private void SetScale(float scale)
    {
        
    }
    private int internal_SetScale(lua_State L)
    {
        var frame = GetThis(L, 1) as Region;
        var scale = (float)lua_tonumber(L, 2);

        frame?.SetScale(scale);

        return 0;
    }
    
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Region_SetVertexColor
    /// Region:SetVertexColor(colorR, colorG, colorB [, a]) - Sets the vertex shading color of the region.
    /// </summary>
    /// <param name="colorR"></param>
    /// <param name="colorG"></param>
    /// <param name="colorB"></param>
    /// <param name="colorA"></param>
    private void SetVertexColor(float colorR, float colorG, float colorB, float? colorA)
    {
        
    }
    private int internal_SetVertexColor(lua_State L)
    {
        var region = GetThis(L, 1) as Region;

        var argc = lua_gettop(L);
        if (argc < 3)
        {
            Log.ErrorL(L, "SetVertexColor requires at least 3 arguments: colorR, colorG, colorB.");
            return 0; // Unreachable
        }

        var colorR = (float)lua_tonumber(L, 2);
        var colorG = (float)lua_tonumber(L, 3);
        var colorB = (float)lua_tonumber(L, 4);
        float? colorA = null;

        if (argc >= 5)
        {
            colorA = (float)lua_tonumber(L, 5);
        }

        region?.SetVertexColor(colorR, colorG, colorB, colorA);

        lua_pushboolean(L, 1);
        return 1;
    }
    
    // ----------- Virtual Registration ---------------
    
    public override string GetMetatableName() => "RegionMetaTable";
        
    public override void RegisterMetaTable(lua_State L)
    {
        // 1) Register the base class's metatable first
        base.RegisterMetaTable(L);

        // 2) Define "RegionMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = RegionMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("ScriptRegionMetaTable")
        var baseMetaName = base.GetMetatableName();
        luaL_getmetatable(L, baseMetaName);
        lua_setmetatable(L, -2); // Sets RegionMetaTable's metatable to ScriptRegionMetaTable

        // 5) Bind Region-specific methods
        LuaHelpers.RegisterMethod(L, "GetVertexColor", internal_GetVertexColor);
        LuaHelpers.RegisterMethod(L, "SetVertexColor", internal_SetVertexColor);
        LuaHelpers.RegisterMethod(L, "SetScale", internal_SetScale);
        LuaHelpers.RegisterMethod(L, "GetScale", internal_GetScale);
        LuaHelpers.RegisterMethod(L, "GetEffectiveScale", internal_GetEffectiveScale);
        LuaHelpers.RegisterMethod(L, "SetAlpha", internal_SetAlpha);

        // Optional __gc
        lua_pushcfunction(L, internal_ObjectGC);
        lua_setfield(L, -2, "__gc");

        // 6) pop
        lua_pop(L, 1);
    }
}