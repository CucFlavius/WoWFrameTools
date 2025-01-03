using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Internal;

public static class Button
{
    public static int internal_RegisterForClicks(lua_State L)
    {
        var frame = GetThis(L, 1);

        var argc = lua_gettop(L);
        if (argc < 2)
        {
            Log.ErrorL(L, "RegisterForClicks requires at least 1 argument: button1.");
            return 0; // Unreachable
        }

        List<string> buttons = new();
        for (var i = 2; i < argc; i++)
        {
            if (lua_isstring(L, i) != 0)
            {
                var button = lua_tostring(L, i);
                buttons.Add(button);
            }
            else
            {
                throw new ArgumentException($"Argument {i} is not a valid string.");
            }
        }

        frame?.RegisterForClicks(buttons.ToArray());

        lua_pushboolean(L, 1);
        return 1;
        
    }
    
    public static int internal_SetHighlightTexture(lua_State L)
    {
        var frame = GetThis(L, 1);
        string? blendMode = null;
        
        if (lua_gettop(L) >= 3)
        {
            blendMode = lua_tostring(L, 3);
        }
        
        if (lua_istable(L, 2) != 0)
        {
            var texture = Texture.GetThis(L, 2) as Widgets.Texture;
            if (texture == null)
                return 0;
            frame?.SetHighlightTexture(texture, blendMode);
        }
        else if (lua_isnumber(L, 2) != 0)
        {
            var fileID = (int)lua_tonumber(L, 2);
            frame?.SetHighlightTexture(fileID, blendMode);
        }
        
        return 0;
    }
    
    public static int internal_SetNormalTexture(lua_State L)
    {
        var frame = GetThis(L, 1);
        string? blendMode = null;
        
        if (lua_gettop(L) >= 3)
        {
            blendMode = lua_tostring(L, 3);
        }
        
        if (lua_istable(L, 2) != 0)
        {
            var texture = Texture.GetThis(L, 2) as Widgets.Texture;
            if (texture == null)
                return 0;
            frame?.SetNormalTexture(texture, blendMode);
        }
        else if (lua_isnumber(L, 2) != 0)
        {
            var fileID = (int)lua_tonumber(L, 2);
            frame?.SetNormalTexture(fileID, blendMode);
        }
        
        return 0;
    }
    
    public static int internal_SetPushedTexture(lua_State L)
    {
        var frame = GetThis(L, 1);
        string? blendMode = null;
        
        if (lua_gettop(L) >= 3)
        {
            blendMode = lua_tostring(L, 3);
        }
        
        if (lua_istable(L, 2) != 0)
        {
            var texture = Texture.GetThis(L, 2) as Widgets.Texture;
            if (texture == null)
                return 0;
            frame?.SetPushedTexture(texture, blendMode);
        }
        else if (lua_isnumber(L, 2) != 0)
        {
            var fileID = (int)lua_tonumber(L, 2);
            frame?.SetPushedTexture(fileID, blendMode);
        }
        
        return 0;
    }
    
    public static string GetMetatableName() => "ButtonMetaTable";

    private static Widgets.Button? GetThis(lua_State L, int index)
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

        return handle.Target as Widgets.Button;
    }
    
    public static void RegisterMetaTable(lua_State L)
    {
        // 1) call base to register the "ButtonMetaTable"
        Frame.RegisterMetaTable(L);

        // 2) Now define "ButtonMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = ButtonMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("FrameMetaTable")
        var baseMetaName = Frame.GetMetatableName();
        luaL_getmetatable(L, baseMetaName);
        lua_setmetatable(L, -2); // Sets ButtonMetaTable's metatable to FrameMetaTable
        
        // 5) Bind Frame-specific methods
        LuaHelpers.RegisterMethod(L, "SetHighlightTexture", internal_SetHighlightTexture);
        LuaHelpers.RegisterMethod(L, "SetNormalTexture", internal_SetNormalTexture);
        LuaHelpers.RegisterMethod(L, "SetPushedTexture", internal_SetPushedTexture);
        LuaHelpers.RegisterMethod(L, "RegisterForClicks", internal_RegisterForClicks);
        
        // 6) pop
        lua_pop(L, 1);
    }
}