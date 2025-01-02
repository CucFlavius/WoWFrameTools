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
    private float GetStringWidth()
    {
        return 1.0f;
    }
    private int internal_GetStringWidth(lua_State L)
    {
        var fontString = GetThis(L, 1) as FontString;

        var width = fontString?.GetStringWidth() ?? 1.0f;

        lua_pushnumber(L, width);
        return 1;
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
    private void SetText(string text)
    {
        
    }
    public int internal_SetText(lua_State L)
    {
        try
        {
            var fontString = GetThis(L, 1) as FontString;
            if (fontString == null)
            {
                Log.ErrorL(L, "SetText: Invalid FontString object.");
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
    public int internal_SetFont(lua_State L)
    {
        var fontString = GetThis(L, 1) as FontString;

        var argc = lua_gettop(L);
        if (argc < 4)
        {
            Log.ErrorL(L, "SetFont requires exactly 3 arguments: fontFile, height, flags.");
            return 0; // Unreachable
        }

        var fontFile = lua_tostring(L, 2);
        var height = (int)lua_tonumber(L, 3);
        var flags = lua_tostring(L, 4);

        var success = fontString?.SetFont(fontFile, height, flags);

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
    public int internal_SetJustifyH(lua_State L)
    {
        try
        {
            var fontString = GetThis(L, 1) as FontString;
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
    public int internal_SetJustifyV(lua_State L)
    {
        try
        {
            var fontString = GetThis(L, 1) as FontString;
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
        var fontString = GetThis(L, 1) as FontString;

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
    
    public override string ToString()
    {
        return $"FontString: {GetName() ?? "nil"} - {_drawLayer ?? "nil"}";
    }
    
    // ----------- Virtual Registration ---------------
    
    public override string GetMetatableName() => "FontStringMetaTable";
        
    public override void RegisterMetaTable(lua_State L)
    {
        // 1) call base to register the "FontStringMetaTable"
        base.RegisterMetaTable(L);

        // 2) Now define "FontStringMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = FontStringMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("RegionMetaTable")
        var baseMetaName = base.GetMetatableName();
        luaL_getmetatable(L, baseMetaName);
        lua_setmetatable(L, -2); // Sets FontStringMetaTable's metatable to RegionMetaTable
        
        // 5) Bind Frame-specific methods
        LuaHelpers.RegisterMethod(L, "SetText", internal_SetText);
        LuaHelpers.RegisterMethod(L, "GetStringWidth", internal_GetStringWidth);
        
        // IFontInstance
        LuaHelpers.RegisterMethod(L, "SetFont", internal_SetFont);
        LuaHelpers.RegisterMethod(L, "SetJustifyH", internal_SetJustifyH);
        LuaHelpers.RegisterMethod(L, "SetJustifyV", internal_SetJustifyV);
        LuaHelpers.RegisterMethod(L, "SetTextColor", internal_SetTextColor);

        // Optional __gc
        lua_pushcfunction(L, internal_ObjectGC);
        lua_setfield(L, -2, "__gc");

        // 6) pop
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
        if (API.UIObjects._fontStringRegistry.TryGetValue(frameUserdataPtr, out var frame))
        {
            // Free the GCHandle
            if (frame.Handle.IsAllocated)
            {
                frame.Handle.Free();
            }

            // Remove from registry
            API.UIObjects._fontStringRegistry.Remove(frameUserdataPtr);

            // Perform any additional cleanup if necessary
            // Example: frame.Dispose();
        }

        return 0;
    }
}