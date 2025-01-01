using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets;

/// <summary>
/// https://warcraft.wiki.gg/wiki/UIOBJECT_Texture
/// </summary>
public class Texture : TextureBase
{
    public Texture(string? name = null, string? drawLayer = null, string? templateName = null, int subLevel = 0, Frame? parent = null)
        : base("Texture", name, drawLayer, templateName, subLevel, parent)
    {
    }
    
    // Texture:AddMaskTexture(mask)
    // Texture:GetMaskTexture(index) : mask
    // Texture:GetNumMaskTextures() : count
    // Texture:RemoveMaskTexture(mask)
    
    public override string ToString()
    {
        return $"Texture: {GetName() ?? "nil"} - {_drawLayer ?? "nil"}";
    }
    
    // ----------- Virtual Registration ---------------
    
    public override string GetMetatableName() => "TextureMetaTable";
        
    public override void RegisterMetaTable(lua_State L)
    {
        // 1) call base to register the "TextureMetaTable"
        base.RegisterMetaTable(L);

        // 2) Now define "TextureMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = self
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Bind methods
        //LuaHelpers.RegisterMethod(L, "RegisterEvent", internal_RegisterEvent);
        //LuaHelpers.RegisterMethod(L, "UnregisterAllEvents", internal_UnregisterAllEvents);
        //LuaHelpers.RegisterMethod(L, "UnregisterEvent", internal_UnregisterEvent);
        //LuaHelpers.RegisterMethod(L, "SetFrameStrata", internal_SetFrameStrata);

        // Optional __gc
        lua_pushcfunction(L, internal_FrameGC);
        lua_setfield(L, -2, "__gc");

        // 5) pop
        lua_pop(L, 1);
    }
    
    private int internal_FrameGC(lua_State L)
    {
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
        if (API.UIObjects._textureRegistry.TryGetValue(frameUserdataPtr, out var frame))
        {
            // Free the GCHandle
            if (frame.Handle.IsAllocated)
            {
                frame.Handle.Free();
            }

            // Remove from registry
            API.UIObjects._textureRegistry.Remove(frameUserdataPtr);

            // Perform any additional cleanup if necessary
            // Example: frame.Dispose();
        }

        return 0;
    }
}