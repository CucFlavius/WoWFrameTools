using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets;

public class TextureBase : Region
{
    protected TextureBase(string objectType, string? name, string? drawLayer, string? templateName, int subLevel, Frame? parent)
        : base(objectType, name, drawLayer, parent)
    {
    }
    // TextureBase:ClearTextureSlice()
    // TextureBase:GetAtlas() : atlas - Returns the atlas for the texture.
    // TextureBase:GetBlendMode() : blendMode - Returns the blend mode of the texture.
    // TextureBase:GetDesaturation() : desaturation - Returns the desaturation level of the texture.
    // TextureBase:GetHorizTile() : tiling - Returns true if the texture is tiling horizontally.
    // TextureBase:GetRotation() : radians, normalizedRotationPoint - Returns the rotation of the texture.
    // TextureBase:GetTexCoord() : ULx, ULy, LLx, LLy, URx, URy, LRx, LRy - Returns the texture space coordinates of the texture.
    // TextureBase:GetTexelSnappingBias() : bias - Returns the texel snapping bias for the texture.
    // TextureBase:GetTexture() : textureFile - Returns the FileID for the texture.
    // TextureBase:GetTextureFileID() : textureFile - Returns the FileID for the texture.
    // TextureBase:GetTextureFilePath() : textureFile - Returns the FileID for the texture.
    // TextureBase:GetTextureSliceMargins() : left, top, right, bottom
    // TextureBase:GetTextureSliceMode() : sliceMode
    // TextureBase:GetVertTile() : tiling - Returns true if the texture is tiling vertically.
    // TextureBase:GetVertexOffset(vertexIndex) : offsetX, offsetY - Returns a vertex offset for the texture.
    // TextureBase:IsBlockingLoadRequested() : blocking
    // TextureBase:IsDesaturated() : desaturated - Returns true if the texture is desaturated.
    // TextureBase:IsSnappingToPixelGrid() : snap - Returns true if the texture is snapping to the pixel grid.
    // TextureBase:SetAtlas(atlas [, useAtlasSize [, filterMode [, resetTexCoords]]]) - Sets the texture to an atlas.
    // TextureBase:SetBlendMode(blendMode) - Sets the blend mode of the texture.
    // TextureBase:SetBlockingLoadsRequested([blocking])
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_TextureBase_SetColorTexture
    /// TextureBase:SetColorTexture(colorR, colorG, colorB [, a]) - Sets the texture to a solid color.
    /// </summary>
    /// <param name="colorR"></param>
    /// <param name="colorG"></param>
    /// <param name="colorB"></param>
    /// <param name="colorA"></param>
    private void SetColorTexture(float colorR, float colorG, float colorB, float colorA)
    {
    }
    private int internal_SetColorTexture(lua_State L)
    {
        var textureBase = GetThis(L, 1) as TextureBase;

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
    
    // TextureBase:SetDesaturated([desaturated]) - Sets the texture to be desaturated.
    // TextureBase:SetDesaturation(desaturation) - Sets the desaturation level of the texture.
    // TextureBase:SetGradient(orientation, minColor, maxColor) - Sets a gradient color shading for the texture.
    // TextureBase:SetHorizTile([tiling]) - Sets whether the texture should tile horizontally.
    // TextureBase:SetMask(file) - Applies a mask to the texture.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_TextureBase_SetRotation
    /// TextureBase:SetRotation(radians [, normalizedRotationPoint]) - Applies a rotation to the texture.
    /// </summary>
    /// <param name="radians"></param>
    /// <param name="normalizedRotationPoint"></param>
    private void SetRotation(float radians, string? normalizedRotationPoint)
    {
        
    }
    private int internal_SetRotation(lua_State L)
    {
        var texture = GetThis(L, 1) as TextureBase;
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
    
    // TextureBase:SetSnapToPixelGrid([snap]) - Sets the texture to snap to the pixel grid.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_TextureBase_SetTexCoord
    /// TextureBase:SetTexCoord(left, right, top, bottom) - Sets the coordinates for cropping or transforming the texture.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <param name="top"></param>
    /// <param name="bottom"></param>
    private void SetTexCoord(float left, float right, float top, float bottom)
    {

    }
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_TextureBase_SetTexCoord
    /// TextureBase:SetTexCoord(ULx, ULy, LLx, LLy, URx, URy, LRx, LRy)
    /// </summary>
    /// <param name="uLx"></param>
    /// <param name="uLy"></param>
    /// <param name="lLx"></param>
    /// <param name="lLy"></param>
    /// <param name="uRx"></param>
    /// <param name="uRy"></param>
    /// <param name="lRx"></param>
    /// <param name="lRy"></param>
    private void SetTexCoord(float uLx, float uLy, float lLx, float lLy, float uRx, float uRy, float lRx, float lRy)
    {
 
    }
    private int internal_SetTexCoord(lua_State L)
    {
        var textureBase = GetThis(L, 1) as TextureBase;

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
    
    // TextureBase:SetTexelSnappingBias(bias) - Returns the texel snapping bias for the texture.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_TextureBase_SetTexture
    /// TextureBase:SetTexture([textureAsset [, wrapModeHorizontal [, wrapModeVertical [, filterMode]]]]) - Sets the texture to an image.
    /// </summary>
    /// <param name="fileID"></param>
    /// <param name="wrapModeHorizontal"></param>
    /// <param name="wrapModeVertical"></param>
    /// <param name="filterMode"></param>
    private void SetTexture(int fileID, string? wrapModeHorizontal, string? wrapModeVertical, string? filterMode)
    {

    }
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_TextureBase_SetTexture
    /// TextureBase:SetTexture([textureAsset [, wrapModeHorizontal [, wrapModeVertical [, filterMode]]]]) - Sets the texture to an image.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="wrapModeHorizontal"></param>
    /// <param name="wrapModeVertical"></param>
    /// <param name="filterMode"></param>
    private void SetTexture(string? filePath, string? wrapModeHorizontal, string? wrapModeVertical, string? filterMode)
    {

    }
    public int internal_SetTexture(lua_State L)
    {
        var textureBase = GetThis(L, 1) as TextureBase;

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
    
    // TextureBase:SetTextureSliceMargins(left, top, right, bottom)
    // TextureBase:SetTextureSliceMode(sliceMode)
    // TextureBase:SetVertTile([tiling]) - Sets whether the texture should tile vertically.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_TextureBase_SetVertexOffset
    /// TextureBase:SetVertexOffset(vertexIndex, offsetX, offsetY) - Sets a vertex offset for the texture.
    /// </summary>
    /// <param name="vertexIndex"></param>
    /// <param name="offsetX"></param>
    /// <param name="offsetY"></param>
    private void SetVertexOffset(int vertexIndex, float offsetX, float offsetY)
    {
    }
    private int internal_SetVertexOffset(lua_State L)
    {
        var textureBase = GetThis(L, 1) as TextureBase;

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
    
    
    // ----------- Virtual Registration ---------------
    
    public override string GetMetatableName() => "TextureBaseMetaTable";
        
    public override void RegisterMetaTable(lua_State L)
    {
        // 1) call base to register the "TextureBaseMetaTable"
        base.RegisterMetaTable(L);

        // 2) Now define "TextureBaseMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = TextureBaseMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("RegionMetaTable")
        var baseMetaName = base.GetMetatableName();
        luaL_getmetatable(L, baseMetaName);
        lua_setmetatable(L, -2); // Sets TextureBaseMetaTable's metatable to RegionMetaTable
        
        // 5) Bind Frame-specific methods
        LuaHelpers.RegisterMethod(L, "SetColorTexture", internal_SetColorTexture);
        LuaHelpers.RegisterMethod(L, "SetVertexOffset", internal_SetVertexOffset);
        LuaHelpers.RegisterMethod(L, "SetTexture", internal_SetTexture);
        LuaHelpers.RegisterMethod(L, "SetTexCoord", internal_SetTexCoord);
        LuaHelpers.RegisterMethod(L, "SetRotation", internal_SetRotation);

        // Optional __gc
        lua_pushcfunction(L, internal_ObjectGC);
        lua_setfield(L, -2, "__gc");

        // 5) pop
        lua_pop(L, 1);
    }
    
    public override int internal_ObjectGC(lua_State L)
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