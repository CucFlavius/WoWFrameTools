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
    private void RegisterForClicks(string[] toArray)
    {
        
    }
    public int internal_RegisterForClicks(lua_State L)
    {
        var frame = GetThis(L, 1) as Button;

        var argc = lua_gettop(L);
        if (argc < 2)
        {
            Log.ErrorL(L, "RegisterForClicks requires at least 1 argument: button1.");
            return 0; // Unreachable
        }

        List<string> buttons = new();
        for (var i = 2; i < argc; i++)
        {
            if (lua_isstring(L, i) != 0)
            {
                var button = lua_tostring(L, i);
                buttons.Add(button);
            }
            else
            {
                throw new ArgumentException($"Argument {i} is not a valid string.");
            }
        }

        frame?.RegisterForClicks(buttons.ToArray());

        lua_pushboolean(L, 1);
        return 1;
        
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
    private void SetHighlightTexture(Texture asset, string? blendMode)
    {
    }
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Button_SetHighlightTexture
    /// Button:SetHighlightTexture(fileID [, blendMode])
    /// </summary>
    /// <param name="fileID"></param>
    /// <param name="blendMode"></param>
    private void SetHighlightTexture(int fileID, string? blendMode)
    {
    }
    private int internal_SetHighlightTexture(lua_State L)
    {
        var frame = GetThis(L, 1) as Button;
        string? blendMode = null;
        
        if (lua_gettop(L) >= 3)
        {
            blendMode = lua_tostring(L, 3);
        }
        
        if (lua_istable(L, 2) != 0)
        {
            var texture = GetThis(L, 2) as Texture;
            if (texture == null)
                return 0;
            frame?.SetHighlightTexture(texture, blendMode);
        }
        else if (lua_isnumber(L, 2) != 0)
        {
            var fileID = (int)lua_tonumber(L, 2);
            frame?.SetHighlightTexture(fileID, blendMode);
        }
        
        return 0;
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
    private void SetNormalTexture(Texture asset, string? blendMode)
    {
    }
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Button_SetNormalTexture
    /// Button:SetNormalTexture(fileID) - Sets the normal texture for a button.
    /// </summary>
    /// <param name="fileID"></param>
    /// <param name="blendMode"></param>
    private void SetNormalTexture(int fileID, string? blendMode)
    {
    }
    private int internal_SetNormalTexture(lua_State L)
    {
        var frame = GetThis(L, 1) as Button;
        string? blendMode = null;
        
        if (lua_gettop(L) >= 3)
        {
            blendMode = lua_tostring(L, 3);
        }
        
        if (lua_istable(L, 2) != 0)
        {
            var texture = GetThis(L, 2) as Texture;
            if (texture == null)
                return 0;
            frame?.SetNormalTexture(texture, blendMode);
        }
        else if (lua_isnumber(L, 2) != 0)
        {
            var fileID = (int)lua_tonumber(L, 2);
            frame?.SetNormalTexture(fileID, blendMode);
        }
        
        return 0;
    }
    
    // Button:SetPushedAtlas(atlas)
    // Button:SetPushedTextOffset(offsetX, offsetY)
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Button_SetPushedTexture
    /// Button:SetPushedTexture(asset)
    /// </summary>
    /// <param name="asset"></param>
    /// <param name="blendMode"></param>
    private void SetPushedTexture(Texture asset, string? blendMode)
    {
    }
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_Button_SetPushedTexture
    /// Button:SetPushedTexture(fileID)
    /// </summary>
    /// <param name="fileID"></param>
    /// <param name="blendMode"></param>
    private void SetPushedTexture(int fileID, string? blendMode)
    {
    }
    private int internal_SetPushedTexture(lua_State L)
    {
        var frame = GetThis(L, 1) as Button;
        string? blendMode = null;
        
        if (lua_gettop(L) >= 3)
        {
            blendMode = lua_tostring(L, 3);
        }
        
        if (lua_istable(L, 2) != 0)
        {
            var texture = GetThis(L, 2) as Texture;
            if (texture == null)
                return 0;
            frame?.SetPushedTexture(texture, blendMode);
        }
        else if (lua_isnumber(L, 2) != 0)
        {
            var fileID = (int)lua_tonumber(L, 2);
            frame?.SetPushedTexture(fileID, blendMode);
        }
        
        return 0;
    }
    
    // Button:SetText([text]) - Sets the text of the button.
    
    // ----------- Virtual Registration ---------------
    
    public override string GetMetatableName() => "ButtonMetaTable";
        
    public override void RegisterMetaTable(lua_State L)
    {
        // 1) call base to register the "ButtonMetaTable"
        base.RegisterMetaTable(L);

        // 2) Now define "ButtonMetaTable"
        var metaName = GetMetatableName();
        luaL_newmetatable(L, metaName);

        // 3) __index = ButtonMetaTable
        lua_pushvalue(L, -1);
        lua_setfield(L, -2, "__index");

        // 4) Link to the base class's metatable ("FrameMetaTable")
        var baseMetaName = base.GetMetatableName();
        luaL_getmetatable(L, baseMetaName);
        lua_setmetatable(L, -2); // Sets ButtonMetaTable's metatable to FrameMetaTable
        
        // 5) Bind Frame-specific methods
        LuaHelpers.RegisterMethod(L, "SetHighlightTexture", internal_SetHighlightTexture);
        LuaHelpers.RegisterMethod(L, "SetNormalTexture", internal_SetNormalTexture);
        LuaHelpers.RegisterMethod(L, "SetPushedTexture", internal_SetPushedTexture);
        LuaHelpers.RegisterMethod(L, "RegisterForClicks", internal_RegisterForClicks);
        
        // 6) pop
        lua_pop(L, 1);
    }
}