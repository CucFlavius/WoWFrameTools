using LuaNET.Lua51;
using WoWFrameTools.API;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets;

public class EditBox : Frame, IFontInstance
{
    public EditBox(string? name = null, Frame? parent = null, string? template = null, int id = 0)
        : base("EditBox", name, parent, template, id)
    {
    }
    
    // EditBox:AddHistoryLine(text) - Adds text to the edit history.
    // EditBox:ClearFocus() - Removes text input focus from this editbox element.
    // EditBox:ClearHighlightText()
    // EditBox:ClearHistory()
    // EditBox:Disable()
    // EditBox:Enable()
    // EditBox:GetAltArrowKeyMode() : altMode
    // EditBox:GetBlinkSpeed() : cursorBlinkSpeedSec - Returns the blink speed.
    // EditBox:GetCursorPosition() : cursorPosition - Returns the cursor position in the editbox.
    // EditBox:GetDisplayText() : displayText
    // EditBox:GetHighlightColor() : colorR, colorG, colorB, colorA
    // EditBox:GetHistoryLines() : numHistoryLines - Returns the number of history lines for this editbox.
    // EditBox:GetInputLanguage() : language - Returns the input language (locale based not in-game).
    // EditBox:GetMaxBytes() : maxBytes
    // EditBox:GetMaxLetters() : maxLetters
    // EditBox:GetNumLetters() : numLetters - Returns the number of letters in the editbox.
    // EditBox:GetNumber() : number - Returns the number entered in the editbox, or 0 if editbox text is not a number.
    // EditBox:GetText() : text - Returns the current text contained in the edit box.
    // EditBox:GetTextInsets() : left, right, top, bottom - Returns a list of left,right,top,bottom text insets.
    // EditBox:GetUTF8CursorPosition() : cursorPosition
    // EditBox:GetVisibleTextByteLimit() : maxVisibleBytes
    // EditBox:HasFocus() : hasFocus - Returns true if the edit box has the focus.
    // EditBox:HasText() : hasText
    // EditBox:HighlightText([start, stop]) - Highlights all or some of the edit box text.
    // EditBox:Insert(text) - Insert text into the editbox.
    // EditBox:IsAlphabeticOnly() : enabled
    // EditBox:IsAutoFocus() : autoFocus
    // EditBox:IsCountInvisibleLetters() : countInvisibleLetters
    // EditBox:IsEnabled() : isEnabled
    // EditBox:IsInIMECompositionMode() : isInIMECompositionMode
    // EditBox:IsMultiLine() : multiline
    // EditBox:IsNumeric() : isNumeric
    // EditBox:IsNumericFullRange() : isNumeric
    // EditBox:IsPassword() : isPassword
    // EditBox:IsSecureText() : isSecure
    // EditBox:ResetInputMode()
    // EditBox:SetAlphabeticOnly([enabled])
    // EditBox:SetAltArrowKeyMode([altMode])
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_EditBox_SetAutoFocus
    /// EditBox:SetAutoFocus([autoFocus])
    /// </summary>
    /// <param name="autoFocus"></param>
    public void SetAutoFocus(bool autoFocus)
    {
        
    }
    
    // EditBox:SetBlinkSpeed(cursorBlinkSpeedSec)
    // EditBox:SetCountInvisibleLetters([countInvisibleLetters])
    // EditBox:SetCursorPosition(cursorPosition) - Sets the cursor position in the editbox.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_EditBox_SetEnabled
    /// EditBox:SetEnabled([enabled])
    /// </summary>
    /// <param name="enabled"></param>
    public void SetEnabled(bool enabled)
    {
        
    }
    
    // EditBox:SetFocus()
    // EditBox:SetHighlightColor(colorR, colorG, colorB [, a])
    // EditBox:SetHistoryLines(numHistoryLines) - Sets the number of history lines to remember.
    // EditBox:SetMaxBytes(maxBytes) - Sets the maximum byte size for entered text.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_EditBox_SetMaxLetters
    /// EditBox:SetMaxLetters(maxLetters) - Sets the maximum number of letters for entered text.
    /// </summary>
    /// <param name="maxLetters"></param>
    public void SetMaxLetters(int maxLetters)
    {
        
    }
    
    // EditBox:SetMultiLine([multiline])
    // EditBox:SetNumber(number)
    // EditBox:SetNumeric([isNumeric])
    // EditBox:SetNumericFullRange([isNumeric])
    // EditBox:SetPassword([isPassword])
    // EditBox:SetSecureText([isSecure])
    // EditBox:SetSecurityDisablePaste()
    // EditBox:SetSecurityDisableSetText()
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_EditBox_SetText
    /// EditBox:SetText(text)  - Sets the text contained in the edit box.
    /// </summary>
    /// <param name="text"></param>
    public void SetText(string text)
    {
        
    }
    
    // EditBox:SetTextInsets(left, right, top, bottom)
    // EditBox:SetVisibleTextByteLimit(maxVisibleBytes)
    // EditBox:ToggleInputLanguage()

    // IFontInstance //
    // FontInstance:GetFont() : fontFile, height, flags - Returns the font path, height, and flags.
    // FontInstance:GetFontObject() : font - Returns the "parent" font object.
    // FontInstance:GetIndentedWordWrap() : wordWrap - Returns the indentation when text wraps beyond the first line.
    // FontInstance:GetJustifyH() : justifyH - Returns the horizontal text justification.
    // FontInstance:GetJustifyV() : justifyV - Returns the vertical text justification.
    // FontInstance:GetShadowColor() : colorR, colorG, colorB, colorA - Sets the text shadow color.
    // FontInstance:GetShadowOffset() : offsetX, offsetY - Returns the text shadow offset.
    // FontInstance:GetSpacing() : spacing - Returns the line spacing.
    // FontInstance:GetTextColor() : colorR, colorG, colorB, colorA - Returns the default text color.

    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_FontInstance_SetFont
    /// FontInstance:SetFont(fontFile, height, flags) - Sets the basic font properties.
    /// </summary>
    /// <param name="fontFile"></param>
    /// <param name="height"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public bool SetFont(string? fontFile, int height, string? flags)
    {
        return true;
    }

    // FontInstance:SetFontObject(font) - Sets the "parent" font object from which this object inherits properties.
    // FontInstance:SetIndentedWordWrap(wordWrap) - Sets the indentation when text wraps beyond the first line.

    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_FontInstance_SetJustifyH
    /// FontInstance:SetJustifyH(justifyH) - Sets the horizontal text justification
    /// </summary>
    /// <param name="justify"></param>
    public void SetJustifyH(string justify)
    {
        
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_FontInstance_SetJustifyV
    /// FontInstance:SetJustifyV(justifyV) - Sets the vertical text justification.
    /// </summary>
    /// <param name="justify"></param>
    public void SetJustifyV(string justify)
    {
    }
    
    // FontInstance:SetShadowColor(colorR, colorG, colorB [, a]) - Returns the color of text shadow.
    // FontInstance:SetShadowOffset(offsetX, offsetY) - Sets the text shadow offset.
    // FontInstance:SetSpacing(spacing) - Sets the spacing between lines of text in the object.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_FontInstance_SetTextColor
    /// FontInstance:SetTextColor(colorR, colorG, colorB [, a]) - Sets the default text color.
    /// </summary>
    /// <param name="colorR"></param>
    /// <param name="colorG"></param>
    /// <param name="colorB"></param>
    /// <param name="colorA"></param>
    public void SetTextColor(float colorR, float colorG, float colorB, float colorA)
    {
    }
}