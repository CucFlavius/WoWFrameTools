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
    public static readonly Dictionary<IntPtr, Line> _lineRegistry = new();
    public static readonly Dictionary<string, HashSet<Frame>> _eventToFrames = new();

    public static int internal_FrameToString(lua_State L)
    {
        try
        {
            // Stack:
            // 1 - table

            // Retrieve the table
            if (lua_istable(L, 1) == 0)
            {
                lua_pushstring(L, "Frame: Invalid Frame Table");
                return 1;
            }

            // Retrieve the Frame userdata from the table's __frame field
            lua_pushstring(L, "__frame");
            lua_gettable(L, 1); // table.__frame
            if (lua_islightuserdata(L, -1) == 0)
            {
                lua_pop(L, 1);
                lua_pushstring(L, "Frame: __frame field is missing or invalid");
                return 1;
            }

            IntPtr frameUserdataPtr = (IntPtr)lua_touserdata(L, -1);
            lua_pop(L, 1); // Remove __frame userdata from the stack

            // Retrieve the Frame instance
            if (!Frame._frameRegistry.TryGetValue(frameUserdataPtr, out var frame))
            {
                lua_pushstring(L, "Frame: Frame not found in registry");
                return 1;
            }

            // Construct a meaningful string representation
            string frameName = string.IsNullOrEmpty(frame._name) ? "Unnamed" : frame._name;
            string frameType = frame._frameType;
            int frameId = frame._id;

            string result = $"Frame: Name='{frameName}', Type='{frameType}', ID={frameId}";

            // Push the string onto the Lua stack
            lua_pushstring(L, result);
            return 1; // Number of return values
        }
        catch (Exception ex)
        {
            lua_pushstring(L, $"Frame: Error - {ex.Message}");
            return 1;
        }
    }
    
    public static int internal_FrameGC(lua_State L)
    {
        // Stack:
        // 1 - table

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
        if (Frame._frameRegistry.TryGetValue(frameUserdataPtr, out var frame))
        {
            // Free the GCHandle
            if (frame.Handle.IsAllocated)
            {
                frame.Handle.Free();
            }

            // Remove from registry
            Frame._frameRegistry.Remove(frameUserdataPtr);

            // Perform any additional cleanup if necessary
            // Example: frame.Dispose();
        }

        return 0;
    }
    
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
        RegisterFrameMethod(L, "SetResizable", internal_SetResizable);
        RegisterFrameMethod(L, "SetResizeBounds", internal_SetResizeBounds);
        RegisterFrameMethod(L, "SetVertexColor", internal_SetVertexColor);
        RegisterFrameMethod(L, "SetScale", internal_SetScale);
        RegisterFrameMethod(L, "ClearAllPoints", internal_ClearAllPoints);
        RegisterFrameMethod(L, "SetFrameLevel", internal_SetFrameLevel);
        RegisterFrameMethod(L, "GetEffectiveScale", internal_GetEffectiveScale);
        RegisterFrameMethod(L, "GetHeight", internal_GetHeight);
        RegisterFrameMethod(L, "GetFrameLevel", internal_GetFrameLevel);
        RegisterFrameMethod(L, "SetAlpha", internal_SetAlpha);
        RegisterFrameMethod(L, "SetClipsChildren", internal_SetClipsChildren);
        RegisterFrameMethod(L, "SetUserPlaced", internal_SetUserPlaced);
        RegisterFrameMethod(L, "RegisterForClicks", internal_RegisterForClicks);
        RegisterFrameMethod(L, "SetFont", internal_SetFont);
        RegisterFrameMethod(L, "SetText", internal_SetText);
        RegisterFrameMethod(L, "SetAutoFocus", internal_SetAutoFocus);
        RegisterFrameMethod(L, "SetMaxLetters", internal_SetMaxLetters);
        RegisterFrameMethod(L, "SetParent", internal_SetParent);
        RegisterFrameMethod(L, "GetChildren", internal_GetChildren);
        RegisterFrameMethod(L, "GetParent", internal_GetParent);
        RegisterFrameMethod(L, "CreateLine", internal_CreateLine);
        RegisterFrameMethod(L, "SetFixedFrameStrata", internal_SetFixedFrameStrata);
        RegisterFrameMethod(L, "SetFixedFrameLevel", internal_SetFixedFrameLevel);
        RegisterFrameMethod(L, "SetEnabled", internal_SetEnabled);
        
        
        // ModelScene
        RegisterFrameMethod(L, "SetCameraPosition", ModelScene.internal_SetCameraPosition);
        RegisterFrameMethod(L, "SetCameraOrientationByYawPitchRoll", ModelScene.internal_SetCameraOrientationByYawPitchRoll);
        RegisterFrameMethod(L, "SetLightAmbientColor", ModelScene.internal_SetLightAmbientColor);
        RegisterFrameMethod(L, "SetLightDiffuseColor", ModelScene.internal_SetLightDiffuseColor);
        RegisterFrameMethod(L, "SetLightVisible", ModelScene.internal_SetLightVisible);
        RegisterFrameMethod(L, "SetFogColor", ModelScene.internal_SetFogColor);
        RegisterFrameMethod(L, "SetFogFar", ModelScene.internal_SetFogFar);
        RegisterFrameMethod(L, "SetFogNear", ModelScene.internal_SetFogNear);
        RegisterFrameMethod(L, "ClearFog", ModelScene.internal_ClearFog);

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

    public static void RegisterFrameMethod(lua_State L, string methodName, lua_CFunction function)
    {
        lua_pushstring(L, methodName);
        lua_pushcfunction(L, function);
        lua_settable(L, -3); // metatable.__index[methodName] = function
    }

    public static Frame? GetFrame(lua_State L, int index)
    {
        if (lua_isuserdata(L, index) != 0)
        {
            IntPtr userdataPtr = (IntPtr)lua_touserdata(L, index);
            if (Frame._frameRegistry.TryGetValue(userdataPtr, out var frame))
            {
                return frame;
            }
        }
        else if (lua_istable(L, index) != 0)
        {
            // Assume the table has a '__frame' field containing the userdata
            lua_pushstring(L, "__frame");      // Push key '__frame'
            lua_gettable(L, index);             // Get table['__frame']
            if (lua_islightuserdata(L, -1) != 0)
            {
                IntPtr userdataPtr = (IntPtr)lua_touserdata(L, -1);
                lua_pop(L, 1);                   // Remove '__frame' value from stack
                if (Frame._frameRegistry.TryGetValue(userdataPtr, out var frame))
                {
                    return frame;
                }
            }
            else
            {
                lua_pop(L, 1);                   // Remove '__frame' value from stack
            }
        }

        return null; // Frame not found or invalid argument
    }
    
    public static int internal_SetParent(lua_State L)
    {
        var frame = GetFrame(L, 1);
        var parent = GetFrame(L, 2);

        frame?.SetParent(parent);

        return 0;
    }

    /// <summary>
    /// Pushes the existing Lua table (or userdata) that represents a Frame onto the stack.
    /// This is similar to what you'd do in internal_CreateFrame, but for an already existing frame.
    /// </summary>
    public static void PushExistingFrameToLua(lua_State L, Frame child)
    {
        lua_rawgeti(L, LUA_REGISTRYINDEX, child.LuaRegistryRef);
    }
    
    public static int internal_GetParent(lua_State L)
    {
        var frame = GetFrame(L, 1);
        var parent = frame?.GetParent();

        if (parent == null)
        {
            lua_pushnil(L);
            return 1;
        }

        // Push the parent's existing Lua object/table/userdata
        // so that Lua sees it as a proper "Frame". For example:
        PushExistingFrameToLua(L, parent);

        return 1;
    }

    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_SetFixedFrameStrata
    /// Frame:SetFixedFrameStrata(isFixed)
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int internal_SetFixedFrameStrata(lua_State L)
    {
        var frame = GetFrame(L, 1);
        var isFixed = lua_toboolean(L, 2) != 0;

        frame?.SetFixedFrameStrata(isFixed);

        return 0;
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Frame_SetFixedFrameLevel
    /// Frame:SetFixedFrameLevel(isFixed)
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int internal_SetFixedFrameLevel(lua_State L)
    {
        var frame = GetFrame(L, 1);

        var argc = lua_gettop(L);
        if (argc != 2)
        {
            lua_pushstring(L, "SetFixedFrameLevel requires exactly 1 argument: isFixed.");
            lua_error(L);
            return 0; // Unreachable
        }
        
        var isFixed = lua_toboolean(L, 2) != 0;
        
        frame?.SetFixedFrameLevel(isFixed);
        
        return 0;
    }
    
    public static int internal_SetEnabled(lua_State L)
    {
        var frame = GetFrame(L, 1);
        var enabled = lua_toboolean(L, 2) != 0;

        frame?.SetEnabled(enabled);

        return 0;
    }
}
