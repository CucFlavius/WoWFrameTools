using System.Runtime.InteropServices;
using LuaNET.Lua51;
using WoWFrameTools.API;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets;

/// <summary>
/// https://warcraft.wiki.gg/wiki/UIOBJECT_ModelSceneActor
/// </summary>
public class ModelSceneActor : DressUpModel
{
    public ModelSceneActor(string? name = null, string? template = null) : base("ModelSceneActor", name, null, template, 0)
    {
        
    }
    
    // ModelSceneActor:AttachToMount(rider, animation [, spellKitVisualID]) : success
    // ModelSceneActor:CalculateMountScale(rider) : scale
    // ModelSceneActor:DressPlayerSlot(invSlot)
    // ModelSceneActor:GetPaused() : paused, globalPaused
    // ModelSceneActor:ReleaseFrontEndCharacterDisplays() : success
    // ModelSceneActor:ResetNextHandSlot()
    // ModelSceneActor:SetFrontEndLobbyModelFromDefaultCharacterDisplay(characterIndex) : success
    // ModelSceneActor:SetModelByHyperlink(link) : success
    // ModelSceneActor:SetPaused(paused [, affectsGlobalPause])
    // ModelSceneActor:SetPlayerModelFromGlues([sheatheWeapons, autoDress, hideWeapons, usePlayerNativeForm]) : success
    
    // ModelSceneActorBase:GetActiveBoundingBox() : boxBottom, boxTop
    // ModelSceneActorBase:GetAnimation() : animation
    // ModelSceneActorBase:GetAnimationBlendOperation() : blendOp
    // ModelSceneActorBase:GetAnimationVariation() : variation
    // ModelSceneActorBase:GetMaxBoundingBox() : boxBottom, boxTop
    // ModelSceneActorBase:GetModelPath() : path
    // ModelSceneActorBase:GetModelUnitGUID() : guid
    // ModelSceneActorBase:GetParticleOverrideScale() : scale
    // ModelSceneActorBase:GetSpellVisualKit() : spellVisualKitID
    // ModelSceneActorBase:GetYaw() : yaw
    // ModelSceneActorBase:IsLoaded() : isLoaded
    // ModelSceneActorBase:IsUsingCenterForOrigin() : x, y, z
    // ModelSceneActorBase:PlayAnimationKit(animationKit [, isLooping])
    // ModelSceneActorBase:SetAnimation(animation [, variation, animSpeed, animOffsetSeconds])
    // ModelSceneActorBase:SetAnimationBlendOperation(blendOp)
    // ModelSceneActorBase:SetModelByCreatureDisplayID(creatureDisplayID [, useActivePlayerCustomizations]) : success
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_ModelSceneActorBase_SetModelByFileID
    /// ModelSceneActorBase:SetModelByFileID(asset [, useMips]) : success
    /// </summary>
    /// <param name="fileID"></param>
    /// <param name="useMips"></param>
    /// <returns></returns>
    public bool SetModelByFileID(int fileID, bool useMips = false)
    {
        return true;
    }
    
    // ModelSceneActorBase:SetModelByPath(asset [, useMips]) : success
    // ModelSceneActorBase:SetModelByUnit(unit [, sheatheWeapons, autoDress, hideWeapons, usePlayerNativeForm, holdBowString]) : success
    // ModelSceneActorBase:SetParticleOverrideScale([scale])
    // ModelSceneActorBase:SetSpellVisualKit([spellVisualKitID, oneShot])

    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_ModelSceneActorBase_SetUseCenterForOrigin
    /// ModelSceneActorBase:SetUseCenterForOrigin([x, y, z])
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void SetUseCenterForOrigin(bool x, bool y, bool z)
    {
        
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Model_SetYaw
    /// Model:SetYaw(yaw)
    /// </summary>
    /// <param name="yaw"></param>
    public void SetYaw(float yaw)
    {
        
    }
    
    // ModelSceneActorBase:StopAnimationKit()
}