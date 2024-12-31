using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using LuaNET.Lua51;
using Spectre.Console;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools;

public partial class Global
{
    public static void RegisterBindings(lua_State L)
    {
        // Register the CreateFrame function
        lua_pushcfunction(L, internal_CreateFrame);
        lua_setglobal(L, "CreateFrame");

        // Register the GetTime function
        lua_pushcfunction(L, internal_GetTime);
        lua_setglobal(L, "GetTime");

        // Register the GetCVar function
        lua_pushcfunction(L, internal_GetCVar);
        lua_setglobal(L, "GetCVar");

        // Register the SendChatMessage function
        lua_pushcfunction(L, internal_SendChatMessage);
        lua_setglobal(L, "SendChatMessage");

        // Register the SendAddonMessage function as a global Lua function
        lua_pushcfunction(L, internal_SendAddonMessage);
        lua_setglobal(L, "SendAddonMessage");

        // Register the GetLocale function as a global Lua function
        lua_pushcfunction(L, internal_GetLocale);
        lua_setglobal(L, "GetLocale");

        // Register IsLoggedIn function as a global Lua function
        lua_pushcfunction(L, internal_IsLoggedIn);
        lua_setglobal(L, "IsLoggedIn");

        // Initialize Minimap and set it as a global variable
        InitializeMinimap(L);

        // Initialize SlashCmdList
        InitializeSlashCmdList(L);

        // Register the AddSlashCommand function as a global Lua function
        lua_pushcfunction(L, internal_AddSlashCommand);
        lua_setglobal(L, "AddSlashCommand");

        lua_pushcfunction(L, internal_Print);
        lua_setglobal(L, "print");

        RegisterC_AddOns(L);
        RegisterC_ChatInfo(L);
    }

    public static int internal_CreateFrame(lua_State L)
    {
        // Retrieve arguments from Lua (frameType, name, parent, template, id)
        var frameType = lua_tostring(L, 1) ?? "Frame";
        var name = lua_tostring(L, 2) ?? "";

        // Handle the parent frame (if provided)
        Frame? parentFrame = null;
        if (lua_isnil(L, 3) != 0)
        {
            var parentUserdataPtr = (IntPtr)lua_touserdata(L, 3);
            if (parentUserdataPtr != IntPtr.Zero)
            {
                if (Frame._frameRegistry.TryGetValue(parentUserdataPtr, out var foundFrame))
                    parentFrame = foundFrame;
                else
                    throw new ArgumentException("Invalid parent frame specified.");
            }
            // TODO: Set to UIParent if parent is nil
            // parentFrame = UIParent.Instance; // Example
        }

        var template = lua_tostring(L, 4) ?? "";
        var id = LuaHelpers.IsNumber(L, 5) ? (int)lua_tonumber(L, 5) : 0;

        // Create a new Frame instance
        var frame = new Frame(L, frameType, name, parentFrame, template, id);

        // Allocate a GCHandle to prevent garbage collection
        var handle = GCHandle.Alloc(frame);
        var handlePtr = GCHandle.ToIntPtr(handle);

        // Create userdata with the size of IntPtr
        var userdataPtr = (IntPtr)lua_newuserdata(L, (UIntPtr)IntPtr.Size);

        // Write the handlePtr into the userdata memory
        Marshal.WriteIntPtr(userdataPtr, handlePtr);

        // Set the metatable for the userdata
        luaL_getmetatable(L, "FrameMetaTable"); // Ensure FrameMetaTable is set up
        lua_setmetatable(L, -2);

        // Add the Frame to the registry for later retrieval
        Frame._frameRegistry[userdataPtr] = frame;

        // Assign the userdataPtr and LuaRegistryRef to the Frame instance
        frame.UserdataPtr = userdataPtr;

        // Create a reference to the userdata in the Lua registry
        lua_pushvalue(L, -1); // Push the userdata
        var refIndex = luaL_ref(L, LUA_REGISTRYINDEX);
        frame.LuaRegistryRef = refIndex;

        Log.CreateFrame(frame);

        return 1; // Number of return values (userdata is returned)
    }

