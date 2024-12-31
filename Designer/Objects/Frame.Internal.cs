using System.Runtime.InteropServices;
using LuaNET.Lua51;
using Spectre.Console;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools;

public partial class Frame
{
    public static readonly object _eventLock = new();
    public static readonly Dictionary<IntPtr, Frame> _frameRegistry = new();
    public static readonly Dictionary<IntPtr, Texture> _textureRegistry = new();
    public static readonly Dictionary<IntPtr, FontString> _fontStringRegistry = new();
    public static readonly Dictionary<string, HashSet<Frame>> _eventToFrames = new();

    public static void RegisterMetaTable(lua_State L)
    {
        // Create a new metatable for Frame
        luaL_newmetatable(L, "FrameMetaTable");

        // Push the __index metamethod
        lua_pushstring(L, "__index");
        lua_newtable(L); // Create a table to hold the methods

        RegisterFrameMethod(L, "RegisterEvent", internal_RegisterEvent);
        RegisterFrameMethod(L, "SetScript", internal_SetScript);
        RegisterFrameMethod(L, "HookScript", internal_HookScript);
        RegisterFrameMethod(L, "TriggerEvent", internal_TriggerEvent);
        RegisterFrameMethod(L, "Show", internal_Show);
        RegisterFrameMethod(L, "Hide", internal_Hide);
        RegisterFrameMethod(L, "SetPoint", internal_SetPoint);
        RegisterFrameMethod(L, "SetAllPoints", internal_SetAllPoints);
        RegisterFrameMethod(L, "SetVertexOffset", internal_SetVertexOffset);
        RegisterFrameMethod(L, "SetFrameStrata", internal_SetFrameStrata);
        RegisterFrameMethod(L, "CreateTexture", internal_CreateTexture);
        RegisterFrameMethod(L, "CreateFontString", internal_CreateFontString);
        RegisterFrameMethod(L, "UnregisterEvent", internal_UnregisterEvent);
        RegisterFrameMethod(L, "UnregisterAllEvents", internal_UnregisterAllEvents);
        RegisterFrameMethod(L, "SetHeight", internal_SetHeight);
        RegisterFrameMethod(L, "SetWidth", internal_SetWidth);
        RegisterFrameMethod(L, "SetSize", internal_SetSize);
        RegisterFrameMethod(L, "SetMovable", internal_SetMovable);
        RegisterFrameMethod(L, "EnableMouse", internal_EnableMouse);
        RegisterFrameMethod(L, "SetClampedToScreen", internal_SetClampedToScreen);
        RegisterFrameMethod(L, "RegisterForDrag", internal_RegisterForDrag);
        RegisterFrameMethod(L, "SetNormalTexture", internal_SetNormalTexture);
        RegisterFrameMethod(L, "SetHighlightTexture", internal_SetHighlightTexture);
        RegisterFrameMethod(L, "SetPushedTexture", internal_SetPushedTexture);
        RegisterFrameMethod(L, "GetWidth", internal_GetWidth);

        // Set the __index table
        lua_settable(L, -3);

        // Register the __newindex metamethod
        //lua_pushstring(L, "__newindex");
        //lua_pushcfunction(L, internal_FrameNewIndex);
        //lua_settable(L, -3); // metatable.__newindex = internal_FrameNewIndex

        // Register the __tostring metamethod
        lua_pushstring(L, "__tostring");
        lua_pushcfunction(L, internal_FrameToString);
        lua_settable(L, -3); // metatable.__tostring = internal_FrameToString

        // Set the __gc metamethod to handle garbage collection if necessary
        lua_pushstring(L, "__gc");
        lua_pushcfunction(L, internal_FrameGC);
        lua_settable(L, -3);

        // Pop the metatable from the stack
        lua_pop(L, 1);
    }

    private static void RegisterFrameMethod(lua_State L, string methodName, lua_CFunction function)
    {
        lua_pushstring(L, methodName);
        lua_pushcfunction(L, function);
        lua_settable(L, -3); // metatable.__index[methodName] = function
    }

    public static Frame? GetFrame(lua_State L, int index)
    {
        if (lua_isuserdata(L, index) == 0)
            return null;

        var userdataPtr = (IntPtr)lua_touserdata(L, index);
        if (_frameRegistry.TryGetValue(userdataPtr, out var frame)) return frame;

        return null;
    }

