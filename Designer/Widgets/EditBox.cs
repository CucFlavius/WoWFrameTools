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
    private void SetAutoFocus(bool autoFocus)
    {
        
    }
    private int internal_SetAutoFocus(lua_State L)
    {
        var frame = GetThis(L, 1) as EditBox;
        var autoFocus = lua_toboolean(L, 2) != 0;

        frame?.SetAutoFocus(autoFocus);

        return 0;
    }
    
    // EditBox:SetBlinkSpeed(cursorBlinkSpeedSec)
    // EditBox:SetCountInvisibleLetters([countInvisibleLetters])
    // EditBox:SetCursorPosition(cursorPosition) - Sets the cursor position in the editbox.
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_EditBox_SetEnabled
    /// EditBox:SetEnabled([enabled])
    /// </summary>
    /// <param name="enabled"></param>
    private void SetEnabled(bool enabled)
    {
        
    }
    private int internal_SetEnabled(lua_State L)
    {
        var frame = GetThis(L, 1) as EditBox;
        var enabled = lua_toboolean(L, 2) != 0;

        frame?.SetEnabled(enabled);

        return 0;
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
    private void SetMaxLetters(int maxLetters)
    {
        
    }
    private int internal_SetMaxLetters(lua_State L)
    {
        var frame = GetThis(L, 1) as EditBox;
        var maxLetters = (int)lua_tonumber(L, 2);

        frame?.SetMaxLetters(maxLetters);

        return 0;
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
    private void SetText(string text)
    {
        
    }
    private int internal_SetText(lua_State L)
    {
        try
        {
            var fontString = GetThis(L, 1) as EditBox;
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
    private int internal_SetFont(lua_State L)
    {
        var editBox = GetThis(L, 1) as EditBox;
        
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
    private int internal_SetJustifyH(lua_State L)
    {
        try
        {
            var fontString = GetThis(L, 1) as EditBox;
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
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_FontInstance_SetJustifyV
    /// FontInstance:SetJustifyV(justifyV) - Sets the vertical text justification.
    /// </summary>
    /// <param name="justify"></param>
    public void SetJustifyV(string justify)
    {
    }
    private int internal_SetJustifyV(lua_State L)
    {
        try
        {
            var fontString = GetThis(L, 1) as EditBox;
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
    public int internal_SetTextColor(lua_State L)
    {
        var fontString = GetThis(L, 1) as EditBox;

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
    
    // ----------- Virtual Registration ---------------
    
    public override string GetMetatableName() => "EditBoxMetaTable";
        
    public override void RegisterMetaTable(lua_State L)
    {
        // 1) call base to register the "EditBoxMetaTable"
        base.RegisterMetaTable(L);

        // 2) Now define "EditBoxMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = EditBoxMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("FrameMetaTable")
        var baseMetaName = base.GetMetatableName();
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