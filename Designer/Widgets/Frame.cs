using System.Runtime.InteropServices;
using LuaNET.Lua51;
using WoWFrameTools.API;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets;

public class Frame : Region
{
    public readonly HashSet<string> _registeredEvents;
    public readonly HashSet<string> _registeredForDragButtons;
    
    public readonly List<Frame?> _children;
    public readonly List<Texture> _textures;
    public readonly List<FontString> _fontStrings;
    public readonly List<Line> _lines;
    
    public string? _strata;        // https://warcraft.wiki.gg/wiki/Frame_Strata
    public bool _isMovable;

    public Frame(string objectType = "Frame", string? name = null, Frame? parent = null, string? template = null, int id = 0) : base(objectType, name, null, parent)
    {
        _registeredEvents = [];
        _registeredForDragButtons = [];
        
        _children = [];
        _textures = [];
        _fontStrings = [];
        _lines = [];
    }
    
    // Frame:AbortDrag()
    // Frame:CanChangeAttribute() : canChangeAttributes
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_CreateFontString
    /// Frame:CreateFontString([name, drawLayer, templateName]) : line - Creates a fontstring.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="drawLayer"></param>
    /// <param name="templateName"></param>
    /// <returns></returns>
    public FontString CreateFontString(string? name, string? drawLayer, string? templateName)
    {
        var fontString = new FontString(name, drawLayer, templateName, this);
        _fontStrings.Add(fontString);
        return fontString;
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_CreateLine
    /// Frame:CreateLine([name, drawLayer, templateName, subLevel]) : line - Draws a line.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="drawLayer"></param>
    /// <param name="templateName"></param>
    /// <param name="subLevel"></param>
    /// <returns></returns>
    public Line CreateLine(string? name, string? drawLayer, string? templateName, int subLevel)
    {
        var line = new Line(name, drawLayer, templateName, subLevel, this);
        _lines.Add(line);
        return line;
    }
    
    // Frame:CreateMaskTexture([name, drawLayer, templateName, subLevel]) : maskTexture - Creates a mask texture.

    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_CreateTexture
    /// Frame:CreateTexture([name, drawLayer, templateName, subLevel]) : texture - Creates a texture.
    /// </summary>
    /// <param name="name">The global variable name that will be assigned, or nil for an anonymous texture.</param>
    /// <param name="drawLayer">DrawLayer - The layer the texture should be drawn in.</param>
    /// <param name="templateName">Comma-delimited list of names of virtual textures (created in XML) to inherit from.</param>
    /// <param name="subLevel">[-8, 7] = 0 - The level of the sublayer if textures overlap</param>
    /// <returns></returns>
    public Texture CreateTexture(string? name = null, string? drawLayer = null, string? templateName = null, int subLevel = 0)
    {
        var texture = new Texture(name, drawLayer, templateName, subLevel, null);
        _textures.Add(texture);
        return texture;
    }
    
    // Frame:DesaturateHierarchy(desaturation [, excludeRoot])
    // Frame:DisableDrawLayer(layer) - Prevents display of the frame on the specified draw layer.
    // Frame:DoesClipChildren() : clipsChildren
    // Frame:EnableDrawLayer(layer) - Allows display of the frame on the specified draw layer.
    // Frame:EnableGamePadButton([enable]) - Allows the receipt of gamepad button inputs for this frame.
    // Frame:EnableGamePadStick([enable]) - Allows the receipt of gamepad stick inputs for this frame.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_EnableKeyboard
    /// Frame:EnableKeyboard([enable]) - Allows this frame to receive keyboard input.
    /// </summary>
    /// <param name="enable"></param>
    public void EnableKeyboard(bool enable)
    {
    }
    
    // Frame:ExecuteAttribute(attributeName, unpackedPrimitiveType, ...) : success, unpackedPrimitiveType, ...
    // Frame:GetAttribute(attributeName) : value - Returns the value of a secure frame attribute.
    // Frame:GetBoundsRect() : left, bottom, width, height - Returns the calculated bounding box of the frame and all of its descendant regions.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_GetChildren
    /// Frame:GetChildren() : child1, ... - Returns a list of child frames belonging to the frame.
    /// </summary>
    /// <returns></returns>
    public List<Frame> GetChildren()
    {
        return this._children!;
    }
    
    // Frame:GetClampRectInsets() : left, right, top, bottom - Returns the frame's clamp rectangle offsets.
    // Frame:GetDontSavePosition() : dontSave
    // Frame:GetEffectiveAlpha() : effectiveAlpha - Returns the effective alpha after propagating from the parent region.
    // Frame:GetEffectivelyFlattensRenderLayers() : flatten - Returns true if render layer flattening has been implicitly enabled.
    // Frame:GetFlattensRenderLayers() : flatten - Returns true if render layer flattening has been enabled.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_GetFrameLevel
    /// Frame:GetFrameLevel() : frameLevel - Returns the frame level of the frame.
    /// </summary>
    /// <returns></returns>
    public int GetFrameLevel()
    {
        return 1;
    }
    
    // Frame:GetFrameStrata() : strata - Returns the layering strata of the frame.
    // Frame:GetHitRectInsets() : left, right, top, bottom - Returns the insets of the frame's hit rectangle.
    // Frame:GetHyperlinksEnabled() : enabled - Returns true if mouse interaction with hyperlinks on the frame is enabled.
    // Frame:GetID() : id - Returns the frame's numeric identifier.
    // Frame:GetNumChildren() : numChildren - Returns the number of child frames belonging to the frame.
    // Frame:GetNumRegions() : numRegions - Returns the number of non-Frame child regions belonging to the frame.
    // Frame:GetPropagateKeyboardInput() : propagate - Returns whether the frame propagates keyboard events.
    // Frame:GetRaisedFrameLevel() : frameLevel
    // Frame:GetRegions() : region1, ... - Returns a list of non-Frame child regions belonging to the frame.
    // Frame:GetResizeBounds() : minWidth, minHeight, maxWidth, maxHeight - Returns the minimum and maximum size of the frame for user resizing.
    // Frame:GetScale() : frameScale -> Region:GetScale
    // Frame:GetWindow() : window
    // Frame:HasFixedFrameLevel() : isFixed
    // Frame:HasFixedFrameStrata() : isFixed
    // Frame:InterceptStartDrag(delegate)
    // Frame:IsClampedToScreen() : clampedToScreen - Returns whether a frame is prevented from being moved off-screen.
    // Frame:IsEventRegistered(eventName) : isRegistered, unit1, ... - Returns whether a frame is registered to an event.
    // Frame:IsGamePadButtonEnabled() : enabled - Checks if this frame is configured to receive gamepad button inputs.
    // Frame:IsGamePadStickEnabled() : enabled - Checks if this frame is configured to receive gamepad stick inputs.
    // Frame:IsKeyboardEnabled() : enabled - Returns true if keyboard interactivity is enabled for the frame.
    // Frame:IsMovable() : isMovable - Returns true if the frame is movable.
    // Frame:IsResizable() : resizable - Returns true if the frame can be resized by the user.
    // Frame:IsToplevel() : isTopLevel - Returns whether this frame should raise its frame level on mouse interaction.
    // Frame:IsUserPlaced() : isUserPlaced - Returns whether the frame has been moved by the user.
    // Frame:IsUsingParentLevel() : usingParentLevel
    // Frame:LockHighlight() - Sets the frame or button to always be drawn highlighted.
    // Frame:Lower() - Reduces the frame's frame level below all other frames in its strata.
    // Frame:Raise() - Increases the frame's frame level above all other frames in its strata.
    // Frame:RegisterAllEvents() - Flags the frame to receive all events.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_RegisterEvent
    /// Frame:RegisterEvent(eventName) : registered - Registers the frame to an event.
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public bool RegisterEvent(string eventName)
    {
        if (string.IsNullOrWhiteSpace(eventName))
            throw new ArgumentException("Event name cannot be null or empty.", nameof(eventName));

        if (!_registeredEvents.Add(eventName)) return false; // Already registered

        return true;
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_RegisterForDrag
    /// Frame:RegisterForDrag([button1, ...]) - Registers the frame for dragging with a mouse button.
    /// </summary>
    /// <param name="buttons"></param>
    public void RegisterForDrag(params string[] buttons)
    {
        foreach (var button in buttons)
        {
            if (!string.IsNullOrEmpty(button))
            {
                _registeredForDragButtons.Add(button);
            }
        }
    }
    
    // Frame:RegisterUnitEvent(eventName [, unit1, ...]) : registered - Registers the frame for a specific event, triggering only for the specified units.
    // Frame:RotateTextures(radians [, x, y])
    // Frame:SetAttribute(attributeName, value) - Sets an attribute on the frame.
    // Frame:SetAttributeNoHandler(attributeName, value) - Sets an attribute on the frame without triggering the OnAttributeChanged script handler.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_SetClampedToScreen
    /// Frame:SetClampedToScreen(clampedToScreen) - Prevents the frame from moving off-screen.
    /// </summary>
    /// <param name="clamped"></param>
    public void SetClampedToScreen(bool clamped)
    {
    }
    
    // Frame:SetClampRectInsets(left, right, top, bottom) - Controls how much of the frame may be moved off-screen.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_SetClipsChildren
    /// Frame:SetClipsChildren(clipsChildren)
    /// </summary>
    /// <param name="clips"></param>
    public void SetClipsChildren(bool clips)
    {
        
    }
    
    // Frame:SetDontSavePosition(dontSave)
    // Frame:SetDrawLayerEnabled(layer [, isEnabled])
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_SetFixedFrameLevel
    /// Frame:SetFixedFrameLevel(isFixed)
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public void SetFixedFrameLevel(bool isFixed)
    {
        
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_SetFixedFrameStrata
    /// Frame:SetFixedFrameStrata(isFixed)
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public void SetFixedFrameStrata(bool isFixed)
    {
        
    }
    
    // Frame:SetFlattensRenderLayers(flatten) - Controls whether all subregions are composited into a single render layer.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_SetFrameLevel
    /// Frame:SetFrameLevel(frameLevel) - Sets the level at which the frame is layered relative to others in its strata.
    /// </summary>
    /// <param name="frameLevel"></param>
    public void SetFrameLevel(int frameLevel)
    {
        
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_SetFrameStrata
    /// Frame:SetFrameStrata(strata) - Sets the layering strata of the frame.
    /// </summary>
    /// <param name="strata"></param>
    public void SetFrameStrata(string? strata)
    {
        _strata = strata;
    }
    
    // Frame:SetHighlightLocked(locked)
    // Frame:SetHitRectInsets(left, right, top, bottom) #secureframe - Returns the insets of the frame's hit rectangle.
    // Frame:SetHyperlinksEnabled([enabled]) - Allows mouse interaction with hyperlinks on the frame.
    // Frame:SetID(id) - Returns the frame's numeric identifier.
    // Frame:SetIsFrameBuffer(isFrameBuffer) - Controls whether a frame is rendered to its own framebuffer prior to being composited atop the UI.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_SetMovable
    /// Frame:SetMovable(movable) - Sets whether the frame can be moved.
    /// </summary>
    /// <param name="movable"></param>
    public void SetMovable(bool movable)
    {
        _isMovable = movable;
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_SetPropagateKeyboardInput
    /// Frame:SetPropagateKeyboardInput(propagate) #nocombat - Sets whether keyboard input is consumed by this frame or propagates to further frames.
    /// </summary>
    /// <param name="propagate"></param>
    public void SetPropagateKeyboardInput(bool propagate)
    {
        
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_SetResizable
    /// Frame:SetResizable(resizable) - Sets whether the frame can be resized by the user.
    /// </summary>
    /// <param name="resizable"></param>
    public void SetResizable(bool resizable)
    {
        
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_SetResizeBounds
    /// Frame:SetResizeBounds(minWidth, minHeight [, maxWidth, maxHeight]) - Sets the minimum and maximum size of the frame for user resizing.
    /// </summary>
    /// <param name="minWidth"></param>
    /// <param name="minHeight"></param>
    /// <param name="maxWidth"></param>
    /// <param name="maxHeight"></param>
    public void SetResizeBounds(float minWidth, float minHeight, float? maxWidth, float? maxHeight)
    {
        
    }
    
    // Frame:SetToplevel(topLevel) #secureframe - Controls whether a frame should raise its frame level on mouse interaction.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_SetUserPlaced
    /// Frame:SetUserPlaced(userPlaced) - Sets whether a frame has been moved by the user and will be saved in the layout cache.
    /// </summary>
    /// <param name="placed"></param>
    public void SetUserPlaced(bool placed)
    {
        
    }
    
    // Frame:SetUsingParentLevel(usingParentLevel)
    // Frame:SetWindow([window])
    // Frame:StartMoving([alwaysStartFromMouse]) - Begins repositioning the frame via mouse movement.
    // Frame:StartSizing([resizePoint, alwaysStartFromMouse]) - Begins resizing the frame via mouse movement.
    // Frame:StopMovingOrSizing() - Stops moving or resizing the frame.
    // Frame:UnlockHighlight() - Sets the frame or button to not always be drawn highlighted.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_UnregisterAllEvents
    /// Frame:UnregisterAllEvents() - Unregisters all events from the frame.
    /// </summary>
    public void UnregisterAllEvents()
    {
        if (_registeredEvents.Count > 0) _registeredEvents.Clear();
        Log.UnregisterEvent("All", this);
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_UnregisterEvent
    /// Frame:UnregisterEvent(eventName) : registered - Unregisters an event from the frame.
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public bool UnregisterEvent(string? eventName)
    {
        if (string.IsNullOrWhiteSpace(eventName)) throw new ArgumentException("Event name cannot be null or empty.", nameof(eventName));

        if (!_registeredEvents.Contains(eventName)) return false; // Not registered

        _registeredEvents.Remove(eventName);
        Log.UnregisterEvent(eventName, this);

        return true;
    }
}