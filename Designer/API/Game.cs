using System.Globalization;
using System.Runtime.InteropServices;
using LuaNET.Lua51;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools.API;

public static class Game
{
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_GetTime
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int GetTime(lua_State L)
    {
        // Get the current time in Unix epoch format (seconds since 1970-01-01)
        var now = DateTimeOffset.UtcNow;
        var unixTime = now.ToUnixTimeSeconds();

        // Push the Unix time as a number onto the Lua stack
        lua_pushnumber(L, unixTime);

        // Return the number of return values
        return 1;
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_GetLocale
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int GetLocale(lua_State L)
    {
        try
        {
            // Retrieve the locale from the system or configuration
            var locale = CultureInfo.CurrentCulture.Name;

            // Push the locale string onto the Lua stack
            lua_pushstring(L, locale);

            // Log the retrieval
            //AnsiConsole.WriteLine($"GetLocale called. Returning locale: {locale}");

            // Return the number of return values
            return 1;
        }
        catch (Exception ex)
        {
            // Log the exception
            Log.Exception(ex);

            // Raise a Lua error
            Log.ErrorL(L, "GetLocale encountered an error.");
            return 0; // Unreachable
        }
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_SendChatMessage
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int SendChatMessage(lua_State L)
    {
        // Ensure there are at least 2 arguments: message and chatType
        var argc = lua_gettop(L);
        if (argc < 2)
        {
            Log.ErrorL(L, "SendChatMessage requires at least 2 arguments: message and chatType.");
            return 0; // Unreachable
        }

        // Retrieve arguments from Lua
        var message = lua_tostring(L, 1);
        var chatType = lua_tostring(L, 2);
        var language = argc >= 3 ? lua_tostring(L, 3) : "Common"; // Default language
        var target = argc >= 4 ? lua_tostring(L, 4) : "";

        // Validate required parameters
        if (string.IsNullOrEmpty(message))
        {
            Log.ErrorL(L, "SendChatMessage: 'message' cannot be empty.");
            return 0; // Unreachable
        }

        if (string.IsNullOrEmpty(chatType))
        {
            Log.ErrorL(L, "SendChatMessage: 'chatType' cannot be empty.");
            return 0; // Unreachable
        }

        // Define chat types that require a target
        HashSet<string> chatTypesRequiringTarget = new() { "WHISPER", "TARGET", "CHANNEL" };
        if (chatTypesRequiringTarget.Contains(chatType.ToUpper()) && string.IsNullOrEmpty(target))
        {
            Log.ErrorL(L, $"SendChatMessage: 'target' is required for chatType '{chatType}'.");
            return 0; // Unreachable
        }

        // Validate chatType
        HashSet<string> validChatTypes = new() { "SAY", "YELL", "PARTY", "GUILD", "WHISPER", "CHANNEL" };
        if (!validChatTypes.Contains(chatType.ToUpper()))
        {
            Log.ErrorL(L, $"SendChatMessage: Unsupported chatType '{chatType}'.");
            return 0; // Unreachable
        }

        Log.ChatMessage(message, chatType, language, target);

        // Optionally, implement actual message sending logic here
        // For example, integrating with your application's messaging system

        // SendChatMessage does not return any value in WoW API
        return 0;
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_C_ChatInfo.SendAddonMessage
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int SendAddonMessage(lua_State L)
    {
        // Ensure there are at least 3 arguments: prefix, message, channel
        var argc = lua_gettop(L);
        if (argc < 3)
        {
            Log.ErrorL(L, "SendAddonMessage requires at least 3 arguments: prefix, message, and channel.");
            return 0; // Unreachable
        }

        // Retrieve arguments from Lua
        var prefix = lua_tostring(L, 1);
        var message = lua_tostring(L, 2);
        var channel = lua_tostring(L, 3);
        var target = argc >= 4 ? lua_tostring(L, 4) : "";

        // Validate required parameters
        if (string.IsNullOrEmpty(prefix))
        {
            Log.ErrorL(L, "SendAddonMessage: 'prefix' cannot be empty.");
            return 0; // Unreachable
        }

        if (string.IsNullOrEmpty(message))
        {
            Log.ErrorL(L, "SendAddonMessage: 'message' cannot be empty.");
            return 0; // Unreachable
        }

        if (string.IsNullOrEmpty(channel))
        {
            Log.ErrorL(L, "SendAddonMessage: 'channel' cannot be empty.");
            return 0; // Unreachable
        }

        // Define chat types that require a target
        HashSet<string> chatTypesRequiringTarget = new() { "WHISPER", "TARGET", "CHANNEL" };
        if (chatTypesRequiringTarget.Contains(channel.ToUpper()) && string.IsNullOrEmpty(target))
        {
            Log.ErrorL(L, $"SendAddonMessage: 'target' is required for chatType '{channel}'.");
            return 0; // Unreachable
        }

        // Validate chatType
        HashSet<string> validChatTypes = new() { "SAY", "YELL", "PARTY", "GUILD", "WHISPER", "CHANNEL" };
        if (!validChatTypes.Contains(channel.ToUpper()))
        {
            Log.ErrorL(L, $"SendAddonMessage: Unsupported chatType '{channel}'.");
            return 0; // Unreachable
        }

        Log.AddonMessage(message, prefix, channel, target);

        // Optionally, implement actual addon message sending logic here
        // For example, integrating with your application's networking or messaging system

        // SendAddonMessage does not return any value in WoW API
        return 0;
    }
    
    /// <summary>
    /// https://warcraft.wiki.gg/wiki/API_UnitName
    /// name, realm = UnitName(unit)
    /// </summary>
    /// <param name="L"></param>
    /// <returns></returns>
    public static int UnitName(lua_State L)
    {
        // Ensure there is exactly 1 argument: unit
        var argc = lua_gettop(L);
        if (argc != 1)
        {
            Log.ErrorL(L, "UnitName requires exactly 1 argument: unit.");
            return 0; // Unreachable
        }

        // Retrieve the unit from Lua
        var unit = lua_tostring(L, 1);

        // Validate required parameters
        if (string.IsNullOrEmpty(unit))
        {
            Log.ErrorL(L, "UnitName: 'unit' cannot be empty.");
            return 0; // Unreachable
        }

        // Optionally, implement actual unit name retrieval logic here
        // For example, querying a database or game API

        // Push the unit name onto the Lua stack
        lua_pushstring(L, "PlayerName");

        // Push the realm name onto the Lua stack
        lua_pushstring(L, "PlayerRealm");

        // Return the number of return values
        return 2;
    }
}