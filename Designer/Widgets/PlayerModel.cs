using System.Runtime.InteropServices;
using LuaNET.Lua51;
using WoWFrameTools.API;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets;

/// <summary>
/// https://warcraft.wiki.gg/wiki/UIOBJECT_PlayerModel
/// </summary>
public class PlayerModel : Model
{
    public PlayerModel(string objectType = "PlayerModel", string? name = null, Frame? parent = null, string? template = null, int id = 0)
        : base(objectType, name, parent, template, id)
    {
    }
    
    // PlayerModel:ApplySpellVisualKit(spellVisualKitID [, oneShot])
    // PlayerModel:CanSetUnit(unit)
    // PlayerModel:FreezeAnimation(anim, variation, frame) - Freezes an animation at a specific animation frame on the model.
    // PlayerModel:GetDisplayInfo() : displayID
    // PlayerModel:GetDoBlend() : doBlend
    // PlayerModel:GetKeepModelOnHide() : keepModelOnHide
    // PlayerModel:HasAnimation(anim) : hasAnimation - Returns true if the currently displayed model supports the given animation ID.
    // PlayerModel:PlayAnimKit(animKit [, loop])
    // PlayerModel:RefreshCamera()
    // PlayerModel:RefreshUnit()
    // PlayerModel:SetAnimation(anim [, variation]) - Sets the animation to be played by the model.
    // PlayerModel:SetBarberShopAlternateForm()
    // PlayerModel:SetCamDistanceScale(scale)
    // PlayerModel:SetCreature(creatureID [, displayID])
    // PlayerModel:SetDisplayInfo(displayID [, mountDisplayID])
    // PlayerModel:SetDoBlend([doBlend])
    // PlayerModel:SetItem(itemID [, appearanceModID, itemVisualID])
    // PlayerModel:SetItemAppearance(itemAppearanceID [, itemVisualID, itemSubclass])
    // PlayerModel:SetKeepModelOnHide(keepModelOnHide)
    // PlayerModel:SetPortraitZoom(zoom)
    // PlayerModel:SetRotation(radians [, animate])
    // PlayerModel:SetUnit(unit [, blend, useNativeForm]) : success - Sets the model to display the specified unit.
    // PlayerModel:StopAnimKit()
    // PlayerModel:ZeroCachedCenterXY()
}