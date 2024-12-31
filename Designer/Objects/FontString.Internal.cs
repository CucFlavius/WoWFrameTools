using System.Runtime.InteropServices;
using LuaNET.Lua51;
using Spectre.Console;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools;

public partial class FontString
{
    public static void RegisterMetatable(lua_State L)
    {
        // Create a new metatable for FontString objects
        luaL_newmetatable(L, "FontStringMetaTable"); // Register "FontStringMetaTable"

        // Set the __index field to the metatable itself to allow method access
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // Register SetFont method
        lua_pushcfunction(L, internal_SetFont);
        lua_setfield(L, -2, "SetFont");

        lua_pushcfunction(L, internal_SetPoint);
        lua_setfield(L, -2, "SetPoint");

        lua_pushcfunction(L, internal_SetText);
        lua_setfield(L, -2, "SetText");

        lua_pushcfunction(L, internal_SetJustifyV);
        lua_setfield(L, -2, "SetJustifyV");

        lua_pushcfunction(L, internal_SetJustifyH);
        lua_setfield(L, -2, "SetJustifyH");

        lua_pushcfunction(L, internal_SetAllPoints);
        lua_setfield(L, -2, "SetAllPoints");
        
        lua_pushcfunction(L, internal_ClearAllPoints);
        lua_setfield(L, -2, "ClearAllPoints");
        
        lua_pushcfunction(L, internal_Hide);
        lua_setfield(L, -2, "Hide");
        
        lua_pushcfunction(L, internal_GetStringWidth);
        lua_setfield(L, -2, "GetStringWidth");
        
        lua_pushcfunction(L, internal_SetSize);
        lua_setfield(L, -2, "SetSize");
        
        lua_pushcfunction(L, internal_SetTextColor);
        lua_setfield(L, -2, "SetTextColor");

        // Set __gc metamethod to handle garbage collection
        lua_pushcfunction(L, internal_FontStringGC);
        lua_setfield(L, -2, "__gc");

        // Pop the metatable from the stack
        lua_pop(L, 1);
    }

    public static FontString? GetFontString(lua_State L, int index)
    {
        // Ensure the object at the index has the correct metatable
        luaL_getmetatable(L, "FontStringMetaTable");
        lua_getmetatable(L, index);
        var hasMetatable = lua_rawequal(L, -1, -2) != 0;
        lua_pop(L, 2); // Pop both metatables from the stack

        if (!hasMetatable)
        {
            lua_pushstring(L, "Attempt to use a non-FontString object as a FontString.");
            lua_error(L);
            return null; // Unreachable
        }

        // Get the userdata pointer
        var userdataPtr = (IntPtr)lua_touserdata(L, index);
        if (userdataPtr == IntPtr.Zero)
        {
            lua_pushstring(L, "Invalid FontString userdata.");
            lua_error(L);
            return null; // Unreachable
        }

        // Read the IntPtr from the userdata memory
        var handlePtr = Marshal.ReadIntPtr(userdataPtr);
        if (handlePtr == IntPtr.Zero)
        {
            lua_pushstring(L, "FontString userdata contains a null GCHandle.");
            lua_error(L);
            return null; // Unreachable
        }

        var handle = GCHandle.FromIntPtr(handlePtr);
        if (!handle.IsAllocated)
        {
            lua_pushstring(L, "FontString has been garbage collected.");
            lua_error(L);
            return null; // Unreachable
        }

        var fontString = handle.Target as FontString;
        if (fontString == null)
        {
            luaL_error(L, "FontString userdata references an invalid object.");
            return null; // Unreachable
        }

        return fontString;
    }

