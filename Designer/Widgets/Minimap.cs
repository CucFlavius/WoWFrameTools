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
    public Minimap(string? name = null, Frame? parent = null, string? template = null, int id = 0) : base("Minimap", name, parent, template, id)
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
}