    // GetTime function callable from Lua
    public static int internal_GetTime(lua_State L)
    {
        // Get the current time in Unix epoch format (seconds since 1970-01-01)
        var now = DateTimeOffset.UtcNow;
        var unixTime = now.ToUnixTimeSeconds();

        // Push the Unix time as a number onto the Lua stack
        lua_pushnumber(L, unixTime);

        // Return the number of return values
        return 1;
    }

    // GetCVar function callable from Lua
    public static int internal_GetCVar(lua_State L)
    {
        // Ensure there is exactly 1 argument: cvarName
        var argc = lua_gettop(L);
        if (argc != 1)
        {
            lua_pushstring(L, "GetCVar requires exactly 1 argument: cvarName.");
            lua_error(L);
            return 0; // Unreachable
        }

        // Retrieve the cvarName from Lua
        var cvarName = lua_tostring(L, 1);

        // Retrieve the value of the specified CVar
        string cvarValue;
        switch (cvarName)
        {
            case "gxWindowedResolution":
                cvarValue = "1024x768";
                break;
            default:
                cvarValue = string.Empty;
                AnsiConsole.MarkupLine($"[red]GetCVar: Unsupported CVar '{cvarName}'[/]");
                break;
        }

        // Push the CVar value as a string onto the Lua stack
        lua_pushstring(L, cvarValue);

        // Return the number of return values
        return 1;
    }

    // SendChatMessage function callable from Lua
    public static int internal_SendChatMessage(lua_State L)
    {
        // Ensure there are at least 2 arguments: message and chatType
        var argc = lua_gettop(L);
        if (argc < 2)
        {
            lua_pushstring(L, "SendChatMessage requires at least 2 arguments: message and chatType.");
            lua_error(L);
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
            lua_pushstring(L, "SendChatMessage: 'message' cannot be empty.");
            lua_error(L);
            return 0; // Unreachable
        }

        if (string.IsNullOrEmpty(chatType))
        {
            lua_pushstring(L, "SendChatMessage: 'chatType' cannot be empty.");
            lua_error(L);
            return 0; // Unreachable
        }

        // Define chat types that require a target
        HashSet<string> chatTypesRequiringTarget = new() { "WHISPER", "TARGET", "CHANNEL" };
        if (chatTypesRequiringTarget.Contains(chatType.ToUpper()) && string.IsNullOrEmpty(target))
        {
            lua_pushstring(L, $"SendChatMessage: 'target' is required for chatType '{chatType}'.");
            lua_error(L);
            return 0; // Unreachable
        }

        // Validate chatType
        HashSet<string> validChatTypes = new() { "SAY", "YELL", "PARTY", "GUILD", "WHISPER", "CHANNEL" };
        if (!validChatTypes.Contains(chatType.ToUpper()))
        {
            lua_pushstring(L, $"SendChatMessage: Unsupported chatType '{chatType}'.");
            lua_error(L);
            return 0; // Unreachable
        }

        Log.ChatMessage(message, chatType, language, target);

        // Optionally, implement actual message sending logic here
        // For example, integrating with your application's messaging system

        // SendChatMessage does not return any value in WoW API
        return 0;
    }

