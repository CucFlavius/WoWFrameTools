using System.Runtime.InteropServices;
using LuaNET.Lua51;
using WoWFrameTools.API;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets;

public class Frame : Region
{
    private readonly HashSet<string> _registeredEvents;
    private readonly HashSet<string> _registeredForDragButtons;
    
    private readonly List<Frame?> _children;
    private readonly List<Texture> _textures;
    private readonly List<FontString> _fontStrings;
    private readonly List<Line> _lines;
    
    private string? _strata;        // https://warcraft.wiki.gg/wiki/Frame_Strata
    private bool _isMovable;

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
    private FontString CreateFontString(string? name, string? drawLayer, string? templateName)
    {
        var fontString = new FontString(name, drawLayer, templateName, this);
        _fontStrings.Add(fontString);
        return fontString;
    }
    private int internal_CreateFontString(lua_State L)
    {
        var frame = GetThis(L, 1) as Frame;

        var argc = lua_gettop(L);

        string? name = null;
        string? drawLayer = null;
        string? templateName = null;

        if (argc > 1) name = lua_tostring(L, 2);
        if (argc > 2) drawLayer = lua_tostring(L, 3);
        if (argc > 3) templateName = lua_tostring(L, 4);

        var fontString = frame?.CreateFontString(name, drawLayer, templateName);

        // Allocate a GCHandle to prevent the Frame from being garbage collected
        var handle = GCHandle.Alloc(fontString);
        var handlePtr = GCHandle.ToIntPtr(handle);

        // Create userdata with the size of IntPtr
        var userdataPtr = (IntPtr)lua_newuserdata(L, (UIntPtr)IntPtr.Size);

        // Write the handlePtr into the userdata memory
        Marshal.WriteIntPtr(userdataPtr, handlePtr);

        // Set the metatable for the userdata
        luaL_getmetatable(L, "FontStringMetaTable");
        lua_setmetatable(L, -2);

        // Add the Frame to the registry for later retrieval
        UIObjects._fontStringRegistry[userdataPtr] = fontString;

        // Assign the userdataPtr and LuaRegistryRef to the Frame instance
        fontString.UserdataPtr = userdataPtr;

        // Create a reference to the userdata in the registry
        lua_pushvalue(L, -1); // Push the userdata
        var refIndex = luaL_ref(L, LUA_REGISTRYINDEX);
        fontString.LuaRegistryRef = refIndex;

        Log.CreateFontString(fontString);
        
        return 1;
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
    private Line CreateLine(string? name, string? drawLayer, string? templateName, int subLevel)
    {
        var line = new Line(name, drawLayer, templateName, subLevel, this);
        _lines.Add(line);
        return line;
    }
    private int internal_CreateLine(lua_State L)
    {
        // Retrieve the Frame object (assuming the first argument is the Frame userdata)
        var frame = GetThis(L, 1) as Frame;
        if (frame == null)
        {
            lua_pushnil(L);
            return 1;
        }

        // Retrieve arguments: name, layer
        string? name = null;
        if (lua_gettop(L) >= 2) name = lua_tostring(L, 2);
        string? drawLayer = null;
        if (lua_gettop(L) >= 3) drawLayer = lua_tostring(L, 3);
        string? templateName = null;
        if (lua_gettop(L) >= 4) templateName = lua_tostring(L, 4);
        int subLevel = 0;
        if (lua_gettop(L) >= 5) subLevel = (int)lua_tonumber(L, 5);

        // Create the line
        Line line = frame.CreateLine(name, drawLayer, templateName, subLevel);
        
        // Allocate a GCHandle to prevent the Frame from being garbage collected
        var handle = GCHandle.Alloc(line);
        var handlePtr = GCHandle.ToIntPtr(handle);

        // Create userdata with the size of IntPtr
        var userdataPtr = (IntPtr)lua_newuserdata(L, (UIntPtr)IntPtr.Size);

        // Write the handlePtr into the userdata memory
        Marshal.WriteIntPtr(userdataPtr, handlePtr);

        // Set the metatable for the userdata
        luaL_getmetatable(L, "LineMetaTable");
        lua_setmetatable(L, -2);

        // Add the Frame to the registry for later retrieval
        UIObjects._lineRegistry[userdataPtr] = line;

        // Assign the userdataPtr and LuaRegistryRef to the Frame instance
        line.UserdataPtr = userdataPtr;

        // Create a reference to the userdata in the registry
        lua_pushvalue(L, -1); // Push the userdata
        var refIndex = luaL_ref(L, LUA_REGISTRYINDEX);
        line.LuaRegistryRef = refIndex;

        Log.CreateLine(line);
        
        return 1;
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
    private Texture CreateTexture(string? name = null, string? drawLayer = null, string? templateName = null, int subLevel = 0)
    {
        var texture = new Texture(name, drawLayer, templateName, subLevel, this);
        _textures.Add(texture);
        return texture;
    }
    private int internal_CreateTexture(lua_State L)
    {
        var frame = GetThis(L, 1) as Frame;
        
        // Retrieve arguments: name, layer
        string? textureName = null;
        if (lua_gettop(L) >= 2) textureName = lua_tostring(L, 2);
        string? drawLayer = null;
        if (lua_gettop(L) >= 3) drawLayer = lua_tostring(L, 3);
        string? templateName = null;
        if (lua_gettop(L) >= 4) templateName = lua_tostring(L, 4);
        int subLevel = 0;
        if (lua_gettop(L) >= 5) subLevel = (int)lua_tonumber(L, 5);

        // Create the texture
        var texture = frame?.CreateTexture(textureName, drawLayer, templateName, subLevel);

        // Allocate a GCHandle to prevent garbage collection
        var handle = GCHandle.Alloc(texture);
        var handlePtr = GCHandle.ToIntPtr(handle);

        // Create userdata with the size of IntPtr
        var textureUserdataPtr = (IntPtr)lua_newuserdata(L, (UIntPtr)IntPtr.Size);

        // Write the handlePtr into the userdata memory
        Marshal.WriteIntPtr(textureUserdataPtr, handlePtr);

        // Set the metatable for the userdata
        luaL_getmetatable(L, "TextureMetaTable"); // Ensure TextureMetaTable is set up
        lua_setmetatable(L, -2);

        // Add the Frame to the registry for later retrieval
        UIObjects._textureRegistry[textureUserdataPtr] = texture;

        // Assign the userdataPtr and LuaRegistryRef to the Frame instance
        texture.UserdataPtr = textureUserdataPtr;

        // Create a reference to the userdata in the Lua registry
        lua_pushvalue(L, -1); // Push the userdata
        int refIndex = luaL_ref(L, LUA_REGISTRYINDEX);
        texture.LuaRegistryRef = refIndex;

        // **Proceed to create a Lua table and embed the userdata**
        // Create a new Lua table
        lua_newtable(L); // Push a new table onto the stack

        // Set the Frame userdata in the table with a hidden key
        lua_pushstring(L, "__frame");
        lua_pushlightuserdata(L, (UIntPtr)textureUserdataPtr); // Value (light userdata)
        lua_settable(L, -3); // table["__frame"] = userdata

        // Set the metatable for the table to handle method calls and property accesses
        luaL_getmetatable(L, "TextureMetaTable"); // Push the TextureMetaTable
        lua_setmetatable(L, -2); // setmetatable(table, "TextureMetaTable")

        Log.CreateTexture(texture);

        return 1; // Return the table
    }
    
    
    // Frame:DesaturateHierarchy(desaturation [, excludeRoot])
    // Frame:DisableDrawLayer(layer) - Prevents display of the frame on the specified draw layer.
    // Frame:DoesClipChildren() : clipsChildren
    // Frame:EnableDrawLayer(layer) - Allows display of the frame on the specified draw layer.
    // Frame:EnableGamePadButton([enable]) - Allows the receipt of gamepad button inputs for this frame.
    // Frame:EnableGamePadStick([enable]) - Allows the receipt of gamepad stick inputs for this frame.
    // Frame:EnableKeyboard([enable]) - Allows this frame to receive keyboard input.
    // Frame:ExecuteAttribute(attributeName, unpackedPrimitiveType, ...) : success, unpackedPrimitiveType, ...
    // Frame:GetAttribute(attributeName) : value - Returns the value of a secure frame attribute.
    // Frame:GetBoundsRect() : left, bottom, width, height - Returns the calculated bounding box of the frame and all of its descendant regions.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_GetChildren
    /// Frame:GetChildren() : child1, ... - Returns a list of child frames belonging to the frame.
    /// </summary>
    /// <returns></returns>
    private List<Frame> GetChildren()
    {
        return this._children!;
    }
    private int internal_GetChildren(lua_State L)
    {
        var frame = GetThis(L, 1) as Frame;
        var children = frame?.GetChildren(); // e.g. List<Frame>
        if (children == null || children.Count == 0)
        {
            return 0; // Return no values
        }

        int count = 0;
        foreach (var child in children)
        {
            // child -> push
            LuaHelpers.PushExistingFrameToLua(L, child);
            count++;
        }
        return count;
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
    private int GetFrameLevel()
    {
        return 1;
    }
    private int internal_GetFrameLevel(lua_State L)
    {
        var frame = GetThis(L, 1) as Frame;
        var level = frame?.GetFrameLevel() ?? 0;

        lua_pushnumber(L, level);
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
    private bool RegisterEvent(string eventName)
    {
        if (string.IsNullOrWhiteSpace(eventName))
            throw new ArgumentException("Event name cannot be null or empty.", nameof(eventName));

        if (!_registeredEvents.Add(eventName)) return false; // Already registered

        return true;
    }
    private int internal_RegisterEvent(lua_State L)
    {
        lock (API.API._luaLock)
        {
            try
            {
                // Ensure there are exactly 2 arguments: frame, eventName
                var argc = lua_gettop(L);
                if (argc != 2)
                {
                    Log.ErrorL(L, "RegisterEvent requires exactly 2 arguments: frame, eventName.");
                    return 0; // Unreachable
                }

                // Retrieve the Frame object from Lua
                var frame = GetThis(L, 1) as Frame;
                if (frame == null)
                {
                    Log.ErrorL(L, "RegisterEvent: Invalid Frame object.");
                    return 0; // Unreachable
                }

                // Retrieve the eventName
                if (!LuaHelpers.IsString(L, 2))
                {
                    Log.ErrorL(L, "RegisterEvent: 'eventName' must be a string.");
                    return 0; // Unreachable
                }

                var eventName = lua_tostring(L, 2);

                // Register the event on the Frame
                var success = frame.RegisterEvent(eventName);

                if (success)
                {
                    // Update the event-to-frames mapping
                    lock (API.API._eventLock)
                    {
                        if (!API.UIObjects._eventToFrames.ContainsKey(eventName))
                            API.UIObjects._eventToFrames[eventName] = [];

                        API.UIObjects._eventToFrames[eventName].Add(frame);
                    }

                    Log.EventRegister(eventName, frame);
                }
                else
                {
                    Log.Warn($"[ref]Event [yellow]'{eventName}'[/] already registered for frame {frame}[/].");
                }

                // Push the success status to Lua
                lua_pushboolean(L, success ? 1 : 0);
                return 1;
            }
            catch (Exception ex)
            {
                Log.ErrorL(L, "RegisterEvent encountered an error.");
                return 0; // Unreachable
            }
        }
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_RegisterForDrag
    /// Frame:RegisterForDrag([button1, ...]) - Registers the frame for dragging with a mouse button.
    /// </summary>
    /// <param name="buttons"></param>
    private void RegisterForDrag(params string[] buttons)
    {
        foreach (var button in buttons)
        {
            if (!string.IsNullOrEmpty(button))
            {
                _registeredForDragButtons.Add(button);
            }
        }
    }
    private int internal_RegisterForDrag(lua_State L)
    {
        // Retrieve the Frame object (assuming the first argument is the Frame userdata)
        var frame = GetThis(L, 1) as Frame;
        if (frame == null)
        {
            lua_pushboolean(L, 0); // Push false to indicate failure
            return 1;
        }

        // Retrieve the number of arguments passed to the Lua function
        var argc = lua_gettop(L);

        // Collect all arguments starting from the second one
        List<string> buttons = [];
        for (var i = 2; i < argc; i++)
        {
            // Check if the argument is a string
            if (lua_isstring(L, i) != 0)
            {
                var button = lua_tostring(L, i);
                if (button != null) buttons.Add(button);
            }
            else
            {
                Log.ErrorL(L, $"Argument {i} is not a valid string.");
            }
        }

        // Register the collected buttons for drag
        frame.RegisterForDrag(buttons.ToArray());
        
        return 0;
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
    private void SetClampedToScreen(bool clamped)
    {
    }
    private int internal_SetClampedToScreen(lua_State L)
    {
        var frame = GetThis(L, 1) as Frame;

        var argc = lua_gettop(L);
        if (argc < 2)
        {
            Log.ErrorL(L, "SetClampedToScreen requires exactly 1 argument: clamped.");
            return 0; // Unreachable
        }

        var clamped = lua_toboolean(L, 2) != 0;
        frame?.SetClampedToScreen(clamped);

        return 0;
    }
    
    // Frame:SetClampRectInsets(left, right, top, bottom) - Controls how much of the frame may be moved off-screen.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_SetClipsChildren
    /// Frame:SetClipsChildren(clipsChildren)
    /// </summary>
    /// <param name="clips"></param>
    private void SetClipsChildren(bool clips)
    {
        
    }
    private int internal_SetClipsChildren(lua_State L)
    {
        var frame = GetThis(L, 1) as Frame;
        var clips = lua_toboolean(L, 2) != 0;

        frame?.SetClipsChildren(clips);

        return 0;
    }
    
    // Frame:SetDontSavePosition(dontSave)
    // Frame:SetDrawLayerEnabled(layer [, isEnabled])
    // Frame:SetFixedFrameLevel(isFixed)
    // Frame:SetFixedFrameStrata(isFixed)
    // Frame:SetFlattensRenderLayers(flatten) - Controls whether all subregions are composited into a single render layer.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_SetFrameLevel
    /// Frame:SetFrameLevel(frameLevel) - Sets the level at which the frame is layered relative to others in its strata.
    /// </summary>
    /// <param name="frameLevel"></param>
    private void SetFrameLevel(int frameLevel)
    {
        
    }
    private int internal_SetFrameLevel(lua_State L)
    {
        var frame = GetThis(L, 1) as Frame;
        var level = (int)lua_tonumber(L, 2);

        frame?.SetFrameLevel(level);

        return 0;
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
    public int internal_SetFrameStrata(lua_State L)
    {
        var frame = GetThis(L, 1) as Frame;

        var argc = lua_gettop(L);
        if (argc < 2)
        {
            Log.ErrorL(L, $"SetFrameStrata requires exactly 1 argument: strata. Got {argc - 1}.");
            return 0; // Unreachable
        }

        var strata = lua_tostring(L, 2);
        frame?.SetFrameStrata(strata);

        lua_pushboolean(L, 1);
        return 1;
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
    private void SetMovable(bool movable)
    {
        _isMovable = movable;
    }

    private int internal_SetMovable(lua_State L)
    {
        var frame = GetThis(L, 1) as Frame;

        var argc = lua_gettop(L);
        if (argc < 2)
        {
            Log.ErrorL(L, "SetMovable requires exactly 1 argument: movable.");
            return 0; // Unreachable
        }

        var movable = lua_toboolean(L, 2) != 0;
        frame?.SetMovable(movable);

        return 0;
    }
    
    // Frame:SetPropagateKeyboardInput(propagate) #nocombat - Sets whether keyboard input is consumed by this frame or propagates to further frames.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_SetResizable
    /// Frame:SetResizable(resizable) - Sets whether the frame can be resized by the user.
    /// </summary>
    /// <param name="resizable"></param>
    private void SetResizable(bool resizable)
    {
        
    }
    private int internal_SetResizable(lua_State L)
    {
        var frame = GetThis(L, 1) as Frame;
        var resizable = lua_toboolean(L, 2) != 0;

        frame?.SetResizable(resizable);

        return 0;
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_SetResizeBounds
    /// Frame:SetResizeBounds(minWidth, minHeight [, maxWidth, maxHeight]) - Sets the minimum and maximum size of the frame for user resizing.
    /// </summary>
    /// <param name="minWidth"></param>
    /// <param name="minHeight"></param>
    /// <param name="maxWidth"></param>
    /// <param name="maxHeight"></param>
    private void SetResizeBounds(float minWidth, float minHeight, float? maxWidth, float? maxHeight)
    {
        
    }
    public int internal_SetResizeBounds(lua_State L)
    {
        var frame = GetThis(L, 1) as Frame;

        var argc = lua_gettop(L);
        if (argc < 3)
        {
            Log.ErrorL(L, "SetResizeBounds requires at least 2 arguments: minWidth and minHeight.");
            return 0; // Unreachable
        }

        var minWidth = (float)lua_tonumber(L, 2);
        var minHeight = (float)lua_tonumber(L, 3);
        float? maxWidth = null;
        float? maxHeight = null;

        if (argc >= 4)
        {
            maxWidth = (float)lua_tonumber(L, 4);
        }

        if (argc >= 5)
        {
            maxHeight = (float)lua_tonumber(L, 5);
        }

        frame?.SetResizeBounds(minWidth, minHeight, maxWidth, maxHeight);

        lua_pushboolean(L, 1);
        return 1;
    }
    
    // Frame:SetToplevel(topLevel) #secureframe - Controls whether a frame should raise its frame level on mouse interaction.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_SetUserPlaced
    /// Frame:SetUserPlaced(userPlaced) - Sets whether a frame has been moved by the user and will be saved in the layout cache.
    /// </summary>
    /// <param name="placed"></param>
    private void SetUserPlaced(bool placed)
    {
        
    }
    private int internal_SetUserPlaced(lua_State L)
    {
        var frame = GetThis(L, 1) as Frame;
        var placed = lua_toboolean(L, 2) != 0;

        frame?.SetUserPlaced(placed);

        return 0;
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
    private void UnregisterAllEvents()
    {
        if (_registeredEvents.Count > 0) _registeredEvents.Clear();
        Log.UnregisterEvent("All", this);
    }
    public int internal_UnregisterAllEvents(lua_State L)
    {
        lock (API.API._luaLock)
        {
            try
            {
                // Ensure there is at least 1 argument: the Frame object
                var argc = lua_gettop(L);
                if (argc < 1)
                {
                    Log.ErrorL(L, "UnregisterAllEvents requires at least 1 argument: frame.");
                    return 0; // Unreachable
                }

                // Retrieve the Frame object from Lua
                var frame = GetThis(L, 1) as Frame;
                if (frame == null)
                {
                    Log.ErrorL(L, "UnregisterAllEvents: Invalid Frame object.");
                    return 0; // Unreachable
                }

                // Get the list of events the frame is registered for
                lock (_registeredEvents)
                {
                    List<string> registeredEvents = [.._registeredEvents];
                    
                    // Unregister all events from the Frame
                    frame.UnregisterAllEvents();

                    // Update the event-to-frames mapping
                    lock (API.API._eventLock)
                    {
                        foreach (var eventName in registeredEvents)
                        {
                            if (API.UIObjects._eventToFrames.ContainsKey(eventName))
                            {
                                API.UIObjects._eventToFrames[eventName].Remove(frame);
                                if (API.UIObjects._eventToFrames[eventName].Count == 0) API.UIObjects._eventToFrames.Remove(eventName);
                            }
                        }
                    }
                }

                //AnsiConsole.WriteLine($"Frame unregistered from all events.");
                // No return values
                return 0;
            }
            catch (Exception ex)
            {
                Log.ErrorL(L, "UnregisterAllEvents encountered an error.");
                return 0; // Unreachable
            }
        }
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
    private int internal_UnregisterEvent(lua_State L)
    {
        lock (API.API._luaLock)
        {
            try
            {
                // Ensure there are exactly 2 arguments: frame, eventName
                var argc = lua_gettop(L);
                if (argc != 2)
                {
                    Log.ErrorL(L, "UnregisterEvent requires exactly 2 arguments: frame, eventName.");
                    return 0; // Unreachable
                }

                // Retrieve the Frame object from Lua
                var frame = GetThis(L, 1) as Frame;
                if (frame == null)
                {
                    Log.ErrorL(L, "UnregisterEvent: Invalid Frame object.");
                    return 0; // Unreachable
                }

                // Retrieve the eventName
                if (!LuaHelpers.IsString(L, 2))
                {
                    Log.ErrorL(L, "UnregisterEvent: 'eventName' must be a string.");
                    return 0; // Unreachable
                }

                var eventName = lua_tostring(L, 2);

                // Unregister the event on the Frame
                var success = frame.UnregisterEvent(eventName);

                if (success)
                {
                    // Update the event-to-frames mapping
                    lock (API.API._eventLock)
                    {
                        if (eventName != null && UIObjects._eventToFrames.ContainsKey(eventName))
                        {
                            UIObjects._eventToFrames[eventName].Remove(frame);
                            if (UIObjects._eventToFrames[eventName].Count == 0) UIObjects._eventToFrames.Remove(eventName);
                        }
                    }

                    //AnsiConsole.WriteLine($"Frame unregistered for event '{eventName}'.");
                }
                else
                {
                    Log.Warn($"Frame was not registered for event '{eventName}'.");
                }

                // Push the success status to Lua
                lua_pushboolean(L, success ? 1 : 0);
                return 1;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                Log.ErrorL(L, "UnregisterEvent encountered an error.");
                return 0; // Unreachable
            }
        }
    }
    
    // ----------- Virtual Registration ---------------
    
    public override string GetMetatableName() => "FrameMetaTable";
        
    public override void RegisterMetaTable(lua_State L)
    {
        // 1) call base to register the "FrameMetaTable"
        base.RegisterMetaTable(L);

        // 2) Now define "FrameMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = FrameMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("RegionMetaTable")
        var baseMetaName = base.GetMetatableName();
        luaL_getmetatable(L, baseMetaName);
        lua_setmetatable(L, -2); // Sets FrameMetaTable's metatable to RegionMetaTable
        
        // 5) Bind Frame-specific methods
        LuaHelpers.RegisterMethod(L, "RegisterEvent", internal_RegisterEvent);
        LuaHelpers.RegisterMethod(L, "UnregisterAllEvents", internal_UnregisterAllEvents);
        LuaHelpers.RegisterMethod(L, "UnregisterEvent", internal_UnregisterEvent);
        LuaHelpers.RegisterMethod(L, "SetFrameStrata", internal_SetFrameStrata);
        LuaHelpers.RegisterMethod(L, "CreateTexture", internal_CreateTexture);
        LuaHelpers.RegisterMethod(L, "CreateFontString", internal_CreateFontString);
        LuaHelpers.RegisterMethod(L, "SetMovable", internal_SetMovable);
        LuaHelpers.RegisterMethod(L, "SetClampedToScreen", internal_SetClampedToScreen);
        LuaHelpers.RegisterMethod(L, "RegisterForDrag", internal_RegisterForDrag);
        LuaHelpers.RegisterMethod(L, "SetResizable", internal_SetResizable);
        LuaHelpers.RegisterMethod(L, "SetResizeBounds", internal_SetResizeBounds);
        LuaHelpers.RegisterMethod(L, "SetFrameLevel", internal_SetFrameLevel);
        LuaHelpers.RegisterMethod(L, "GetFrameLevel", internal_GetFrameLevel);
        LuaHelpers.RegisterMethod(L, "SetClipsChildren", internal_SetClipsChildren);
        LuaHelpers.RegisterMethod(L, "SetUserPlaced", internal_SetUserPlaced);
        LuaHelpers.RegisterMethod(L, "GetChildren", internal_GetChildren);
        LuaHelpers.RegisterMethod(L, "CreateLine", internal_CreateLine);

        // Optional __gc
        lua_pushcfunction(L, internal_ObjectGC);
        lua_setfield(L, -2, "__gc");

        // 6) pop
        lua_pop(L, 1);
    }
    
    public override int internal_ObjectGC(lua_State L)
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