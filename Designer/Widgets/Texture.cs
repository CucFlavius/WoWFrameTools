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
    
    // ----------- Virtual Registration ---------------
    
    public override string GetMetatableName() => "TextureMetaTable";
        
    public override void RegisterMetaTable(lua_State L)
    {
        // 1) call base to register the "TextureMetaTable"
        base.RegisterMetaTable(L);

        // 2) Now define "TextureMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = TextureMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("TextureBaseMetaTable")
        var baseMetaName = base.GetMetatableName();
        luaL_getmetatable(L, baseMetaName);
        lua_setmetatable(L, -2); // Sets TextureBaseMetaTable's metatable to TextureBaseMetaTable
        
        // 5) Bind Frame-specific methods
        //LuaHelpers.RegisterMethod(L, "RegisterEvent", internal_RegisterEvent);
        //LuaHelpers.RegisterMethod(L, "UnregisterAllEvents", internal_UnregisterAllEvents);
        //LuaHelpers.RegisterMethod(L, "UnregisterEvent", internal_UnregisterEvent);
        //LuaHelpers.RegisterMethod(L, "SetFrameStrata", internal_SetFrameStrata);

        // 6) pop
        lua_pop(L, 1);
    }
}