    // SendAddonMessage function callable from Lua
    public static int internal_SendAddonMessage(lua_State L)
    {
        // Ensure there are at least 3 arguments: prefix, message, channel
        var argc = lua_gettop(L);
        if (argc < 3)
        {
            lua_pushstring(L, "SendAddonMessage requires at least 3 arguments: prefix, message, and channel.");
            lua_error(L);
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
            lua_pushstring(L, "SendAddonMessage: 'prefix' cannot be empty.");
            lua_error(L);
            return 0; // Unreachable
        }

        if (string.IsNullOrEmpty(message))
        {
            lua_pushstring(L, "SendAddonMessage: 'message' cannot be empty.");
            lua_error(L);
            return 0; // Unreachable
        }

        if (string.IsNullOrEmpty(channel))
        {
            lua_pushstring(L, "SendAddonMessage: 'channel' cannot be empty.");
            lua_error(L);
            return 0; // Unreachable
        }

        // Define chat types that require a target
        HashSet<string> chatTypesRequiringTarget = new() { "WHISPER", "TARGET", "CHANNEL" };
        if (chatTypesRequiringTarget.Contains(channel.ToUpper()) && string.IsNullOrEmpty(target))
        {
            lua_pushstring(L, $"SendAddonMessage: 'target' is required for chatType '{channel}'.");
            lua_error(L);
            return 0; // Unreachable
        }

        // Validate chatType
        HashSet<string> validChatTypes = new() { "SAY", "YELL", "PARTY", "GUILD", "WHISPER", "CHANNEL" };
        if (!validChatTypes.Contains(channel.ToUpper()))
        {
            lua_pushstring(L, $"SendAddonMessage: Unsupported chatType '{channel}'.");
            lua_error(L);
            return 0; // Unreachable
        }

        Log.AddonMessage(message, prefix, channel, target);

        // Optionally, implement actual addon message sending logic here
        // For example, integrating with your application's networking or messaging system

        // SendAddonMessage does not return any value in WoW API
        return 0;
    }

