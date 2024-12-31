using System.Runtime.InteropServices;
using LuaNET.Lua51;
using Spectre.Console;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools;

public partial class Texture
{
    public static void RegisterMetaTable(lua_State L)
    {
        // Create a new metatable for Texture
        luaL_newmetatable(L, "TextureMetaTable");

        // Push the __index metamethod
        lua_pushstring(L, "__index");
        lua_newtable(L); // Create a table to hold the methods

        RegisterTextureMethod(L, "SetAllPoints", internal_SetAllPoints);
        RegisterTextureMethod(L, "SetVertexOffset", internal_SetVertexOffset);
        RegisterTextureMethod(L, "SetColorTexture", internal_SetColorTexture);
        RegisterTextureMethod(L, "SetTexture", internal_SetTexture);
        RegisterTextureMethod(L, "SetTexCoord", internal_SetTexCoord);
        RegisterTextureMethod(L, "SetVertexColor", internal_SetVertexColor);
        RegisterTextureMethod(L, "GetVertexColor", internal_GetVertexColor);
        RegisterTextureMethod(L, "SetRotation", internal_SetRotation);
        RegisterTextureMethod(L, "SetSize", internal_SetSize);
        RegisterTextureMethod(L, "SetPoint", internal_SetPoint);
        RegisterTextureMethod(L, "GetParent", internal_GetParent);
        
        // Set the __index table
        lua_settable(L, -3);

        // Register the __newindex metamethod
        //lua_pushstring(L, "__newindex");
        //lua_pushcfunction(L, internal_FrameNewIndex);
        //lua_settable(L, -3); // metatable.__newindex = internal_FrameNewIndex

        // Register the __tostring metamethod
        lua_pushstring(L, "__tostring");
        lua_pushcfunction(L, internal_TextureToString);
        lua_settable(L, -3); // metatable.__tostring = internal_FrameToString

        // Set the __gc metamethod to handle garbage collection if necessary
        lua_pushstring(L, "__gc");
        lua_pushcfunction(L, internal_TextureGC);
        lua_settable(L, -3);

        // Pop the metatable from the stack
        lua_pop(L, 1);
    }

    private static void RegisterTextureMethod(lua_State L, string methodName, lua_CFunction function)
    {
        lua_pushstring(L, methodName);
        lua_pushcfunction(L, function);
        lua_settable(L, -3); // metatable.__index[methodName] = function
    }

    public static Texture? GetTexture(lua_State L, int index)
    {
        if (lua_isuserdata(L, index) != 0)
        {
            IntPtr userdataPtr = (IntPtr)lua_touserdata(L, index);
            if (Frame._textureRegistry.TryGetValue(userdataPtr, out var texture))
            {
                return texture;
            }
        }
        else if (lua_istable(L, index) != 0)
        {
            // Assume the table has a '__texture' field containing the userdata
            lua_pushstring(L, "__texture");      // Push key '__texture'
            lua_gettable(L, index);             // Get table['__texture']
            if (lua_islightuserdata(L, -1) != 0)
            {
                IntPtr userdataPtr = (IntPtr)lua_touserdata(L, -1);
                lua_pop(L, 1);                   // Remove '__texture' value from stack
                if (Frame._textureRegistry.TryGetValue(userdataPtr, out var texture))
                {
                    return texture;
                }
            }
            else
            {
                lua_pop(L, 1);                   // Remove '__texture' value from stack
            }
        }

        return null; // Frame not found or invalid argument
    }

    public static int internal_SetAllPoints(lua_State L)
    {
        var frame = GetTexture(L, 1);

        var argc = lua_gettop(L);
        string? relativeTo = null;
        var doResize = false;
        if (argc >= 2) relativeTo = lua_tostring(L, 2);

        if (argc >= 3) doResize = lua_toboolean(L, 3) != 0;

        frame?.SetAllPoints(relativeTo, doResize);

        lua_pushboolean(L, 1);
        return 1;
    }