    public static int internal_SetPoint(lua_State L)
    {
        var frame = GetFontString(L, 1);

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

    public static int internal_SetText(lua_State L)
    {
        try
        {
            var fontString = GetFontString(L, 1);
            if (fontString == null)
            {
                lua_pushstring(L, "SetText: Invalid FontString object.");
                lua_error(L);
                return 0; // Unreachable
            }

            var text = lua_tostring(L, 2) ?? "";

            fontString.SetText(text);
        }
        catch (Exception ex)
        {
            lua_pushstring(L, $"SetText: {ex.Message}");
            lua_error(L);
            return 0; // Unreachable
        }

        return 0; // No return values
    }

    /// <summary>
    ///     https://warcraft.wiki.gg/wiki/API_FontInstance_SetFont
    ///     success = FontInstance:SetFont(fontFile, height, flags)
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int internal_SetFont(lua_State L)
    {
        var fontString = GetFontString(L, 1);

        var argc = lua_gettop(L);
        if (argc != 4)
        {
            lua_pushstring(L, "SetFont requires exactly 3 arguments: fontFile, height, flags.");
            lua_error(L);
            return 0; // Unreachable
        }

        var fontFile = lua_tostring(L, 2);
        var height = (int)lua_tonumber(L, 3);
        var flags = lua_tostring(L, 4);

        var success = fontString?.SetFont(fontFile, height, flags);

        lua_pushboolean(L, success == true ? 1 : 0);
        return 1;
    }

    public static int internal_SetJustifyV(lua_State L)
    {
        try
        {
            var fontString = GetFontString(L, 1);
            if (fontString == null)
            {
                lua_pushstring(L, "SetJustifyV: Invalid FontString object.");
                lua_error(L);
                return 0; // Unreachable
            }

            var justify = lua_tostring(L, 2) ?? "MIDDLE";

            fontString.SetJustifyV(justify);
        }
        catch (Exception ex)
        {
            lua_pushstring(L, $"SetJustifyV: {ex.Message}");
            lua_error(L);
            return 0; // Unreachable
        }

        return 0; // No return values
    }

    public static int internal_SetJustifyH(lua_State L)
    {
        try
        {
            var fontString = GetFontString(L, 1);
            if (fontString == null)
            {
                lua_pushstring(L, "SetJustifyH: Invalid FontString object.");
                lua_error(L);
                return 0; // Unreachable
            }

            var justify = lua_tostring(L, 2) ?? "LEFT";

            fontString.SetJustifyH(justify);
        }
        catch (Exception ex)
        {
            lua_pushstring(L, $"SetJustifyH: {ex.Message}");
            lua_error(L);
            return 0; // Unreachable
        }

        return 0; // No return values
    }

    public static int internal_FontStringGC(lua_State L)
    {
        // Retrieve the userdata pointer
        var userdataPtr = (IntPtr)lua_touserdata(L, 1);
        if (userdataPtr == IntPtr.Zero) return 0;

        if (Frame._fontStringRegistry.ContainsKey(userdataPtr))
        {
            var fontString = Frame._fontStringRegistry[userdataPtr];

            // Unregister all events
            //fontString.UnregisterAllEvents();

            // Unreference all script handlers
            /*
            foreach (var refIndex in fontString._scriptRefs.Values)
            {
                luaL_unref(L, LUA_REGISTRYINDEX, refIndex);
            }
            */
            // Free the GCHandle
            var handlePtr = Marshal.ReadIntPtr(userdataPtr);
            var handle = GCHandle.FromIntPtr(handlePtr);

            if (handle.IsAllocated) handle.Free();

            // Remove the Frame from the registry
            Frame._fontStringRegistry.Remove(userdataPtr);
        }

        return 0;
    }

    public static int internal_SetAllPoints(lua_State L)
    {
        // Retrieve the frame from the first argument
        var fontString = GetFontString(L, 1);
        if (fontString == null)
        {
            lua_pushboolean(L, 0);
            return 1; // Frame not found, return false
        }

        // Retrieve the number of arguments
        var argc = lua_gettop(L);

        Frame? relativeToFrame = null;
        var doResize = false;

        // Check if the second argument is provided (relativeTo)
        if (argc >= 2)
            if (lua_isnil(L, 2) != 0)
            {
                // Retrieve the userdata pointer for the relativeTo frame
                var relativeToUserdataPtr = (IntPtr)lua_touserdata(L, 2);
                if (relativeToUserdataPtr != IntPtr.Zero && Frame._frameRegistry.TryGetValue(relativeToUserdataPtr, out var foundFrame))
                    relativeToFrame = foundFrame;
                else
                    throw new ArgumentException("Invalid relativeTo frame specified.");
            }

        // Check if the third argument is provided (doResize)
        if (argc >= 3) doResize = lua_toboolean(L, 3) != 0;

        // Call the SetAllPoints method with the resolved relativeTo frame
        fontString.SetAllPoints(relativeToFrame, doResize);

        lua_pushboolean(L, 1);
        return 1; // Return true
    }
    
    public static int internal_ClearAllPoints(lua_State L)
    {
        var fontString = GetFontString(L, 1);

        fontString?.ClearAllPoints();

        return 0;
    }
    
    public static int internal_Hide(lua_State L)
    {
        var fontString = GetFontString(L, 1);

        fontString?.Hide();

        return 0;
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_FontString_GetStringWidth
    /// stringWidth = fontString:GetStringWidth()
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int internal_GetStringWidth(lua_State L)
    {
        var fontString = GetFontString(L, 1);

        var width = fontString?.GetStringWidth() ?? 1.0f;

        lua_pushnumber(L, width);
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
                var frame = GetFontString(L, 1);
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
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_FontInstance_SetTextColor
    /// Font:SetTextColor(colorR, colorG, colorB [, a])
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int internal_SetTextColor(lua_State L)
    {
        var fontString = GetFontString(L, 1);

        var argc = lua_gettop(L);
        if (argc < 3)
        {
            lua_pushstring(L, "SetTextColor requires at least 3 arguments: colorR, colorG, colorB.");
            lua_error(L);
            return 0; // Unreachable
        }

        var colorR = (float)lua_tonumber(L, 2);
        var colorG = (float)lua_tonumber(L, 3);
        var colorB = (float)lua_tonumber(L, 4);
        var colorA = argc == 5 ? (float)lua_tonumber(L, 5) : 1.0f;

        fontString?.SetTextColor(colorR, colorG, colorB, colorA);

        lua_pushboolean(L, 1);
        return 1;
    }
}