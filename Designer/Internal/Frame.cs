using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Internal;

public static class Frame
{
    public static int internal_CreateFontString(lua_State L)
    {
        var frame = GetThis(L, 1);

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
        luaL_getmetatable(L, FontString.GetMetatableName());
        lua_setmetatable(L, -2);

        // Add the Frame to the registry for later retrieval
        API.UIObjects._fontStringRegistry[userdataPtr] = fontString;

        // Assign the userdataPtr and LuaRegistryRef to the Frame instance
        fontString.UserdataPtr = userdataPtr;

        // Create a reference to the userdata in the registry
        lua_pushvalue(L, -1); // Push the userdata
        var refIndex = luaL_ref(L, LUA_REGISTRYINDEX);
        fontString.LuaRegistryRef = refIndex;

        // 9. **Create a Lua table and embed the userdata**
        // Create a new Lua table
        lua_newtable(L); // Push a new table onto the stack

        // Set the Frame userdata in the table with a hidden key
        lua_pushstring(L, "__frame"); // Key
        lua_pushlightuserdata(L, (UIntPtr)userdataPtr); // Value (light userdata)
        lua_settable(L, -3); // table["__frame"] = userdata

        // Set the metatable for the table to handle method calls and property accesses
        luaL_getmetatable(L, FontString.GetMetatableName()); // Push the FrameMetaTable
        lua_setmetatable(L, -2); // setmetatable(table, "FrameMetaTable")
        
        Log.CreateFontString(fontString);
        
        return 1;
    }
        
    public static int internal_CreateLine(lua_State L)
    {
        // Retrieve the Frame object (assuming the first argument is the Frame userdata)
        var frame = GetThis(L, 1);
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
        var subLevel = 0;
        if (lua_gettop(L) >= 5) subLevel = (int)lua_tonumber(L, 5);

        // Create the line
        var line = frame.CreateLine(name, drawLayer, templateName, subLevel);
        
        // Allocate a GCHandle to prevent the Frame from being garbage collected
        var handle = GCHandle.Alloc(line);
        var handlePtr = GCHandle.ToIntPtr(handle);

        // Create userdata with the size of IntPtr
        var userdataPtr = (IntPtr)lua_newuserdata(L, (UIntPtr)IntPtr.Size);

        // Write the handlePtr into the userdata memory
        Marshal.WriteIntPtr(userdataPtr, handlePtr);

        // Set the metatable for the userdata
        luaL_getmetatable(L, Internal.Line.GetMetatableName());
        lua_setmetatable(L, -2);

        // Add the Frame to the registry for later retrieval
        API.UIObjects._lineRegistry[userdataPtr] = line;

        // Assign the userdataPtr and LuaRegistryRef to the Frame instance
        line.UserdataPtr = userdataPtr;
        
        // Create a reference to the userdata in the Lua registry
        lua_pushvalue(L, -1); // Push the userdata
        int refIndex = luaL_ref(L, LUA_REGISTRYINDEX);
        line.LuaRegistryRef = refIndex;

        // 9. **Create a Lua table and embed the userdata**
        // Create a new Lua table
        lua_newtable(L); // Push a new table onto the stack

        // Set the Frame userdata in the table with a hidden key
        lua_pushstring(L, "__frame"); // Key
        lua_pushlightuserdata(L, (UIntPtr)userdataPtr); // Value (light userdata)
        lua_settable(L, -3); // table["__frame"] = userdata

        // Set the metatable for the table to handle method calls and property accesses
        luaL_getmetatable(L, Internal.Line.GetMetatableName()); // Push the FrameMetaTable
        lua_setmetatable(L, -2); // setmetatable(table, "FrameMetaTable")
        
        Log.CreateLine(line);
        
        return 1;
    }
        
    public static int internal_CreateTexture(lua_State L)
    {
        var frame = GetThis(L, 1);
        
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
        luaL_getmetatable(L, Internal.Texture.GetMetatableName()); // Ensure TextureMetaTable is set up
        lua_setmetatable(L, -2);

        // Add the Frame to the registry for later retrieval
        API.UIObjects._textureRegistry[textureUserdataPtr] = texture;

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
        luaL_getmetatable(L, Internal.Texture.GetMetatableName()); // Push the TextureMetaTable
        lua_setmetatable(L, -2); // setmetatable(table, "TextureMetaTable")

        Log.CreateTexture(texture);

        return 1; // Return the table
    }
    
    public static int internal_EnableKeyboard(lua_State L)
    {
        var frame = GetThis(L, 1);
        var enable = lua_toboolean(L, 2) != 0;

        frame?.EnableKeyboard(enable);

        return 0;
    }
    
