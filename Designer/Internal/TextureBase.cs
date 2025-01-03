using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Internal;

public static class TextureBase
{
    public static int internal_SetColorTexture(lua_State L)
    {
        var textureBase = GetThis(L, 1);

        var argc = lua_gettop(L);
        if (argc < 4)
        {
            Log.ErrorL(L, "SetColorTexture requires at least 3 arguments: colorR, colorG, colorB.");
            return 0; // Unreachable
        }

        var colorR = (float)lua_tonumber(L, 2);
        var colorG = (float)lua_tonumber(L, 3);
        var colorB = (float)lua_tonumber(L, 4);
        var colorA = 1.0f;

        if (argc == 5) colorA = (float)lua_tonumber(L, 5);

        textureBase?.SetColorTexture(colorR, colorG, colorB, colorA);

        lua_pushboolean(L, 1);
        return 1;
    }
    
    public static int internal_SetRotation(lua_State L)
    {
        var texture = GetThis(L, 1);
        var argc = lua_gettop(L);
        
        if (argc < 3)
        {
            Log.ErrorL(L, "SetRotation requires at least 1 argument: radians.");
            return 0; // Unreachable
        }

        var radians = (float)lua_tonumber(L, 2);
        string? normalizedRotationPoint = null;

        if (argc > 3)
        {
            normalizedRotationPoint = lua_tostring(L, 3);
        }

        texture?.SetRotation(radians, normalizedRotationPoint);

        lua_pushboolean(L, 1);
        return 1;
    }
    
    public static int internal_SetTexCoord(lua_State L)
    {
        var textureBase = GetThis(L, 1);

        var argc = lua_gettop(L);
        if (argc <= 6)
        {
            // TextureBase:SetTexCoord(left, right, top, bottom)
            var left = (float)lua_tonumber(L, 2);
            var right = (float)lua_tonumber(L, 3);
            var top = (float)lua_tonumber(L, 4);
            var bottom = (float)lua_tonumber(L, 5);

            textureBase?.SetTexCoord(left, right, top, bottom);
        }
        else if (argc == 10)
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

            textureBase?.SetTexCoord(ULx, ULy, LLx, LLy, URx, URy, LRx, LRy);
        }
        else
        {
            Log.ErrorL(L, "SetTexCoord requires either 4 or 8 arguments.");
            return 0; // Unreachable
        }

        lua_pushboolean(L, 1);
        return 1;
    }
    
    public static int internal_SetTexture(lua_State L)
    {
        var textureBase = GetThis(L, 1);

        var argc = lua_gettop(L);
        if (argc < 2)
        {
            Log.ErrorL(L, "SetTexture requires at least 1 argument: textureAsset.");
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
            textureBase?.SetTexture(textureAssetInt, wrapModeHorizontal, wrapModeVertical, filterMode);
        else
            textureBase?.SetTexture(textureAssetStr, wrapModeHorizontal, wrapModeVertical, filterMode);
        
        lua_pushboolean(L, 1);
        return 1;
    }
    
    public static int internal_SetVertexOffset(lua_State L)
    {
        var textureBase = GetThis(L, 1);

        var argc = lua_gettop(L);
        if (argc < 4)
        {
            Log.ErrorL(L, "SetVertexOffset requires exactly 3 arguments: vertexIndex, offsetX, offsetY.");
            return 0; // Unreachable
        }

        var vertexIndex = (int)lua_tonumber(L, 2);
        var offsetX = (float)lua_tonumber(L, 3);
        var offsetY = (float)lua_tonumber(L, 4);

        textureBase?.SetVertexOffset(vertexIndex, offsetX, offsetY);

        lua_pushboolean(L, 1);
        return 1;
    }
    
    public static string GetMetatableName() => "TextureBaseMetaTable";

    public static Widgets.TextureBase? GetThis(lua_State L, int index)
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

        return handle.Target as Widgets.TextureBase;
    }
    
    public static void RegisterMetaTable(lua_State L)
    {
        // 1) call base to register the "TextureBaseMetaTable"
        Region.RegisterMetaTable(L);

        // 2) Now define "TextureBaseMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = TextureBaseMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("RegionMetaTable")
        var baseMetaName = Region.GetMetatableName();
        luaL_getmetatable(L, baseMetaName);
        lua_setmetatable(L, -2); // Sets TextureBaseMetaTable's metatable to RegionMetaTable
        
        // 5) Bind Frame-specific methods
        LuaHelpers.RegisterMethod(L, "SetColorTexture", internal_SetColorTexture);
        LuaHelpers.RegisterMethod(L, "SetVertexOffset", internal_SetVertexOffset);
        LuaHelpers.RegisterMethod(L, "SetTexture", internal_SetTexture);
        LuaHelpers.RegisterMethod(L, "SetTexCoord", internal_SetTexCoord);
        LuaHelpers.RegisterMethod(L, "SetRotation", internal_SetRotation);

        // 5) pop
        lua_pop(L, 1);
    }
}