using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools;

public partial class Texture
{
    // Dictionary to hold frame methods
    public static Dictionary<string, lua_CFunction> FrameMethods = new Dictionary<string, lua_CFunction>
    {
        { "SetAllPoints", internal_SetAllPoints },
        // Add other methods...
    };
    
    public static void RegisterMetaTable(lua_State L)
    {
        // Create a new metatable for Texture objects
        luaL_newmetatable(L, "TextureMetaTable"); // Register "TextureMetaTable"

        // Set the __index field to the metatable itself to allow method access
        lua_pushstring(L, "__index");
        lua_newtable(L);

        RegisterTextureMethod(L, "SetAllPoints", internal_SetAllPoints);
        RegisterTextureMethod(L, "SetVertexOffset", internal_SetVertexOffset);
        RegisterTextureMethod(L, "SetColorTexture", internal_SetColorTexture);
        RegisterTextureMethod(L, "SetTexture", internal_SetTexture);
        RegisterTextureMethod(L, "SetTexCoord", internal_SetTexCoord);
        RegisterTextureMethod(L, "SetVertexColor", internal_SetVertexColor);
        RegisterTextureMethod(L, "SetRotation", internal_SetRotation);
        
        lua_settable(L, -3);

        lua_pushstring(L, "__gc");
        lua_pushcfunction(L, internal_TextureGC);
        lua_settable(L, -3);

        lua_pop(L, 1);
    }

    private static void RegisterTextureMethod(lua_State L, string methodName, lua_CFunction function)
    {
        lua_pushstring(L, methodName);
        lua_pushcfunction(L, function);
        lua_settable(L, -3); // metatable.__index[methodName] = function
    }

    public static Texture GetTexture(lua_State L, int index)
    {
        // Ensure the object at the index has the correct metatable
        luaL_getmetatable(L, "TextureMetaTable");
        lua_getmetatable(L, index);
        var hasMetatable = lua_rawequal(L, -1, -2) != 0;
        lua_pop(L, 2); // Pop both metatables from the stack

        if (!hasMetatable)
        {
            lua_pushstring(L, "Attempt to use a non-Texture object as a Texture.");
            lua_error(L);
            return null; // Unreachable
        }

        // Get the userdata pointer
        var userdataPtr = (IntPtr)lua_touserdata(L, index);
        if (userdataPtr == IntPtr.Zero)
        {
            lua_pushstring(L, "Invalid Texture userdata.");
            lua_error(L);
            return null; // Unreachable
        }

        // Read the IntPtr from the userdata memory
        var handlePtr = Marshal.ReadIntPtr(userdataPtr);
        if (handlePtr == IntPtr.Zero)
        {
            lua_pushstring(L, "Texture userdata contains a null GCHandle.");
            lua_error(L);
            return null; // Unreachable
        }

        var handle = GCHandle.FromIntPtr(handlePtr);
        if (!handle.IsAllocated)
        {
            lua_pushstring(L, "Texture has been garbage collected.");
            lua_error(L);
            return null; // Unreachable
        }

        var texture = handle.Target as Texture;
        if (texture == null)
        {
            lua_pushstring(L, "Texture userdata references an invalid object.");
            lua_error(L);
            return null; // Unreachable
        }

        return texture;
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
}