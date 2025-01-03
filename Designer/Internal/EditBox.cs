using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Internal;

public static class EditBox
{
    public static int internal_SetAutoFocus(lua_State L)
    {
        var frame = GetThis(L, 1);
        var autoFocus = lua_toboolean(L, 2) != 0;

        frame?.SetAutoFocus(autoFocus);

        return 0;
    }
    
    public static int internal_SetEnabled(lua_State L)
    {
        var frame = GetThis(L, 1);
        var enabled = lua_toboolean(L, 2) != 0;

        frame?.SetEnabled(enabled);

        return 0;
    }
    
    public static int internal_SetMaxLetters(lua_State L)
    {
        var frame = GetThis(L, 1);
        var maxLetters = (int)lua_tonumber(L, 2);

        frame?.SetMaxLetters(maxLetters);

        return 0;
    }
    
    public static int internal_SetText(lua_State L)
    {
        try
        {
            var fontString = GetThis(L, 1);
            if (fontString == null)
            {
                Log.ErrorL(L, "SetText: Invalid EditBox object.");
                return 0; // Unreachable
            }

            var text = lua_tostring(L, 2) ?? "";

            fontString.SetText(text);
        }
        catch (Exception ex)
        {
            Log.ErrorL(L, $"SetText: {ex.Message}");
            return 0; // Unreachable
        }

        return 0; // No return values
    }
    
    public static int internal_SetFont(lua_State L)
    {
        var editBox = GetThis(L, 1);
        
        var argc = lua_gettop(L);
        if (argc < 5)
        {
            Log.ErrorL(L, "SetFont requires exactly 3 arguments: fontFile, height, flags.");
            return 0; // Unreachable
        }
        
        var fontFile = lua_tostring(L, 2);
        var height = (int)lua_tonumber(L, 3);
        var flags = lua_tostring(L, 4);
        
        var success = editBox?.SetFont(fontFile, height, flags);

        lua_pushboolean(L, success == true ? 1 : 0);
        return 1;
    }
    
    public static int internal_SetJustifyH(lua_State L)
    {
        try
        {
            var fontString = GetThis(L, 1);
            if (fontString == null)
            {
                Log.ErrorL(L, "SetJustifyH: Invalid FontString object.");
                return 0; // Unreachable
            }

            var justify = lua_tostring(L, 2) ?? "LEFT";

            fontString.SetJustifyH(justify);
        }
        catch (Exception ex)
        {
            Log.ErrorL(L, $"SetJustifyH: {ex.Message}");
            return 0; // Unreachable
        }

        return 0; // No return values
    }
    
    public static int internal_SetJustifyV(lua_State L)
    {
        try
        {
            var fontString = GetThis(L, 1);
            if (fontString == null)
            {
                Log.ErrorL(L, "SetJustifyV: Invalid FontString object.");
                return 0; // Unreachable
            }

            var justify = lua_tostring(L, 2) ?? "MIDDLE";

            fontString.SetJustifyV(justify);
        }
        catch (Exception ex)
        {
            Log.ErrorL(L, $"SetJustifyV: {ex.Message}");
            return 0; // Unreachable
        }

        return 0; // No return values
    }
    
    public static int internal_SetTextColor(lua_State L)
    {
        var fontString = GetThis(L, 1);

        var argc = lua_gettop(L);
        if (argc < 3)
        {
            Log.ErrorL(L, "SetTextColor requires at least 3 arguments: colorR, colorG, colorB.");
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
    
    public static string GetMetatableName() => "EditBoxMetaTable";

    public static Widgets.EditBox? GetThis(lua_State L, int index)
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

        return handle.Target as Widgets.EditBox;
    }
    
    public static void RegisterMetaTable(lua_State L)
    {
        // 1) call base to register the "EditBoxMetaTable"
        Frame.RegisterMetaTable(L);

        // 2) Now define "EditBoxMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = EditBoxMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("FrameMetaTable")
        var baseMetaName = Frame.GetMetatableName();
        luaL_getmetatable(L, baseMetaName);
        lua_setmetatable(L, -2); // Sets EditBoxMetaTable's metatable to FrameMetaTable
        
        // 5) Bind Frame-specific methods
        LuaHelpers.RegisterMethod(L, "SetText", internal_SetText);
        LuaHelpers.RegisterMethod(L, "SetAutoFocus", internal_SetAutoFocus);
        LuaHelpers.RegisterMethod(L, "SetMaxLetters", internal_SetMaxLetters);
        LuaHelpers.RegisterMethod(L, "SetEnabled", internal_SetEnabled);
        
        // IFontInstance
        LuaHelpers.RegisterMethod(L, "SetFont", internal_SetFont);
        LuaHelpers.RegisterMethod(L, "SetJustifyH", internal_SetJustifyH);
        LuaHelpers.RegisterMethod(L, "SetJustifyV", internal_SetJustifyV);
        LuaHelpers.RegisterMethod(L, "SetTextColor", internal_SetTextColor);
        
        // 6) pop
        lua_pop(L, 1);
    }
}