    // RegisterEvent method callable from Lua
    public static int internal_RegisterEvent(lua_State L)
    {
        lock (Global._luaLock)
        {
            try
            {
                // Ensure there are exactly 2 arguments: frame, eventName
                var argc = lua_gettop(L);
                if (argc != 2)
                {
                    lua_pushstring(L, "RegisterEvent requires exactly 2 arguments: frame, eventName.");
                    lua_error(L);
                    return 0; // Unreachable
                }

                // Retrieve the Frame object from Lua
                var frame = GetFrame(L, 1);
                if (frame == null)
                {
                    lua_pushstring(L, "RegisterEvent: Invalid Frame object.");
                    lua_error(L);
                    return 0; // Unreachable
                }

                // Retrieve the eventName
                if (!LuaHelpers.IsString(L, 2))
                {
                    lua_pushstring(L, "RegisterEvent: 'eventName' must be a string.");
                    lua_error(L);
                    return 0; // Unreachable
                }

                var eventName = lua_tostring(L, 2);

                // Register the event on the Frame
                var success = frame.RegisterEvent(eventName);

                if (success)
                {
                    // Update the event-to-frames mapping
                    lock (_eventLock)
                    {
                        if (!_eventToFrames.ContainsKey(eventName)) _eventToFrames[eventName] = new HashSet<Frame>();

                        _eventToFrames[eventName].Add(frame);
                    }

                    Log.EventRegister(eventName, frame);
                }
                else
                {
                    AnsiConsole.WriteLine($"[ref]Event [yellow]'{eventName}'[/] already registered for frame {frame}[/].");
                }

                // Push the success status to Lua
                lua_pushboolean(L, success ? 1 : 0);
                return 1;
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                lua_pushstring(L, "RegisterEvent encountered an error.");
                lua_error(L);
                return 0; // Unreachable
            }
        }
    }

    // SetScript method callable from Lua
    public static int internal_SetScript(lua_State L)
    {
        // Ensure there are at least 2 arguments: Frame and scriptTypeName
        var argc = lua_gettop(L);
        if (argc < 2)
        {
            lua_pushstring(L, "SetScript requires at least 2 arguments: frame and scriptTypeName.");
            lua_error(L);
            return 0; // Unreachable
        }

        // Validate that the second argument is a string
        if (!LuaHelpers.IsString(L, 2))
        {
            lua_pushstring(L, "SetScript: 'scriptTypeName' must be a string.");
            lua_error(L);
            return 0; // Unreachable
        }

        // Now safely retrieve the scriptTypeName
        var scriptTypeName = lua_tostring(L, 2);

        // Check if the third argument is a function or nil
        var isFunction = lua_isfunction(L, 3) != 0;
        var isNil = lua_isnil(L, 3) != 0;

        if (isFunction || isNil)
        {
            if (isFunction)
            {
                // Create a reference to the Lua function
                lua_pushvalue(L, 3); // Push the function to the top of the stack
                var refIndex = luaL_ref(L, LUA_REGISTRYINDEX);

                // Retrieve the Frame object
                var frame = GetFrame(L, 1);

                // Assign the script handler with proper 'self' and 'event' parameters
                frame.SetScript(scriptTypeName, (f, eventName, extraParam) =>
                {
                    lock (Global._luaLock) // Ensure thread-safe access to Lua state
                    {
                        try
                        {
                            //AnsiConsole.MarkupLine($"[yellow]Handling event '{eventName}' with param '{extraParam}' for frame {f}[/]");

                            // Retrieve the Lua function from the registry
                            lua_rawgeti(L, LUA_REGISTRYINDEX, refIndex);

                            // Retrieve the userdata from the registry using LuaRegistryRef
                            lua_rawgeti(L, LUA_REGISTRYINDEX, f.LuaRegistryRef); // Push 'self' userdata

                            // Push 'eventName'
                            lua_pushstring(L, eventName); // Push 'event'

                            // Push 'extraParam' if present
                            if (!string.IsNullOrEmpty(extraParam)) lua_pushstring(L, extraParam);

                            // Determine the number of arguments (2 or 3)
                            var args = string.IsNullOrEmpty(extraParam) ? 2 : 3;

                            // Log the stack state before pcall
                            var stackBefore = lua_gettop(L);
                            //AnsiConsole.MarkupLine($"[blue]Lua stack before pcall: {stackBefore}[/]");

                            // Call the Lua function with 'self', 'eventName', and optionally 'extraParam'
                            var status = lua_pcall(L, args, 0, 0);

                            // Log the stack state after pcall
                            var stackAfter = lua_gettop(L);
                            //AnsiConsole.MarkupLine($"[blue]Lua stack after pcall: {stackAfter}[/]");

                            if (status != 0)
                            {
                                // Retrieve and log the error message
                                var error = lua_tostring(L, -1);
                                AnsiConsole.MarkupLine($"[red]Error in event '{eventName}': {error}[/]");
                                lua_pop(L, 1);
                            }
                            //AnsiConsole.MarkupLine($"[green]Successfully handled event '{eventName}'.[/]");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Exception while executing script '{scriptTypeName}': {ex.Message}");
                            throw;
                        }
                    }
                }, refIndex); // Pass the refIndex to manage references


                // Optionally, log the successful script assignment
                Log.ScriptSet(scriptTypeName, frame);
            }
            else if (isNil)
            {
                // Retrieve the Frame object
                var frame = GetFrame(L, 1);

                // Remove the script handler
                frame.SetScript(scriptTypeName, null);
                Log.RemoveScript(scriptTypeName, frame);
            }

            // Push 'true' to indicate success
            lua_pushboolean(L, 1);
            return 1;
        }

        lua_pushstring(L, "SetScript: Invalid arguments. Expected a function or nil as the third argument.");
        lua_error(L);
        return 0; // Unreachable
    }

