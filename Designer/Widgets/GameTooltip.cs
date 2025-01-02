using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets;

/// <summary>
/// https://warcraft.wiki.gg/wiki/UIOBJECT_GameTooltip
/// </summary>
public class GameTooltip : Frame
{
    public GameTooltip(string? name = null, Frame? parent = null, string? template = null, int id = 0) : base("GameTooltip", name, parent, template, id)
    {
    }
    
    // GameTooltip:AddAtlas(atlas [, minx, maxx, miny, maxy] or [, textureInfoTable])
    // GameTooltip:AddDoubleLine(textL, textR, rL, gL, bL, rR, gR, bR)
    // GameTooltip:AddFontStrings(leftstring, rightstring) - Dynamically expands the size of a tooltip - New in 1.11.
    // GameTooltip:AddLine(tooltipText [, r, g, b [, wrapText]]) - Appends the new line to the tooltip.
    // GameTooltip:AddSpellByID(spellID)
    // GameTooltip:AddTexture(texture) - Add a texture to the last line added.
    // GameTooltip:AdvanceSecondaryCompareItem()
    // GameTooltip:AppendText(text) - Append text to the end of the first line of the tooltip.
    // GameTooltip:ClearLines() - Clear all lines of tooltip (both left and right ones)
    // GameTooltip:CopyTooltip()
    // GameTooltip:FadeOut()
    // GameTooltip:GetAnchorType() - Returns the current anchoring type.
    // GameTooltip:GetAzeritePowerID()
    // GameTooltip:GetCustomLineSpacing()
    // GameTooltip:GetItem() - Returns name, link.
    // GameTooltip:GetMinimumWidth()
    // GameTooltip:GetOwner() - Returns owner frame, anchor.
    // GameTooltip:GetPadding()
    // GameTooltip:GetSpell() - Returns name, rank, id.
    // GameTooltip:GetUnit() - Returns unit name, unit id.
    // GameTooltip:IsEquippedItem()
    // GameTooltip:IsOwned(frame)
    // GameTooltip:IsUnit(unit) - Returns bool.
    // GameTooltip:NumLines() - Get the number of lines in the tooltip.
    // GameTooltip:ResetSecondaryCompareItem()
    // GameTooltip:SetAchievementByID(id)
    // GameTooltip:SetAction(slot) - Shows the tooltip for the specified action button.
    // GameTooltip:SetAllowShowWithNoLines(bool)
    // GameTooltip:SetAnchorType(anchorType [,Xoffset] [,Yoffset])
    // GameTooltip:SetArtifactItem()
    // GameTooltip:SetArtifactPowerByID()
    // GameTooltip:SetAzeriteEssence(essenceID)
    // GameTooltip:SetAzeriteEssenceSlot(slot)
    // GameTooltip:SetAzeritePower(itemID, itemLevel, powerID[, owningItemLink])
    // GameTooltip:SetBackpackToken(id)
    // GameTooltip:SetBagItem(bag, slot)
    // GameTooltip:SetBagItemChild()
    // GameTooltip:SetBuybackItem(slot)
    // GameTooltip:SetCompanionPet()
    // GameTooltip:SetCompareAzeritePower(itemID, itemLevel, powerID[, owningItemLink])
    // GameTooltip:SetCompareItem(shoppingTooltipTwo, primaryMouseover)
    // GameTooltip:SetConduit(id, rank)
    // GameTooltip:SetCurrencyByID(id)
    // GameTooltip:SetCurrencyToken(tokenId) - Shows the tooltip for the specified token
    // GameTooltip:SetCurrencyTokenByID(currencyID)
    // GameTooltip:SetEquipmentSet(name) - Shows details for the equipment manager set identified by "name".
    // GameTooltip:SetCustomLineSpacing(spacing)
    // GameTooltip:SetEnhancedConduit(conduitID, conduitRank)
    // GameTooltip:SetExistingSocketGem(index, [toDestroy])
    // GameTooltip:SetFrameStack(showhidden) - Shows the mouseover frame stack, used for debugging.
    // GameTooltip:SetGuildBankItem(tab, id) - Shows the tooltip for the specified guild bank item
    // GameTooltip:SetHeirloomByItemID(itemID)
    // GameTooltip:SetHyperlink(itemString or itemLink) - Changes the item which is displayed in the tooltip according to the passed argument.
    // GameTooltip:SetInboxItem(index) - Shows the tooltip for the specified mail inbox item.
    // GameTooltip:SetInstanceLockEncountersComplete(index)
    // GameTooltip:SetInventoryItem(unit, slot[, nameOnly, hideUselessStats])
    // GameTooltip:SetInventoryItemByID(itemID)
    // GameTooltip:SetItemByID(itemID) - Shows the tooltip for a specified Item ID. (added in 4.2.0.14002 along with the Encounter Journal)
    // GameTooltip:SetItemKey(itemID, itemLevel, itemSuffix)
    // GameTooltip:SetLFGDungeonReward(dungeonID, lootIndex)
    // GameTooltip:SetLFGDungeonShortageReward(dungeonID, shortageSeverity, lootIndex)
    // GameTooltip:SetLootCurrency(lootSlot)
    // GameTooltip:SetLootItem(lootSlot)
    // GameTooltip:SetLootRollItem(id) - Shows the tooltip for the specified loot roll item.
    // GameTooltip:SetMerchantCostItem(index, item)
    // GameTooltip:SetMerchantItem(merchantSlot)
    // GameTooltip:SetMinimumWidth(width) - (Formerly SetMoneyWidth)
    // GameTooltip:SetMountBySpellID()
    // GameTooltip:SetOwnedItemByID(ID)
    // GameTooltip:SetOwner(owner, anchor[, x, y])
    // GameTooltip:SetPadding(width, height)
    // GameTooltip:SetPetAction(slot) - Shows the tooltip for the specified pet action.
    // GameTooltip:SetPossession(slot)
    // GameTooltip:SetPvpBrawl()
    // GameTooltip:SetPvpTalent(talentID[, talentIndex])
    // GameTooltip:SetQuestCurrency(type, index)
    // GameTooltip:SetQuestItem(type, index)
    // GameTooltip:SetQuestLogCurrency(type, index)
    // GameTooltip:SetQuestLogItem(type, index)
    // GameTooltip:SetQuestLogRewardSpell(rewardSpellIndex[, questID])
    // GameTooltip:SetQuestLogSpecialItem(index)
    // GameTooltip:SetQuestPartyProgress(questID, omitTitle, ignoreActivePlayer)
    // GameTooltip:SetQuestRewardSpell(rewardSpellIndex)
    // GameTooltip:SetRecipeRankInfo(recipeID, learnedRank)
    // GameTooltip:SetRecipeReagentItem(recipeID, reagentIndex)
    // GameTooltip:SetRecipeResultItem(recipeID)
    // GameTooltip:SetRuneforgeResultItem(itemID, itemLevel [, powerID, modifiers])
    // GameTooltip:SetSendMailItem()
    // GameTooltip:SetShapeshift(slot) - Shows the tooltip for the specified shapeshift form.
    // GameTooltip:SetShrinkToFitWrapped()
    // GameTooltip:SetSocketedItem()
    // GameTooltip:SetSocketedRelic(relicSlotIndex)
    // GameTooltip:SetSocketGem(index)
    // GameTooltip:SetSpecialPvpBrawl()
    // GameTooltip:SetSpellBookItem(spellId, bookType) - Shows the tooltip for the specified spell in the spellbook.
    // GameTooltip:SetSpellByID(spellId) - Shows the tooltip for the specified spell by global spell ID.
    // GameTooltip:SetTalent(talentIndex [, isInspect, talentGroup, inspectedUnit, classId]) - Shows the tooltip for the specified talent.
    // GameTooltip:SetText(text, r, g, b[, alphaValue[, textWrap]]) - Set the text of the tooltip.
    // GameTooltip:SetTotem(slot)
    // GameTooltip:SetToyByItemID(itemID)
    // GameTooltip:SetTradePlayerItem(tradeSlot)
    // GameTooltip:SetTradeTargetItem(tradeSlot)
    // GameTooltip:SetTrainerService(index)
    // GameTooltip:SetTransmogrifyItem(slotId) - Shows the tooltip when there is a pending (de)transmogrification
    // GameTooltip:SetUnit(unit[, hideStatus])
    // GameTooltip:SetUnitAura(unit, auraIndex [, filter]) - Shows the tooltip for a unit's aura. (Exclusive to 3.x.x / WotLK)
    // GameTooltip:SetUnitBuff(unit, buffIndex [, raidFilter]) - Shows the tooltip for a unit's buff.
    // GameTooltip:SetUnitDebuff(unit, buffIndex [, raidFilter]) - Shows the tooltip for a unit's debuff.
    // GameTooltip:SetUpgradeItem()
    // GameTooltip:SetVoidDepositItem(slotIndex) - Shows the tooltip for the specified Void Transfer deposit slot (added in 4.3.0)
    // GameTooltip:SetVoidItem(slotIndex) - Shows the tooltip for the specified Void Storage slot (added in 4.3.0)
    // GameTooltip:SetVoidWithdrawalItem(slotIndex) - Shows the tooltip for the specified Void Transfer withdrawal slot (added in 4.3.0)
    // GameTooltip:SetWeeklyReward(itemDBID)
    // GameTooltip:SetAuctionItem(type, index) - Shows the tooltip for the specified auction item.
    // GameTooltip:SetAuctionSellItem()
    // GameTooltip:SetCraftItem(index, reagent)
    // GameTooltip:SetCraftSpell(index)
    // GameTooltip:SetTrackingSpell()
    // GameTooltip:SetTradeSkillItem(index [, reagent])
    
    // ----------- Virtual Registration ---------------
    
    public override string GetMetatableName() => "GameTooltipMetaTable";
        
    public override void RegisterMetaTable(lua_State L)
    {
        // 1) call base to register the "GameTooltipMetaTable"
        base.RegisterMetaTable(L);

        // 2) Now define "GameTooltipMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = self
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Bind methods
        //LuaHelpers.RegisterMethod(L, "RegisterEvent", internal_RegisterEvent);
        //LuaHelpers.RegisterMethod(L, "UnregisterAllEvents", internal_UnregisterAllEvents);

        // Optional __gc
        lua_pushcfunction(L, internal_ObjectGC);
        lua_setfield(L, -2, "__gc");

        // 5) pop
        lua_pop(L, 1);
    }
}