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
}