    /// <summary>
    ///     Hooks a script handler for the specified script type on the Frame.
    /// </summary>
    /// <param name="L">The Lua state.</param>
    /// <returns>Number of return values (1).</returns>
    public static int internal_HookScript(lua_State L)
    {
        lock (Global._luaLock)
        {
            try
            {
                // Ensure there are exactly 3 arguments: frame, scriptTypeName, handler
                var argc = lua_gettop(L);
                if (argc != 3)
                {
                    lua_pushstring(L, "HookScript requires exactly 3 arguments: frame, scriptTypeName, handler.");
                    lua_error(L);
                    return 0; // Unreachable
                }

                // Validate that the second argument is a string
                if (!LuaHelpers.IsString(L, 2))
                {
                    lua_pushstring(L, "HookScript: 'scriptTypeName' must be a string.");
                    lua_error(L);
                    return 0; // Unreachable
                }

                var scriptTypeName = lua_tostring(L, 2);

                // Validate that the third argument is a function
                if (lua_isfunction(L, 3) == 0)
                {
                    lua_pushstring(L, "HookScript: 'handler' must be a function.");
                    lua_error(L);
                    return 0; // Unreachable
                }

                // Retrieve the Frame object
                var frame = GetFrame(L, 1);
                if (frame == null)
                {
                    lua_pushstring(L, "HookScript: Invalid Frame object.");
                    lua_error(L);
                    return 0; // Unreachable
                }

                // Create a reference to the Lua function
                lua_pushvalue(L, 3); // Push the function to the top of the stack
                var refIndex = luaL_ref(L, LUA_REGISTRYINDEX);

                // Assign the script handler
                frame.HookScript(scriptTypeName, (f, eventName, extraParam) =>
                {
                    lock (Global._luaLock) // Ensure thread-safe access to Lua state
                    {
                        try
                        {
                            // Retrieve the Lua function from the registry
                            lua_rawgeti(L, LUA_REGISTRYINDEX, refIndex);

                            // Retrieve the userdata from the registry using LuaRegistryRef
                            lua_rawgeti(L, LUA_REGISTRYINDEX, f.LuaRegistryRef); // Push 'self' userdata

                            // Push 'eventName'
                            lua_pushstring(L, eventName); // Push 'event'

                            // Call the Lua function with 2 arguments, 0 results
                            if (lua_pcall(L, 2, 0, 0) != 0)
                            {
                                // Retrieve and log the error message
                                var error = lua_tostring(L, -1);
                                Console.WriteLine($"Lua Error in HookScript '{scriptTypeName}': {error}");
                                lua_pop(L, 1); // Remove error message
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Exception while executing HookScript '{scriptTypeName}': {ex.Message}");
                        }
                    }
                }, refIndex); // Pass the refIndex to manage references

                // Push 'true' to indicate success
                lua_pushboolean(L, 1);
                return 1;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Exception in HookScript: {ex.Message}");
                lua_pushstring(L, "HookScript encountered an error.");
                lua_error(L);
                return 0; // Unreachable
            }
        }
    }

    // TriggerEvent method callable from Lua (optional, for testing)
    public static int internal_TriggerEvent(lua_State L)
    {
        var frame = GetFrame(L, 1);
        var eventName = lua_tostring(L, 2);
        string? param = null;
        if (lua_gettop(L) >= 3) param = lua_tostring(L, 3);

        if (param != null)
            AnsiConsole.MarkupLine($"Triggering event: [yellow]'{eventName}'[/] with param: [blue]{param} for frame {frame}[/]");
        else
            AnsiConsole.MarkupLine($"Triggering event: [yellow]'{eventName}'[/] for frame {frame}");

        frame.TriggerEvent(eventName, param);
        lua_pushboolean(L, 1);
        return 1;
    }

    // Show method callable from Lua
    public static int internal_Show(lua_State L)
    {
        var frame = GetFrame(L, 1);
        frame.Show();
        lua_pushboolean(L, 1);
        return 1;
    }

    // Hide method callable from Lua
    public static int internal_Hide(lua_State L)
    {
        var frame = GetFrame(L, 1);
        frame.Hide();
        lua_pushboolean(L, 1);
        return 1;
    }

    public static int internal_SetPoint(lua_State L)
    {
        var frame = GetFrame(L, 1);

        var argc = lua_gettop(L);
        if (argc < 2)
        {
            lua_pushstring(L, "SetPoint requires at least 1 argument: point.");
            lua_error(L);
            return 0; // Unreachable
        }

        var point = lua_tostring(L, 2);
        string? relativeTo = null;
        string? relativePoint = null;
        float offsetX = 0;
        float offsetY = 0;


        if (argc >= 3) relativeTo = lua_tostring(L, 3);
        if (argc >= 4) relativePoint = lua_tostring(L, 4);

        if (argc == 5) AnsiConsole.MarkupLine("Offset requires both X and Y values.");
        if (argc == 6)
        {
            offsetX = (float)lua_tonumber(L, 5);
            offsetY = (float)lua_tonumber(L, 6);
        }

        frame?.SetPoint(point, relativeTo, relativePoint, offsetX, offsetY);

        lua_pushboolean(L, 1);
        return 1;
    }

    private static int internal_SetHeight(lua_State L)
    {
        var frame = GetFrame(L, 1);

        var argc = lua_gettop(L);
        if (argc != 2)
        {
            lua_pushstring(L, "SetHeight requires exactly 1 argument: height.");
            lua_error(L);
            return 0; // Unreachable
        }

        var height = (float)lua_tonumber(L, 2);

        frame?.SetHeight(height);

        lua_pushboolean(L, 1);
        return 1;
    }

    private static int internal_SetWidth(lua_State L)
    {
        var frame = GetFrame(L, 1);

        var argc = lua_gettop(L);
        if (argc != 2)
        {
            lua_pushstring(L, "SetWidth requires exactly 1 argument: width.");
            lua_error(L);
            return 0; // Unreachable
        }

        var width = (float)lua_tonumber(L, 2);

        frame?.SetWidth(width);

        lua_pushboolean(L, 1);
        return 1;
    }

    private static int internal_UnregisterEvent(lua_State L)
    {
        lock (Global._luaLock)
        {
            try
            {
                // Ensure there are exactly 2 arguments: frame, eventName
                var argc = lua_gettop(L);
                if (argc != 2)
                {
                    lua_pushstring(L, "UnregisterEvent requires exactly 2 arguments: frame, eventName.");
                    lua_error(L);
                    return 0; // Unreachable
                }

                // Retrieve the Frame object from Lua
                var frame = GetFrame(L, 1);
                if (frame == null)
                {
                    lua_pushstring(L, "UnregisterEvent: Invalid Frame object.");
                    lua_error(L);
                    return 0; // Unreachable
                }

                // Retrieve the eventName
                if (!LuaHelpers.IsString(L, 2))
                {
                    lua_pushstring(L, "UnregisterEvent: 'eventName' must be a string.");
                    lua_error(L);
                    return 0; // Unreachable
                }

                var eventName = lua_tostring(L, 2);

                // Unregister the event on the Frame
                var success = frame.UnregisterEvent(eventName);

                if (success)
                {
                    // Update the event-to-frames mapping
                    lock (_eventLock)
                    {
                        if (eventName != null && _eventToFrames.ContainsKey(eventName))
                        {
                            _eventToFrames[eventName].Remove(frame);
                            if (_eventToFrames[eventName].Count == 0) _eventToFrames.Remove(eventName);
                        }
                    }

                    //AnsiConsole.WriteLine($"Frame unregistered for event '{eventName}'.");
                }
                else
                {
                    AnsiConsole.WriteLine($"Frame was not registered for event '{eventName}'.");
                }

                // Push the success status to Lua
                lua_pushboolean(L, success ? 1 : 0);
                return 1;
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                lua_pushstring(L, "UnregisterEvent encountered an error.");
                lua_error(L);
                return 0; // Unreachable
            }
        }
    }

    // UnregisterAllEvents method callable from Lua
    public static int internal_UnregisterAllEvents(lua_State L)
    {
        lock (Global._luaLock)
        {
            try
            {
                // Ensure there is at least 1 argument: the Frame object
                var argc = lua_gettop(L);
                if (argc < 1)
                {
                    lua_pushstring(L, "UnregisterAllEvents requires at least 1 argument: frame.");
                    lua_error(L);
                    return 0; // Unreachable
                }

                // Retrieve the Frame object from Lua
                var frame = GetFrame(L, 1);
                if (frame == null)
                {
                    lua_pushstring(L, "UnregisterAllEvents: Invalid Frame object.");
                    lua_error(L);
                    return 0; // Unreachable
                }

                // Get the list of events the frame is registered for
                var registeredEvents = frame.GetRegisteredEvents();

                // Unregister all events from the Frame
                frame.UnregisterAllEvents();

                // Update the event-to-frames mapping
                lock (_eventLock)
                {
                    foreach (var eventName in registeredEvents)
                        if (_eventToFrames.ContainsKey(eventName))
                        {
                            _eventToFrames[eventName].Remove(frame);
                            if (_eventToFrames[eventName].Count == 0) _eventToFrames.Remove(eventName);
                        }
                }

                //AnsiConsole.WriteLine($"Frame unregistered from all events.");

                // No return values
                return 0;
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                lua_pushstring(L, "UnregisterAllEvents encountered an error.");
                lua_error(L);
                return 0; // Unreachable
            }
        }
    }

    public static int internal_FrameGC(lua_State L)
    {
        // Retrieve the userdata pointer
        var userdataPtr = (IntPtr)lua_touserdata(L, 1);

        if (_frameRegistry.TryGetValue(userdataPtr, out var frame))
        {
            // Free the GCHandle
            if (frame.Handle.IsAllocated) frame.Handle.Free();

            // Remove from registry
            _frameRegistry.Remove(userdataPtr);

            // Perform any additional cleanup if necessary
        }

        return 0;
    }

    /// <summary>
    ///     https://warcraft.wiki.gg/wiki/API_ScriptRegionResizing_SetAllPoints
    ///     ScriptRegionResizing:SetAllPoints([relativeTo, doResize])
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int internal_SetAllPoints(lua_State L)
    {
        var frame = GetFrame(L, 1);
        if (frame == null)
        {
            lua_pushboolean(L, 0);
            return 1;
        }

        // Retrieve arguments: relativeTo (optional), doResize (optional)
        Frame? relativeTo = null;
        var doResize = false;

        if (lua_gettop(L) >= 2 && lua_isnil(L, 2) != 0)
        {
            var relativeToPtr = (IntPtr)lua_touserdata(L, 2);
            if (relativeToPtr != IntPtr.Zero && _frameRegistry.TryGetValue(relativeToPtr, out var foundFrame))
                relativeTo = foundFrame;
            else
                throw new ArgumentException("Invalid relativeTo frame specified.");
        }

        if (lua_gettop(L) >= 3) doResize = lua_toboolean(L, 3) != 0;

        // Set all points relative to another frame
        frame.SetAllPoints(relativeTo, doResize);

        lua_pushboolean(L, 1);
        return 1;
    }

    /// <summary>
    ///     https://warcraft.wiki.gg/wiki/API_TextureBase_SetVertexOffset
    ///     TextureBase:SetVertexOffset(vertexIndex, offsetX, offsetY)
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int internal_SetVertexOffset(lua_State L)
    {
        var frame = GetFrame(L, 1);

        var argc = lua_gettop(L);
        if (argc != 4)
        {
            lua_pushstring(L, "SetVertexOffset requires exactly 3 arguments: vertexIndex, offsetX, offsetY.");
            lua_error(L);
            return 0; // Unreachable
        }

        var vertexIndex = (int)lua_tonumber(L, 2);
        var offsetX = (float)lua_tonumber(L, 3);
        var offsetY = (float)lua_tonumber(L, 4);

        frame?.SetVertexOffset(vertexIndex, offsetX, offsetY);

        lua_pushboolean(L, 1);
        return 1;
    }

    /// <summary>
    ///     https://warcraft.wiki.gg/wiki/API_Frame_SetFrameStrata
    ///     Frame:SetFrameStrata(strata)
    ///     string : FrameStrata - Sets the layering strata of the frame.
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int internal_SetFrameStrata(lua_State L)
    {
        var frame = GetFrame(L, 1);

        var argc = lua_gettop(L);
        if (argc != 2)
        {
            lua_pushstring(L, "SetFrameStrata requires exactly 1 argument: strata.");
            lua_error(L);
            return 0; // Unreachable
        }

        var strata = lua_tostring(L, 2);
        frame?.SetFrameStrata(strata);

        lua_pushboolean(L, 1);
        return 1;
    }

    /// <summary>
    ///     https://warcraft.wiki.gg/wiki/API_Frame_CreateTexture
    ///     texture = Frame:CreateTexture([name, drawLayer, templateName, subLevel])
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int internal_CreateTexture(lua_State L)
    {
        // Retrieve the frame from the first argument
        var frame = GetFrame(L, 1);
        if (frame == null)
        {
            lua_pushnil(L);
            return 1;
        }

        // Retrieve arguments: name, layer
        var textureName = lua_tostring(L, 2) ?? "Texture";
        var drawLayer = lua_tostring(L, 3) ?? "BACKGROUND";
        string? templateName = null;
        var subLevel = 0;

        if (lua_gettop(L) >= 4) templateName = lua_tostring(L, 4);
        if (lua_gettop(L) >= 5) subLevel = (int)lua_tonumber(L, 5);

        // Create the texture
        var texture = frame.CreateTexture(textureName, drawLayer, templateName, subLevel);

        // Push the texture as userdata
        var handle = GCHandle.Alloc(texture);
        var handlePtr = GCHandle.ToIntPtr(handle);

        var userdataPtr = (IntPtr)lua_newuserdata(L, (UIntPtr)IntPtr.Size);
        Marshal.WriteIntPtr(userdataPtr, handlePtr);

        // Set the metatable for the texture userdata
        luaL_getmetatable(L, "TextureMetaTable"); // Ensure TextureMetaTable is defined
        lua_setmetatable(L, -2);

        // Register the texture in a registry if needed
        _textureRegistry[userdataPtr] = texture;

        return 1; // Return the texture userdata
    }


    /// <summary>
    ///     https://warcraft.wiki.gg/wiki/API_Frame_CreateFontString
    ///     line = Frame:CreateFontString([name, drawLayer, templateName])
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int internal_CreateFontString(lua_State L)
    {
        var frame = GetFrame(L, 1);

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
        _fontStringRegistry[userdataPtr] = fontString;

        // Assign the userdataPtr and LuaRegistryRef to the Frame instance
        fontString.UserdataPtr = userdataPtr;

        // Create a reference to the userdata in the registry
        lua_pushvalue(L, -1); // Push the userdata
        var refIndex = luaL_ref(L, LUA_REGISTRYINDEX);
        fontString.LuaRegistryRef = refIndex;

        Log.CreateFontString(fontString);

        //lua_pushboolean(L, 1);
        return 1;
    }

    /// <summary>
    ///     Sets the size of the Frame.
    /// </summary>
    /// <param name="L">The Lua state.</param>
    /// <returns>Number of return values (0).</returns>
    public static int internal_SetSize(lua_State L)
    {
        lock (Global._luaLock)
        {
            try
            {
                // Ensure there are exactly 3 arguments: frame, width, height
                var argc = lua_gettop(L);
                if (argc != 3)
                {
                    lua_pushstring(L, "SetSize requires exactly 3 arguments: frame, width, height.");
                    lua_error(L);
                    return 0; // Unreachable
                }

                // Retrieve the Frame object
                var frame = GetFrame(L, 1);
                if (frame == null)
                {
                    lua_pushstring(L, "SetSize: Invalid Frame object.");
                    lua_error(L);
                    return 0; // Unreachable
                }

                // Retrieve width and height
                if (!LuaHelpers.IsNumber(L, 2) || !LuaHelpers.IsNumber(L, 3))
                {
                    lua_pushstring(L, "SetSize: 'width' and 'height' must be numbers.");
                    lua_error(L);
                    return 0; // Unreachable
                }

                var width = (float)lua_tonumber(L, 2);
                var height = (float)lua_tonumber(L, 3);

                // Set the size
                frame.SetSize(width, height);

                // Log the action
                //AnsiConsole.WriteLine($"SetSize called on Frame. Width: {width}, Height: {height}");

                // No return values
                return 0;
            }
            catch (Exception ex)
            {
                // Log the exception
                AnsiConsole.WriteException(ex);

                // Raise a Lua error
                lua_pushstring(L, "SetSize encountered an error.");
                lua_error(L);
                return 0; // Unreachable
            }
        }
    }

    public static int internal_SetMovable(lua_State L)
    {
        var frame = GetFrame(L, 1);

        var argc = lua_gettop(L);
        if (argc != 2)
        {
            lua_pushstring(L, "SetMovable requires exactly 1 argument: movable.");
            lua_error(L);
            return 0; // Unreachable
        }

        var movable = lua_toboolean(L, 2) != 0;
        frame?.SetMovable(movable);

        return 0;
    }

    public static int internal_EnableMouse(lua_State L)
    {
        var frame = GetFrame(L, 1);

        var argc = lua_gettop(L);
        if (argc != 2)
        {
            lua_pushstring(L, "EnableMouse requires exactly 1 argument: enable.");
            lua_error(L);
            return 0; // Unreachable
        }

        var enable = lua_toboolean(L, 2) != 0;
        frame?.EnableMouse(enable);

        return 0;
    }

    public static int internal_SetClampedToScreen(lua_State L)
    {
        var frame = GetFrame(L, 1);

        var argc = lua_gettop(L);
        if (argc != 2)
        {
            lua_pushstring(L, "SetClampedToScreen requires exactly 1 argument: clamped.");
            lua_error(L);
            return 0; // Unreachable
        }

        var clamped = lua_toboolean(L, 2) != 0;
        frame?.SetClampedToScreen(clamped);

        return 0;
    }

    public static int internal_RegisterForDrag(lua_State L)
    {
        // Retrieve the Frame object (assuming the first argument is the Frame userdata)
        var frame = GetFrame(L, 1);
        if (frame == null)
        {
            lua_pushboolean(L, 0); // Push false to indicate failure
            return 1;
        }

        // Retrieve the number of arguments passed to the Lua function
        var argc = lua_gettop(L);

        // Collect all arguments starting from the second one
        List<string> buttons = new();
        for (var i = 2; i <= argc; i++)
            // Check if the argument is a string
            if (lua_isstring(L, i) != 0)
            {
                var button = lua_tostring(L, i);
                buttons.Add(button);
            }
            else
            {
                // Handle non-string arguments gracefully (optional)
                throw new ArgumentException($"Argument {i} is not a valid string.");
            }

        // Register the collected buttons for drag
        frame.RegisterForDrag(buttons.ToArray());

        lua_pushboolean(L, 1); // Push true to indicate success
        return 1;
    }

    public static int internal_SetNormalTexture(lua_State L)
    {
        var frame = GetFrame(L, 1);
        var texture = Texture.GetTexture(L, 2);

        frame?.SetNormalTexture(texture);

        return 0;
    }

    public static int internal_SetHighlightTexture(lua_State L)
    {
        var frame = GetFrame(L, 1);
        var texture = Texture.GetTexture(L, 2);

        frame?.SetHighlightTexture(texture);

        return 0;
    }

    public static int internal_SetPushedTexture(lua_State L)
    {
        var frame = GetFrame(L, 1);
        var texture = Texture.GetTexture(L, 2);

        frame?.SetPushedTexture(texture);

        return 0;
    }

    public static int internal_GetWidth(lua_State L)
    {
        var frame = GetFrame(L, 1);
        var width = frame?.GetWidth() ?? 0;

        lua_pushnumber(L, width);
        return 1;
    }

    public static int internal_FrameToString(lua_State L)
    {
        // Retrieve the userdata (Frame) from the first argument
        var frame = GetFrame(L, 1);
        if (frame == null)
        {
            lua_pushstring(L, "Frame: Invalid Frame");
            return 1;
        }

        // Construct a meaningful string representation
        var frameName = string.IsNullOrEmpty(frame._name) ? "Unnamed" : frame._name;
        var frameType = frame._frameType;
        var frameRegistryRef = frame.LuaRegistryRef;

        var result = $"Frame: Name='{frameName}', Type='{frameType}', Ref={frameRegistryRef}";

        // Push the string onto the Lua stack
        lua_pushstring(L, result);
        return 1; // Number of return values
    }

    public static int internal_FrameNewIndex(lua_State L)
    {
        // Ensure there are at least two arguments: userdata and key
        var argc = lua_gettop(L);
        if (argc < 2)
        {
            lua_pushstring(L, "Frame: newindex requires at least a key and a value");
            lua_error(L);
            return 0; // Not reached, lua_error throws
        }

        // Retrieve the Frame object from the first argument (userdata)
        var frame = GetFrame(L, 1);
        if (frame == null)
        {
            lua_pushstring(L, "Frame: Invalid Frame userdata");
            lua_error(L);
            return 0;
        }

        // Get the key (property name) from the second argument
        if (lua_isstring(L, 2) == 0)
        {
            lua_pushstring(L, "Frame: Property name must be a string");
            lua_error(L);
            return 0;
        }

        var propertyName = lua_tostring(L, 2);

        // Get the value from the third argument
        // Note: Lua allows setting with multiple arguments, but typically it's frame.key = value
        if (argc < 3)
        {
            lua_pushstring(L, "Frame: No value provided for property assignment");
            lua_error(L);
            return 0;
        }

        // Depending on your design, you might support different types
        // For simplicity, we'll assume the value is a basic type (string, number, boolean)
        object? value = null;

        if (lua_isstring(L, 3) != 0)
        {
            value = lua_tostring(L, 3);
        }
        else if (lua_isnumber(L, 3) != 0)
        {
            value = lua_tonumber(L, 3);
        }
        else if (lua_isboolean(L, 3) != 0)
        {
            value = lua_toboolean(L, 3) != 0;
        }
        else if (lua_isuserdata(L, 3) != 0)
        {
            // Handle userdata assignments if necessary
            // For example, setting a parent frame or a texture
            var userdataPtr = (IntPtr)lua_touserdata(L, 3);
            // You can retrieve the associated C# object if needed
            // Example: Frame relativeFrame = GetFrameFromUserdata(userdataPtr);
            // Assign relativeFrame to a property
            value = userdataPtr; // Or handle appropriately
        }
        else if (lua_isnil(L, 3) != 0)
        {
            value = null;
        }
        else
        {
            lua_pushstring(L, $"Frame: Unsupported value type for property '{propertyName}'");
            lua_error(L);
            return 0;
        }

        // Set the property on the Frame instance
        var success = frame.SetProperty(propertyName, value);

        if (!success)
        {
            //lua_pushstring(L, $"Frame: Unknown property '{propertyName}' or invalid value");
            //lua_error(L);
            //return 0;
        }

        return 0; // __newindex does not return any values
    }
}