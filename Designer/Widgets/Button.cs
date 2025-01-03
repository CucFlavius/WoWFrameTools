using LuaNET.Lua51;
using WoWFrameTools.API;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets;

public class Button : Frame
{
    public Button(string? name = null, Frame? parent = null, string? template = null, int id = 0)
        : base("Button", name, parent, template, id)
    {
        
    }
    
    // Button:ClearDisabledTexture()
    // Button:ClearHighlightTexture()
    // Button:ClearNormalTexture()
    // Button:ClearPushedTexture()
    // Button:Click([button, isDown]) - Performs a virtual mouse click on the button.
    // Button:Disable()
    // Button:Enable()
    // Button:GetButtonState() : buttonState
    // Button:GetDisabledFontObject() : font
    // Button:GetDisabledTexture() : texture
    // Button:GetFontString() : fontString
    // Button:GetHighlightFontObject() : font
    // Button:GetHighlightTexture() : texture - Returns the highlight texture for the button.
    // Button:GetMotionScriptsWhileDisabled() : motionScriptsWhileDisabled
    // Button:GetNormalFontObject() : font - Returns the font object for the button's normal state.
    // Button:GetNormalTexture() : texture
    // Button:GetPushedTextOffset() : offsetX, offsetY
    // Button:GetPushedTexture() : texture
    // Button:GetText() : text - Returns the text on the button.
    // Button:GetTextHeight() : height
    // Button:GetTextWidth() : width
    // Button:IsEnabled() : isEnabled - Returns true if the button is enabled.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Button_RegisterForClicks
    /// Button:RegisterForClicks([button1, ...]) - Registers the button widget to receive OnClick script events.
    /// </summary>
    /// <param name="toArray"></param>
    public void RegisterForClicks(string[] toArray)
    {
        
    }
    
    // Button:RegisterForMouse([button1, ...]) - Registers the button widget to receive OnMouse script events.
    // Button:SetButtonState(buttonState [, lock])
    // Button:SetDisabledAtlas(atlas)
    // Button:SetDisabledFontObject(font)
    // Button:SetDisabledTexture(asset)
    // Button:SetEnabled([enabled])
    // Button:SetFontString(fontString)
    // Button:SetFormattedText(text)
    // Button:SetHighlightAtlas(atlas [, blendMode])
    // Button:SetHighlightFontObject(font)
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Button_SetHighlightTexture
    /// Button:SetHighlightTexture(asset [, blendMode])
    /// </summary>
    /// <param name="asset"></param>
    /// <param name="blendMode"></param>
    public void SetHighlightTexture(Texture asset, string? blendMode)
    {
    }
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Button_SetHighlightTexture
    /// Button:SetHighlightTexture(fileID [, blendMode])
    /// </summary>
    /// <param name="fileID"></param>
    /// <param name="blendMode"></param>
    public void SetHighlightTexture(int fileID, string? blendMode)
    {
    }
    
    // Button:SetMotionScriptsWhileDisabled(motionScriptsWhileDisabled)
    // Button:SetNormalAtlas(atlas)
    // Button:SetNormalFontObject(font) - Sets the font object used for the button's normal state.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Button_SetNormalTexture
    /// Button:SetNormalTexture(asset) - Sets the normal texture for a button.
    /// </summary>
    /// <param name="asset"></param>
    /// <param name="blendMode"></param>
    public void SetNormalTexture(Texture asset, string? blendMode)
    {
    }
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Button_SetNormalTexture
    /// Button:SetNormalTexture(fileID) - Sets the normal texture for a button.
    /// </summary>
    /// <param name="fileID"></param>
    /// <param name="blendMode"></param>
    public void SetNormalTexture(int fileID, string? blendMode)
    {
    }
    
    // Button:SetPushedAtlas(atlas)
    // Button:SetPushedTextOffset(offsetX, offsetY)
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Button_SetPushedTexture
    /// Button:SetPushedTexture(asset)
    /// </summary>
    /// <param name="asset"></param>
    /// <param name="blendMode"></param>
    public void SetPushedTexture(Texture asset, string? blendMode)
    {
    }
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Button_SetPushedTexture
    /// Button:SetPushedTexture(fileID)
    /// </summary>
    /// <param name="fileID"></param>
    /// <param name="blendMode"></param>
    public void SetPushedTexture(int fileID, string? blendMode)
    {
    }
    
    // Button:SetText([text]) - Sets the text of the button.
}