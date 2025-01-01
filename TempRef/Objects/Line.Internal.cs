using System.Runtime.InteropServices;
using LuaNET.Lua51;
using Spectre.Console;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools;

public partial class Line
{
    public static void RegisterMetatable(lua_State L)
    {
        // Create a new metatable for FontString objects
        luaL_newmetatable(L, "LineMetaTable"); // Register "FontStringMetaTable"

        // Set the __index field to the metatable itself to allow method access
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // Set __gc metamethod to handle garbage collection
        lua_pushcfunction(L, internal_LineGC);
        lua_setfield(L, -2, "__gc");
        
        // Set the methods in the metatable
        lua_pushcfunction(L, internal_SetThickness);
        lua_setfield(L, -2, "SetThickness");
        
        RegisterLineMethod(L, "SetThickness", internal_SetThickness);
        RegisterLineMethod(L, "SetTexture", internal_SetTexture);
        RegisterLineMethod(L, "SetTexCoord", internal_SetTexCoord);
        RegisterLineMethod(L, "SetVertexColor", internal_SetVertexColor);
        RegisterLineMethod(L, "Show", internal_Show);
        RegisterLineMethod(L, "Hide", internal_Hide);
        RegisterLineMethod(L, "SetStartPoint", internal_SetStartPoint);
        RegisterLineMethod(L, "SetEndPoint", internal_SetEndPoint);

        // Pop the metatable from the stack
        lua_pop(L, 1);
    }
    
    private static void RegisterLineMethod(lua_State L, string methodName, lua_CFunction function)
    {
        lua_pushstring(L, methodName);
        lua_pushcfunction(L, function);
        lua_settable(L, -3); // metatable.__index[methodName] = function
    }

    public static Line? GetLine(lua_State L, int index)
    {
        // Ensure the object at the index has the correct metatable
        luaL_getmetatable(L, "LineMetaTable");
        lua_getmetatable(L, index);
        var hasMetatable = lua_rawequal(L, -1, -2) != 0;
        lua_pop(L, 2); // Pop both metatables from the stack

        if (!hasMetatable)
        {
            lua_pushstring(L, "Attempt to use a non-Line object as a Line.");
            lua_error(L);
            return null; // Unreachable
        }

        // Get the userdata pointer
        var userdataPtr = (IntPtr)lua_touserdata(L, index);
        if (userdataPtr == IntPtr.Zero)
        {
            lua_pushstring(L, "Invalid Line userdata.");
            lua_error(L);
            return null; // Unreachable
        }

        // Read the IntPtr from the userdata memory
        var handlePtr = Marshal.ReadIntPtr(userdataPtr);
        if (handlePtr == IntPtr.Zero)
        {
            lua_pushstring(L, "Line userdata contains a null GCHandle.");
            lua_error(L);
            return null; // Unreachable
        }

        var handle = GCHandle.FromIntPtr(handlePtr);
        if (!handle.IsAllocated)
        {
            lua_pushstring(L, "Line has been garbage collected.");
            lua_error(L);
            return null; // Unreachable
        }

        var line = handle.Target as Line;
        if (line == null)
        {
            luaL_error(L, "Line userdata references an invalid object.");
            return null; // Unreachable
        }

        return line;
    }
    
    public static int internal_LineGC(lua_State L)
    {
        // Retrieve the userdata pointer
        var userdataPtr = (IntPtr)lua_touserdata(L, 1);
        if (userdataPtr == IntPtr.Zero) return 0;

        if (Frame._lineRegistry.ContainsKey(userdataPtr))
        {
            var line = Frame._lineRegistry[userdataPtr];
            
            // Free the GCHandle
            var handlePtr = Marshal.ReadIntPtr(userdataPtr);
            var handle = GCHandle.FromIntPtr(handlePtr);

            if (handle.IsAllocated) handle.Free();

            // Remove the Frame from the registry
            Frame._lineRegistry.Remove(userdataPtr);
        }

        return 0;
    }
    

}