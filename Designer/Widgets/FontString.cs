using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.Widgets;

/// <summary>
/// https://warcraft.wiki.gg/wiki/UIOBJECT_FontString
/// </summary>
public class FontString : Region, IFontInstance
{
    public FontString(string? name = null, string? drawLayer = null, string? templateName = null, Frame? parent = null)
        : base("FontString", name, drawLayer, parent)
    {
    }
    
    // FontString:CalculateScreenAreaFromCharacterSpan(leftIndex, rightIndex) : areas
    // FontString:CanNonSpaceWrap() : wrap
    // FontString:CanWordWrap() : wrap
    // FontString:FindCharacterIndexAtCoordinate(x, y) : characterIndex, inside
    // FontString:GetFieldSize() : fieldSize
    // FontString:GetLineHeight() : lineHeight
    // FontString:GetMaxLines() : maxLines
    // FontString:GetNumLines() : numLines
    // FontString:GetRotation() : radians
    // FontString:GetStringHeight() : height
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_FontString_GetStringWidth
    /// FontString:GetStringWidth() : width
    /// </summary>
    /// <returns></returns>
    public float GetStringWidth()
    {
        return 1.0f;
    }
    
    // FontString:GetText() : text
    // FontString:GetTextScale() : textScale
    // FontString:GetUnboundedStringWidth() : width
    // FontString:GetWrappedWidth() : width
    // FontString:IsTruncated() : isTruncated
    // FontString:SetAlphaGradient(start, length) : isWithinText
    // FontString:SetFixedColor(fixedColor)
    // FontString:SetFormattedText(text)
    // FontString:SetMaxLines(maxLines)
    // FontString:SetNonSpaceWrap(wrap)
    // FontString:SetRotation(radians)
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_FontString_SetText
    /// FontString:SetText([text])
    /// </summary>
    /// <param name="text"></param>
    public void SetText(string text)
    {
        
    }
    
    // FontString:SetTextHeight(height)
    // FontString:SetTextScale(textScale)
    // FontString:SetTextToFit([text])
    // FontString:SetWordWrap(wrap)
    
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
    
    public override string ToString()
    {
        return $"FontString: {GetName() ?? "nil"} - {_drawLayer ?? "nil"}";
    }
}