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
    private bool SetModelByFileID(int fileID, bool useMips = false)
    {
        return true;
    }
    private int internal_SetModelByFileID(lua_State L)
    {
        var argc = lua_gettop(L);

        var actor = GetThis(L, 1) as ModelSceneActor;
        
        var useMips = false;
        var fileID = (int)lua_tonumber(L, 2);
        if (argc > 2) useMips = lua_toboolean(L, 3) != 0;

        var success = actor.SetModelByFileID(fileID, useMips);
        lua_pushboolean(L, success ? 1 : 0);
        return 1;
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
    private void SetUseCenterForOrigin(bool x, bool y, bool z)
    {
        
    }
    private int internal_SetUseCenterForOrigin(lua_State L)
    {
        var argc = lua_gettop(L);

        var actor = GetThis(L, 1) as ModelSceneActor;
        
        var x = false;
        var y = false;
        var z = false;
        if (argc > 1) x = lua_toboolean(L, 2) != 0;
        if (argc > 2) y = lua_toboolean(L, 3) != 0;
        if (argc > 3) z = lua_toboolean(L, 4) != 0;

        actor?.SetUseCenterForOrigin(x, y, z);
        return 0;
    }
    
    

    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Model_SetYaw
    /// Model:SetYaw(yaw)
    /// </summary>
    /// <param name="yaw"></param>
    private void SetYaw(float yaw)
    {
        
    }
    private int internal_SetYaw(lua_State L)
    {
        var argc = lua_gettop(L);

        var actor = GetThis(L, 1) as ModelSceneActor;
        
        var yaw = (float)lua_tonumber(L, 2);

        actor?.SetYaw(yaw);
        return 0;
    }
    
    
    // ModelSceneActorBase:StopAnimationKit()
    
    // ----------- Virtual Registration ---------------
    
    public override string GetMetatableName() => "ModelSceneActorMetaTable";
        
    public override void RegisterMetaTable(lua_State L)
    {
        // 1) call base to register the "ModelSceneActorMetaTable"
        base.RegisterMetaTable(L);

        // 2) Now define "ModelSceneActorMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = ModelSceneActorMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("DressUpModel")
        var baseMetaName = base.GetMetatableName();
        luaL_getmetatable(L, baseMetaName);
        lua_setmetatable(L, -2); // Sets ModelSceneActorMetaTable's metatable to DressUpModel
        
        // 5) Bind Frame-specific methods
        LuaHelpers.RegisterMethod(L, "SetModelByFileID", internal_SetModelByFileID);
        LuaHelpers.RegisterMethod(L, "SetUseCenterForOrigin", internal_SetUseCenterForOrigin);
        LuaHelpers.RegisterMethod(L, "SetYaw", internal_SetYaw);

        // 6) pop
        lua_pop(L, 1);
    }
}