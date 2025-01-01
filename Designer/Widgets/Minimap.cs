using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets;

/// <summary>
/// https://warcraft.wiki.gg/wiki/UIOBJECT_Minimap
/// </summary>
public class Minimap : Frame
{
    public Minimap(string objectType, string? name = null, Frame? parent = null, string? template = null, int id = 0) : base(objectType, name, parent, template, id)
    {
    }
    
    // Minimap:GetPingPosition() : positionX, positionY - Get the last ping location.
    // Minimap:GetZoom() : zoomFactor - Get the current zoom level.
    // Minimap:GetZoomLevels() : zoomLevels - Get the maximum zoom level.
    // Minimap:PingLocation([locationX, locationY]) - Perform a ping at the specified location.
    // Minimap:SetArchBlobInsideAlpha(alpha)
    // Minimap:SetArchBlobInsideTexture(asset)
    // Minimap:SetArchBlobOutsideAlpha(alpha)
    // Minimap:SetArchBlobOutsideTexture(asset)
    // Minimap:SetArchBlobRingAlpha(alpha)
    // Minimap:SetArchBlobRingScalar(scalar)
    // Minimap:SetArchBlobRingTexture(asset)
    // Minimap:SetBlipTexture(asset) - Set the file to use for blips (ObjectIcons)
    // Minimap:SetCorpsePOIArrowTexture(asset)
    // Minimap:SetIconTexture(asset)
    // Minimap:SetMaskTexture(asset)
    // Minimap:SetPOIArrowTexture(asset)
    // Minimap:SetPlayerTexture(asset) - Set the file to use for the player arrow texture
    // Minimap:SetQuestBlobInsideAlpha(alpha)
    // Minimap:SetQuestBlobInsideTexture(asset)
    // Minimap:SetQuestBlobOutsideAlpha(alpha)
    // Minimap:SetQuestBlobOutsideTexture(asset)
    // Minimap:SetQuestBlobRingAlpha(alpha)
    // Minimap:SetQuestBlobRingScalar(scalar)
    // Minimap:SetQuestBlobRingTexture(asset)
    // Minimap:SetStaticPOIArrowTexture(asset)
    // Minimap:SetTaskBlobInsideAlpha(alpha)
    // Minimap:SetTaskBlobInsideTexture(asset)
    // Minimap:SetTaskBlobOutsideAlpha(alpha)
    // Minimap:SetTaskBlobOutsideTexture(asset)
    // Minimap:SetTaskBlobRingAlpha(alpha)
    // Minimap:SetTaskBlobRingScalar(scalar)
    // Minimap:SetTaskBlobRingTexture(asset)
    // Minimap:SetZoom(zoomFactor) - Set the current zoom level 
    // Minimap:UpdateBlips()
    
    // ----------- Virtual Registration ---------------
    
    public override string GetMetatableName() => "MinimapMetaTable";
        
    public override void RegisterMetaTable(lua_State L)
    {
        // 1) call base to register the "MinimapMetaTable"
        base.RegisterMetaTable(L);

        // 2) Now define "MinimapMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = self
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Bind methods
        //LuaHelpers.RegisterMethod(L, "RegisterEvent", internal_RegisterEvent);
        //LuaHelpers.RegisterMethod(L, "UnregisterAllEvents", internal_UnregisterAllEvents);

        // Optional __gc
        lua_pushcfunction(L, internal_FrameGC);
        lua_setfield(L, -2, "__gc");

        // 5) pop
        lua_pop(L, 1);
    }

    private int internal_FrameGC(lua_State L)
    {
        // Retrieve the table
        if (lua_istable(L, 1) == 0)
        {
            return 0;
        }

        // Retrieve the Frame userdata from the table's __frame field
        lua_pushstring(L, "__frame");
        lua_gettable(L, 1); // table.__frame
        if (lua_islightuserdata(L, -1) == 0)
        {
            lua_pop(L, 1);
            return 0;
        }

        IntPtr frameUserdataPtr = (IntPtr)lua_touserdata(L, -1);
        lua_pop(L, 1); // Remove __frame userdata from the stack

        // Retrieve the Frame instance
        if (API.UIObjects._frameRegistry.TryGetValue(frameUserdataPtr, out var frame))
        {
            // Free the GCHandle
            if (frame.Handle.IsAllocated)
            {
                frame.Handle.Free();
            }

            // Remove from registry
            API.UIObjects._frameRegistry.Remove(frameUserdataPtr);

            // Perform any additional cleanup if necessary
            // Example: frame.Dispose();
        }

        return 0;
    }
}