    public static int internal_SetVertexOffset(lua_State L)
    {
        var frame = GetTexture(L, 1);

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
    ///     https://warcraft.wiki.gg/wiki/API_TextureBase_SetColorTexture
    ///     TextureBase:SetColorTexture(colorR, colorG, colorB [, a])
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int internal_SetColorTexture(lua_State L)
    {
        var texture = GetTexture(L, 1);

        var argc = lua_gettop(L);
        if (argc < 4)
        {
            lua_pushstring(L, "SetColorTexture requires at least 3 arguments: colorR, colorG, colorB.");
            lua_error(L);
            return 0; // Unreachable
        }

        var colorR = (float)lua_tonumber(L, 2);
        var colorG = (float)lua_tonumber(L, 3);
        var colorB = (float)lua_tonumber(L, 4);
        var colorA = 1.0f;

        if (argc == 5) colorA = (float)lua_tonumber(L, 5);

        texture?.SetColorTexture(colorR, colorG, colorB, colorA);

        lua_pushboolean(L, 1);
        return 1;
    }

    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_TextureBase_SetTexture
    /// TextureBase:SetTexture([textureAsset [, wrapModeHorizontal [, wrapModeVertical [, filterMode]]]])
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int internal_SetTexture(lua_State L)
    {
        var texture = GetTexture(L, 1);

        var argc = lua_gettop(L);
        if (argc < 2)
        {
            lua_pushstring(L, "SetTexture requires at least 1 argument: textureAsset.");
            lua_error(L);
            return 0; // Unreachable
        }

        // Asset can be a string or a number


        int textureAssetInt = 0;
        string? textureAssetStr = null;
        bool isNumber = false;
        
        if (lua_isstring(L, 2) != 0)
        {
            isNumber = false;
            textureAssetStr = lua_tostring(L, 2);
        }
        else if (lua_isnumber(L, 2) != 0)
        {
            isNumber = true;
            textureAssetInt = (int)lua_tonumber(L, 2);
        }
        
        string? wrapModeHorizontal = null;
        string? wrapModeVertical = null;
        string? filterMode = null;
        
        if (argc >= 3) wrapModeHorizontal = lua_tostring(L, 3);
        if (argc >= 4) wrapModeVertical = lua_tostring(L, 4);
        if (argc >= 5) filterMode = lua_tostring(L, 5);

        if (isNumber)
            texture?.SetTexture(textureAssetInt, wrapModeHorizontal, wrapModeVertical, filterMode);
        else
            texture?.SetTexture(textureAssetStr, wrapModeHorizontal, wrapModeVertical, filterMode);
        
        lua_pushboolean(L, 1);
        return 1;
    }

    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_TextureBase_SetTexCoord
    /// TextureBase:SetTexCoord(left, right, top, bottom)
    /// TextureBase:SetTexCoord(ULx, ULy, LLx, LLy, URx, URy, LRx, LRy)
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int internal_SetTexCoord(lua_State L)
    {
        var texture = GetTexture(L, 1);

        var argc = lua_gettop(L);
        if (argc == 5)
        {
            // TextureBase:SetTexCoord(left, right, top, bottom)
            var left = (float)lua_tonumber(L, 2);
            var right = (float)lua_tonumber(L, 3);
            var top = (float)lua_tonumber(L, 4);
            var bottom = (float)lua_tonumber(L, 5);

            texture?.SetTexCoord(left, right, top, bottom);
        }
        else if (argc == 9)
        {
            // TextureBase:SetTexCoord(ULx, ULy, LLx, LLy, URx, URy, LRx, LRy)
            var ULx = (float)lua_tonumber(L, 2);
            var ULy = (float)lua_tonumber(L, 3);
            var LLx = (float)lua_tonumber(L, 4);
            var LLy = (float)lua_tonumber(L, 5);
            var URx = (float)lua_tonumber(L, 6);
            var URy = (float)lua_tonumber(L, 7);
            var LRx = (float)lua_tonumber(L, 8);
            var LRy = (float)lua_tonumber(L, 9);

            texture?.SetTexCoord(ULx, ULy, LLx, LLy, URx, URy, LRx, LRy);
        }
        else
        {
            lua_pushstring(L, "SetTexCoord requires either 4 or 8 arguments.");
            lua_error(L);
            return 0; // Unreachable
        }

        lua_pushboolean(L, 1);
        return 1;
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Region_SetVertexColor
    /// Region:SetVertexColor(colorR, colorG, colorB [, a])
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int internal_SetVertexColor(lua_State L)
    {
        var texture = GetTexture(L, 1);

        var argc = lua_gettop(L);
        if (argc < 3)
        {
            lua_pushstring(L, "SetVertexColor requires at least 3 arguments: colorR, colorG, colorB.");
            lua_error(L);
            return 0; // Unreachable
        }

        var colorR = (float)lua_tonumber(L, 2);
        var colorG = (float)lua_tonumber(L, 3);
        var colorB = (float)lua_tonumber(L, 4);
        float? colorA = null;

        if (argc >= 5)
        {
            colorA = (float)lua_tonumber(L, 5);
        }

        texture?.SetVertexColor(colorR, colorG, colorB, colorA);

        lua_pushboolean(L, 1);
        return 1;
    }
    
    public static int internal_GetVertexColor(lua_State L)
    {
        var texture = GetTexture(L, 1);

        var color = texture?.GetVertexColor();

        lua_pushnumber(L, color?[0] ?? 1);
        lua_pushnumber(L, color?[1] ?? 1);
        lua_pushnumber(L, color?[2] ?? 1);
        lua_pushnumber(L, color?[3] ?? 1);

        return 4;
    }
    
    public static int internal_TextureToString(lua_State L)
    {
        try
        {
            // Stack:
            // 1 - table

            // Retrieve the table
            if (lua_istable(L, 1) == 0)
            {
                lua_pushstring(L, "Texture: Invalid Texture Table");
                return 1;
            }

            // Retrieve the Frame userdata from the table's __texture field
            lua_pushstring(L, "__texture");
            lua_gettable(L, 1); // table.__texture
            if (lua_islightuserdata(L, -1) == 0)
            {
                lua_pop(L, 1);
                lua_pushstring(L, "Texture: __texture field is missing or invalid");
                return 1;
            }

            IntPtr frameUserdataPtr = (IntPtr)lua_touserdata(L, -1);
            lua_pop(L, 1); // Remove __texture userdata from the stack

            // Retrieve the Frame instance
            if (!Frame._textureRegistry.TryGetValue(frameUserdataPtr, out var texture))
            {
                lua_pushstring(L, "Frame: Frame not found in registry");
                return 1;
            }

            // Construct a meaningful string representation
            string textureName = string.IsNullOrEmpty(texture._name) ? "Unnamed" : texture._name;

            string result = $"Texture: Name='{textureName}'";

            // Push the string onto the Lua stack
            lua_pushstring(L, result);
            return 1; // Number of return values
        }
        catch (Exception ex)
        {
            lua_pushstring(L, $"Texture: Error - {ex.Message}");
            return 1;
        }
    }
    
    private static int internal_TextureGC(lua_State L)
    {
        // Retrieve the userdata pointer
        var userdataPtr = (IntPtr)lua_touserdata(L, 1);
        if (userdataPtr == IntPtr.Zero) return 0;

        if (Frame._textureRegistry.ContainsKey(userdataPtr))
        {
            var frame = Frame._textureRegistry[userdataPtr];

            // Unregister all events
            //frame.UnregisterAllEvents();

            // Unreference all script handlers
            //foreach (var refIndex in frame._scriptRefs.Values) luaL_unref(L, LUA_REGISTRYINDEX, refIndex);

            // Free the GCHandle
            var handlePtr = Marshal.ReadIntPtr(userdataPtr);
            var handle = GCHandle.FromIntPtr(handlePtr);

            if (handle.IsAllocated) handle.Free();

            // Remove the Frame from the registry
            Frame._textureRegistry.Remove(userdataPtr);
        }

        return 0;
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_TextureBase_SetRotation
    /// TextureBase:SetRotation(radians [, normalizedRotationPoint])
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    private static int internal_SetRotation(lua_State L)
    {
        var texture = GetTexture(L, 1);

        var argc = lua_gettop(L);
        if (argc < 1)
        {
            lua_pushstring(L, "SetRotation requires at least 1 argument: radians.");
            lua_error(L);
            return 0; // Unreachable
        }

        var radians = (float)lua_tonumber(L, 2);
        string? normalizedRotationPoint = null;

        if (argc >= 2)
        {
            normalizedRotationPoint = lua_tostring(L, 3);
        }

        texture?.SetRotation(radians, normalizedRotationPoint);

        lua_pushboolean(L, 1);
        return 1;
     
    }
    
     /// <summary>
    ///     Sets the size of the Frame.
    /// </summary>
    /// <param name="L">The Lua state.</param>
    /// <returns>Number of return values (0).</returns>
    public static int internal_SetSize(lua_State L)
    {
        var texture = GetTexture(L, 1);

        var argc = lua_gettop(L);
        if (argc < 3)
        {
            lua_pushstring(L, "SetSize requires at exactly 2 arguments: width, height.");
            lua_error(L);
            return 0; // Unreachable
        }

        var width = (float)lua_tonumber(L, 2);
        var height = (float)lua_tonumber(L, 3);

        texture?.SetSize(width, height);

        lua_pushboolean(L, 1);
        return 1;
    }
     
    public static int internal_SetPoint(lua_State L)
    {
        var texture = GetTexture(L, 1);

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

        texture?.SetPoint(point, relativeTo, relativePoint, offsetX, offsetY);

        lua_pushboolean(L, 1);
        return 1;
    }
    
    public static int internal_GetParent(lua_State L)
    {
        var texture = GetTexture(L, 1);
        var parent = texture?.GetParent();

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
    
    public static void PushExistingFrameToLua(lua_State L, Frame child)
    {
        // If you're storing the table in child.LuaRegistryRef, do:
        lua_rawgeti(L, LUA_REGISTRYINDEX, child.LuaRegistryRef);
        // Now the child's table/userdata is on top of the stack.
        // If child has no registry ref, you might need to create one or log an error.
    }
}