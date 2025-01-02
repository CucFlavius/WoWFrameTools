﻿using System.Runtime.InteropServices;
using LuaNET.Lua51;
using WoWFrameTools.API;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets;

/// <summary>
/// https://warcraft.wiki.gg/wiki/UIOBJECT_DressUpModel
/// </summary>
public class DressUpModel : Model
{
    public DressUpModel(string objectType = "DressUpModel", string? name = null, Frame? parent = null, string? template = null, int id = 0)
        : base(objectType, name, parent, template, id)
    {
    }
    
    // DressUpModel:Dress()
    // DressUpModel:GetAutoDress() : enabled
    // DressUpModel:GetItemTransmogInfo(inventorySlot) : itemTransmogInfo
    // DressUpModel:GetItemTransmogInfoList() : infoList
    // DressUpModel:GetObeyHideInTransmogFlag() : enabled
    // DressUpModel:GetSheathed() : sheathed
    // DressUpModel:GetUseTransmogChoices() : enabled
    // DressUpModel:GetUseTransmogSkin() : enabled
    // DressUpModel:IsGeoReady() : ready
    // DressUpModel:IsSlotAllowed(slot) : allowed
    // DressUpModel:IsSlotVisible(slot) : visible
    // DressUpModel:SetAutoDress([enabled])
    // DressUpModel:SetItemTransmogInfo(itemTransmogInfo [, inventorySlot, ignoreChildItems]) : result
    // DressUpModel:SetObeyHideInTransmogFlag([enabled])
    // DressUpModel:SetSheathed([sheathed, hideWeapons])
    // DressUpModel:SetUseTransmogChoices([enabled])
    // DressUpModel:SetUseTransmogSkin([enabled])
    // DressUpModel:TryOn(linkOrItemModifiedAppearanceID [, handSlotName, spellEnchantID]) : result - Previews an item on the model.
    // DressUpModel:Undress()
    // DressUpModel:UndressSlot(inventorySlot)
    
    // ----------- Virtual Registration ---------------
    
    public override string GetMetatableName() => "DressUpModelMetaTable";
        
    public override void RegisterMetaTable(lua_State L)
    {
        // 1) call base to register the "DressUpModelMetaTable"
        base.RegisterMetaTable(L);

        // 2) Now define "DressUpModelMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = DressUpModelMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("PlayerModelMetaTable")
        var baseMetaName = base.GetMetatableName();
        luaL_getmetatable(L, baseMetaName);
        lua_setmetatable(L, -2); // Sets DressUpModelMetaTable's metatable to PlayerModelMetaTable
        
        // 5) Bind Frame-specific methods
        //LuaHelpers.RegisterMethod(L, "RegisterEvent", internal_RegisterEvent);
        
        // 6) pop
        lua_pop(L, 1);
    }
}