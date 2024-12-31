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
    
    public static int internal_SetThickness(lua_State L)
    {
        var line = GetLine(L, 1);
        if (line == null) return 0;

        var thickness = (float)luaL_checknumber(L, 2);
        line.SetThickness(thickness);

        return 0;
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_TextureBase_SetTexture
    /// TextureBase:SetTexture([textureAsset [, wrapModeHorizontal [, wrapModeVertical [, filterMode]]]])
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int internal_SetTexture(lua_State L)
    {
        var line = GetLine(L, 1);

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
            line?.SetTexture(textureAssetInt, wrapModeHorizontal, wrapModeVertical, filterMode);
        else
            line?.SetTexture(textureAssetStr, wrapModeHorizontal, wrapModeVertical, filterMode);
        
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
        var line = GetLine(L, 1);

        var argc = lua_gettop(L);
        if (argc == 5)
        {
            // TextureBase:SetTexCoord(left, right, top, bottom)
            var left = (float)lua_tonumber(L, 2);
            var right = (float)lua_tonumber(L, 3);
            var top = (float)lua_tonumber(L, 4);
            var bottom = (float)lua_tonumber(L, 5);

            line?.SetTexCoord(left, right, top, bottom);
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

            line?.SetTexCoord(ULx, ULy, LLx, LLy, URx, URy, LRx, LRy);
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
        var line = GetLine(L, 1);

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

        line?.SetVertexColor(colorR, colorG, colorB, colorA);

        lua_pushboolean(L, 1);
        return 1;
    }
    
    public static int internal_Show(lua_State L)
    {
        var line = GetLine(L, 1);
        line.Show();
        lua_pushboolean(L, 1);
        return 1;
    }

    public static int internal_Hide(lua_State L)
    {
        var line = GetLine(L, 1);
        line.Hide();
        lua_pushboolean(L, 1);
        return 1;
    }

    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Line_SetStartPoint
    /// Line:SetStartPoint(relativePoint [, relativeTo] [, offsetX, offsetY])
    /// relativeTo
    /// ScriptRegion|string? - If omitted, anchors to the parent region. If explicitly passed nil, anchors relative to the screen dimensions.
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int internal_SetStartPoint(lua_State L)
    {
        var line = GetLine(L, 1);

        var relativePoint = luaL_checkstring(L, 2);
        Frame? relativeTo = null;
        int offsetX = 0;
        int offsetY = 0;
        
        if (lua_isstring(L, 3) != 0)
        {
            relativeTo = Frame.GetFrame(L, 2);
        }
        else if (lua_isnumber(L, 3) != 0)
        {
            offsetX = (int)lua_tonumber(L, 3);
            offsetY = (int)lua_tonumber(L, 4);
        }

        line?.SetStartPoint(relativePoint, relativeTo, offsetX, offsetY);

        lua_pushboolean(L, 1);
        return 1;
    }

    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Line_SetEndPoint
    /// Line:SetEndPoint(relativePoint [, relativeTo] [, offsetX, offsetY])
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int internal_SetEndPoint(lua_State L)
    {
        var line = GetLine(L, 1);

        var relativePoint = luaL_checkstring(L, 2);
        Frame? relativeTo = null;
        int offsetX = 0;
        int offsetY = 0;
        
        if (lua_isstring(L, 3) != 0)
        {
            relativeTo = Frame.GetFrame(L, 2);
        }
        else if (lua_isnumber(L, 3) != 0)
        {
            offsetX = (int)lua_tonumber(L, 3);
            offsetY = (int)lua_tonumber(L, 4);
        }

        line?.SetStartPoint(relativePoint, relativeTo, offsetX, offsetY);

        lua_pushboolean(L, 1);
        return 1;
    }
}