    public static int internal_GetChildren(lua_State L)
    {
        var frame = GetThis(L, 1);
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
    
    public static int internal_GetFrameLevel(lua_State L)
    {
        var frame = GetThis(L, 1);
        var level = frame?.GetFrameLevel() ?? 0;

        lua_pushnumber(L, level);
        return 1;
    }
    
    public static int internal_GetNumChildren(lua_State L)
    {
        var frame = GetThis(L, 1);
        var count = frame?.GetNumChildren() ?? 0;

        lua_pushnumber(L, count);
        return 1;
    }
    
    public static int internal_RegisterEvent(lua_State L)
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
            var frame = GetThis(L, 1);
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
                if (!API.UIObjects._eventToFrames.ContainsKey(eventName))
                    API.UIObjects._eventToFrames[eventName] = [];

                API.UIObjects._eventToFrames[eventName].Add(frame);

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
    
    public static int internal_RegisterForDrag(lua_State L)
    {
        // Retrieve the Frame object (assuming the first argument is the Frame userdata)
        var frame = GetThis(L, 1);
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
    
    public static int internal_SetClampedToScreen(lua_State L)
    {
        var frame = GetThis(L, 1);

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
    
    public static int internal_SetClipsChildren(lua_State L)
    {
        var frame = GetThis(L, 1);
        var clips = lua_toboolean(L, 2) != 0;

        frame?.SetClipsChildren(clips);

        return 0;
    }
    
    public static int internal_SetFixedFrameLevel(lua_State L)
    {
        var frame = GetThis(L, 1);

        var argc = lua_gettop(L);
        if (argc != 3)
        {
            Log.ErrorL(L, "SetFixedFrameLevel requires exactly 1 argument: isFixed.");
            return 0; // Unreachable
        }
        
        var isFixed = lua_toboolean(L, 2) != 0;
        
        frame?.SetFixedFrameLevel(isFixed);
        
        return 0;
    }
    
    public static int internal_SetFixedFrameStrata(lua_State L)
    {
        var frame = GetThis(L, 1);
        var isFixed = lua_toboolean(L, 2) != 0;

        frame?.SetFixedFrameStrata(isFixed);

        return 0;
    }
    
    public static int internal_SetFrameLevel(lua_State L)
    {
        var frame = GetThis(L, 1);
        var level = (int)lua_tonumber(L, 2);

        frame?.SetFrameLevel(level);

        return 0;
    }
    
    public static int internal_SetFrameStrata(lua_State L)
    {
        var frame = GetThis(L, 1);

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
    
    public static int internal_SetMovable(lua_State L)
    {
        var frame = GetThis(L, 1);

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
    
    public static int internal_SetPropagateKeyboardInput(lua_State L)
    {
        var frame = GetThis(L, 1);
        var propagate = lua_toboolean(L, 2) != 0;

        frame?.SetPropagateKeyboardInput(propagate);

        return 0;
    }
    
    public static int internal_SetResizable(lua_State L)
    {
        var frame = GetThis(L, 1);
        var resizable = lua_toboolean(L, 2) != 0;

        frame?.SetResizable(resizable);

        return 0;
    }
    
    public static int internal_SetResizeBounds(lua_State L)
    {
        var frame = GetThis(L, 1);

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
    
    public static int internal_SetUserPlaced(lua_State L)
    {
        var frame = GetThis(L, 1);
        var placed = lua_toboolean(L, 2) != 0;

        frame?.SetUserPlaced(placed);

        return 0;
    }
    
    public static int internal_UnregisterAllEvents(lua_State L)
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
            var frame = GetThis(L, 1);
            if (frame == null)
            {
                Log.ErrorL(L, "UnregisterAllEvents: Invalid Frame object.");
                return 0; // Unreachable
            }

            // Get the list of events the frame is registered for
            lock (frame._registeredEvents)
            {
                List<string> registeredEvents = [..frame._registeredEvents];
                
                // Unregister all events from the Frame
                frame.UnregisterAllEvents();

                // Update the event-to-frames mapping
                foreach (var eventName in registeredEvents)
                {
                    if (API.UIObjects._eventToFrames.ContainsKey(eventName))
                    {
                        API.UIObjects._eventToFrames[eventName].Remove(frame);
                        if (API.UIObjects._eventToFrames[eventName].Count == 0) API.UIObjects._eventToFrames.Remove(eventName);
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
    
    public static int internal_UnregisterEvent(lua_State L)
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
            var frame = GetThis(L, 1);
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
                if (eventName != null && API.UIObjects._eventToFrames.ContainsKey(eventName))
                {
                    API.UIObjects._eventToFrames[eventName].Remove(frame);
                    if (API.UIObjects._eventToFrames[eventName].Count == 0) API.UIObjects._eventToFrames.Remove(eventName);
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

    public static string GetMetatableName() => "FrameMetaTable";

    private static Widgets.Frame? GetThis(lua_State L, int index)
    {
        // 1) Check the correct metatable
        // var metaName = GetMetatableName();
        // luaL_getmetatable(L, metaName);
        // lua_getmetatable(L, index);
        // bool same = (lua_rawequal(L, -1, -2) != 0);
        // lua_pop(L, 2);
        //
        // if (!same)
        //     return null;

        // If it's a table, retrieve the __frame key
        if (lua_istable(L, index) != 0)
        {
            lua_pushstring(L, "__frame");
            lua_gettable(L, index); // Pushes table["__frame"]
            index = -1; // Update index to point to __frame value
        }

        IntPtr userdataPtr = (IntPtr)lua_touserdata(L, index);
        if (userdataPtr == IntPtr.Zero)
            return null;

        IntPtr handlePtr = Marshal.ReadIntPtr(userdataPtr);
        if (handlePtr == IntPtr.Zero)
            return null;

        var handle = GCHandle.FromIntPtr(handlePtr);
        if (!handle.IsAllocated)
            return null;

        return handle.Target as Widgets.Frame;
    }
    
    public static void RegisterMetaTable(lua_State L)
    {
        Region.RegisterMetaTable(L);
        
        // 2) Now define "FrameMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = FrameMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("RegionMetaTable")
        var baseMetaName = Region.GetMetatableName();
        luaL_getmetatable(L, baseMetaName);
        lua_setmetatable(L, -2); // Sets FrameMetaTable's metatable to RegionMetaTable
        
        // 5) Bind Frame-specific methods
        LuaHelpers.RegisterMethod(L, "RegisterEvent", Internal.Frame.internal_RegisterEvent);
        LuaHelpers.RegisterMethod(L, "UnregisterAllEvents", Internal.Frame.internal_UnregisterAllEvents);
        LuaHelpers.RegisterMethod(L, "UnregisterEvent", Internal.Frame.internal_UnregisterEvent);
        LuaHelpers.RegisterMethod(L, "SetFrameStrata", Internal.Frame.internal_SetFrameStrata);
        LuaHelpers.RegisterMethod(L, "CreateTexture", Internal.Frame.internal_CreateTexture);
        LuaHelpers.RegisterMethod(L, "CreateFontString", Internal.Frame.internal_CreateFontString);
        LuaHelpers.RegisterMethod(L, "SetMovable", Internal.Frame.internal_SetMovable);
        LuaHelpers.RegisterMethod(L, "SetClampedToScreen", Internal.Frame.internal_SetClampedToScreen);
        LuaHelpers.RegisterMethod(L, "RegisterForDrag", Internal.Frame.internal_RegisterForDrag);
        LuaHelpers.RegisterMethod(L, "SetResizable", Internal.Frame.internal_SetResizable);
        LuaHelpers.RegisterMethod(L, "SetResizeBounds", Internal.Frame.internal_SetResizeBounds);
        LuaHelpers.RegisterMethod(L, "SetFrameLevel", Internal.Frame.internal_SetFrameLevel);
        LuaHelpers.RegisterMethod(L, "GetFrameLevel", Internal.Frame.internal_GetFrameLevel);
        LuaHelpers.RegisterMethod(L, "GetNumChildren", Internal.Frame.internal_GetNumChildren);
        LuaHelpers.RegisterMethod(L, "SetClipsChildren", Internal.Frame.internal_SetClipsChildren);
        LuaHelpers.RegisterMethod(L, "SetUserPlaced", Internal.Frame.internal_SetUserPlaced);
        LuaHelpers.RegisterMethod(L, "GetChildren", Internal.Frame.internal_GetChildren);
        LuaHelpers.RegisterMethod(L, "CreateLine", Internal.Frame.internal_CreateLine);
        LuaHelpers.RegisterMethod(L, "SetFixedFrameStrata", Internal.Frame.internal_SetFixedFrameStrata);
        LuaHelpers.RegisterMethod(L, "SetFixedFrameLevel", Internal.Frame.internal_SetFixedFrameLevel);
        LuaHelpers.RegisterMethod(L, "EnableKeyboard", Internal.Frame.internal_EnableKeyboard);
        LuaHelpers.RegisterMethod(L, "SetPropagateKeyboardInput", Internal.Frame.internal_SetPropagateKeyboardInput);
        
        // 6) pop
        lua_pop(L, 1);
    }
}