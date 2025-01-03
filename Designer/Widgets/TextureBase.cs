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
    public void SetColorTexture(float colorR, float colorG, float colorB, float colorA)
    {
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
    public void SetRotation(float radians, string? normalizedRotationPoint)
    {
        
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
    public void SetTexCoord(float left, float right, float top, float bottom)
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
    public void SetTexCoord(float uLx, float uLy, float lLx, float lLy, float uRx, float uRy, float lRx, float lRy)
    {
 
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
    public void SetTexture(int fileID, string? wrapModeHorizontal, string? wrapModeVertical, string? filterMode)
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
    public void SetTexture(string? filePath, string? wrapModeHorizontal, string? wrapModeVertical, string? filterMode)
    {

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
    public void SetVertexOffset(int vertexIndex, float offsetX, float offsetY)
    {
    }
}