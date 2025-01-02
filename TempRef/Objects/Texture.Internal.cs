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