    /// <summary>
    ///     Retrieves the client's locale.
    /// </summary>
    /// <param name="L">The Lua state.</param>
    /// <returns>Number of return values (1).</returns>
    public static int internal_GetLocale(lua_State L)
    {
        lock (_luaLock)
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
                AnsiConsole.WriteException(ex);

                // Raise a Lua error
                lua_pushstring(L, "GetLocale encountered an error.");
                lua_error(L);
                return 0; // Unreachable
            }
        }
    }

    // Add the following method to register a slash command
    public static int internal_AddSlashCommand(lua_State L)
    {
        lock (_luaLock)
        {
            try
            {
                // Ensure there are exactly 3 arguments: name, command, handler
                var argc = lua_gettop(L);
                if (argc != 3)
                {
                    lua_pushstring(L, "AddSlashCommand requires exactly 3 arguments: name, command, handler.");
                    lua_error(L);
                    return 0; // Unreachable
                }

                // Get the name (e.g., "HELLO")
                if (!LuaHelpers.IsString(L, 1))
                {
                    lua_pushstring(L, "AddSlashCommand: 'name' must be a string.");
                    lua_error(L);
                    return 0;
                }

                var name = lua_tostring(L, 1);

                // Get the command string (e.g., "/hello")
                if (!LuaHelpers.IsString(L, 2))
                {
                    lua_pushstring(L, "AddSlashCommand: 'command' must be a string.");
                    lua_error(L);
                    return 0;
                }

                var command = lua_tostring(L, 2);

                // Get the handler function
                if (lua_isfunction(L, 3) == 0)
                {
                    lua_pushstring(L, "AddSlashCommand: 'handler' must be a function.");
                    lua_error(L);
                    return 0;
                }

                // Create a reference to the handler function
                lua_pushvalue(L, 3);
                var refIndex = luaL_ref(L, LUA_REGISTRYINDEX);

                // Register the slash command
                // Example: SLASH_HELLO1 = "/hello"
                var slashVar = $"SLASH_{name}1";
                lua_pushstring(L, slashVar);
                lua_setglobal(L, slashVar);
                if (command != null)
                {
                    lua_pushstring(L, command);
                    lua_setglobal(L, slashVar);

                    // Set the handler in SlashCmdList
                    lua_getglobal(L, "SlashCmdList"); // Push SlashCmdList table
                    if (name != null)
                    {
                        lua_pushstring(L, name);
                        lua_rawgeti(L, LUA_REGISTRYINDEX, refIndex); // Push handler function
                        lua_settable(L, -3); // SlashCmdList[name] = handler
                        lua_pop(L, 1); // Pop SlashCmdList table

                        // Optionally, store the mapping in C# if needed
                        _slashCommands[command] = name;

                        AnsiConsole.WriteLine($"Slash command '{command}' registered with name '{name}'.");
                    }
                }

                // Push 'true' to indicate success
                lua_pushboolean(L, 1);
                return 1;
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                lua_pushstring(L, "AddSlashCommand encountered an error.");
                lua_error(L);
                return 0;
            }
        }
    }

    public static int internal_IsLoggedIn(lua_State L)
    {
        lua_pushboolean(L, 1);

        // Return the number of return values
        return 1;
    }

    public static int internal_Print(lua_State L)
    {
        var nargs = lua_gettop(L);
        var sb = new StringBuilder();
        for (var i = 1; i <= nargs; i++)
        {
            var s = lua_tostring(L, i);
            if (s != null)
            {
                sb.Append(s);
                if (i < nargs)
                    sb.Append("\t"); // Separate multiple arguments with tabs
            }
        }

        Log.Print(sb.ToString());
        return 0; // Number of return values
    }

    /// <summary>
    ///     Registers an addon message prefix from Lua.
    ///     Usage in Lua: C_ChatInfo.RegisterAddonMessagePrefix("YourPrefix")
    /// </summary>
    /// <param name="L">The Lua state.</param>
    /// <returns>Number of return values (0).</returns>
    public static int internal_RegisterAddonMessagePrefix(lua_State L)
    {
        // Check if the first argument is a string
        if (!LuaHelpers.IsString(L, 1))
        {
            lua_pushstring(L, "RegisterAddonMessagePrefix requires a string argument.");
            lua_error(L);
            return 0; // Unreachable
        }

        var prefix = lua_tostring(L, 1);

        if (string.IsNullOrEmpty(prefix))
        {
            lua_pushstring(L, "RegisterAddonMessagePrefix: prefix cannot be empty.");
            lua_error(L);
            return 0; // Unreachable
        }

        bool added;
        lock (_prefixLock)
        {
            added = _registeredPrefixes.Add(prefix);
        }

        if (added)
            AnsiConsole.MarkupLine($"[green]Registered Addon Message Prefix: '{prefix}'[/]");
        else
            AnsiConsole.MarkupLine($"[yellow]Addon Message Prefix '{prefix}' is already registered.[/]");

        return 0; // No return values
    }

    /// <summary>
    ///     Retrieves addon metadata from the TOC file.
    ///     Usage in Lua: C_AddOns.GetAddOnMetadata("AddonName", "MetadataKey")
    /// </summary>
    /// <param name="L">The Lua state.</param>
    /// <returns>Number of return values (1).</returns>
    public static int internal_GetAddOnMetadata(lua_State L)
    {
        // Ensure there are exactly 2 arguments: addonName, metadataKey
        var argc = lua_gettop(L);
        if (argc != 2)
        {
            lua_pushstring(L, "GetAddOnMetadata requires exactly 2 arguments: addonName, metadataKey.");
            lua_error(L);
            return 0; // Unreachable
        }

        // Retrieve addonName and metadataKey from Lua
        if (!LuaHelpers.IsString(L, 1) || !LuaHelpers.IsString(L, 2))
        {
            lua_pushstring(L, "GetAddOnMetadata: Both 'addonName' and 'metadataKey' must be strings.");
            lua_error(L);
            return 0; // Unreachable
        }

        var addonName = lua_tostring(L, 1);
        var metadataKey = lua_tostring(L, 2);

        // Verify that a TOC is loaded
        if (_currentToc == null)
        {
            lua_pushstring(L, "GetAddOnMetadata: No TOC loaded.");
            lua_error(L);
            return 0; // Unreachable
        }

        // Retrieve the metadata value from Toc
        // Assuming Toc has a method GetMetadata(string key)
        switch (metadataKey)
        {
            case "Title":
                lua_pushstring(L, _currentToc.Title);
                break;
            case "Version":
                lua_pushstring(L, _currentToc.Version);
                break;
            case "Author":
                lua_pushstring(L, _currentToc.Author);
                break;
            default:
                AnsiConsole.MarkupLine($"[yellow]GetAddOnMetadata: Metadata key '{metadataKey}' not found.[/]");
                lua_pushnil(L); // Push nil if metadataKey not found
                break;
        }

        return 1; // Number of return values
    }
}