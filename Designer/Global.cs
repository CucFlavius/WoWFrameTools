using System.Runtime.InteropServices;
using LuaNET.Lua51;
using Spectre.Console;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools;

public partial class Global
{
    private static Toc? _currentToc;
    public static readonly object _luaLock = new();
    private static readonly object _prefixLock = new();
    private static readonly HashSet<string> _registeredPrefixes = new();
    private static readonly Dictionary<string, string> _slashCommands = new();

    public static void SetToc(Toc toc)
    {
        _currentToc = toc;
    }

    public static void TriggerEvent(lua_State L, string eventName, string? param = null)
    {
        lock (Frame._eventLock)
        {
            if (Frame._eventToFrames.TryGetValue(eventName, out HashSet<Frame> frames))
                foreach (var frame in frames.ToList()) // Use ToList to create a copy for safe iteration
                {
                    Log.EventTrigger(eventName, param, frame);
                    frame.TriggerEvent(eventName, param);
                }
            else
                AnsiConsole.WriteLine($"No frames registered for event '{eventName}'.");
        }
    }

    /// <summary>
    ///     Executes a slash command by invoking the corresponding handler in SlashCmdList.
    /// </summary>
    /// <param name="L"></param>
    /// <param name="command">The slash command string (e.g., "/hello").</param>
    public static void ExecuteSlashCommand(lua_State L, string command)
    {
        lock (_luaLock)
        {
            try
            {
                if (_slashCommands.TryGetValue(command, out var name))
                {
                    // Get the handler function from SlashCmdList
                    lua_getglobal(L, "SlashCmdList");
                    lua_pushstring(L, name);
                    lua_gettable(L, -2); // Push SlashCmdList[name]

                    if (lua_isfunction(L, -1) == 0)
                    {
                        AnsiConsole.WriteLine($"Handler for slash command '{command}' is not a function.");
                        lua_pop(L, 2); // Pop non-function and SlashCmdList
                        return;
                    }

                    // Call the handler function with the command as argument
                    lua_pushvalue(L, -1); // Push handler
                    lua_pushnil(L); // self (could be Frame or nil)
                    lua_pushstring(L, command); // msg
                    if (lua_pcall(L, 2, 0, 0) != 0)
                    {
                        var error = lua_tostring(L, -1);
                        AnsiConsole.WriteLine($"Lua Error in SlashCmdList['{name}']: {error}");
                        lua_pop(L, 1);
                    }

                    lua_pop(L, 1); // Pop SlashCmdList
                }
                else
                {
                    AnsiConsole.WriteLine($"No handler registered for slash command '{command}'.");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }
    }

    public static void InitializeMinimap(lua_State L)
    {
        // Create a new Frame instance for Minimap
        var minimap = new Frame(L);

        // Allocate a GCHandle to prevent the Frame from being garbage collected
        var handle = GCHandle.Alloc(minimap);
        var handlePtr = GCHandle.ToIntPtr(handle);

        // Create userdata with the size of IntPtr
        var userdataPtr = (IntPtr)lua_newuserdata(L, (UIntPtr)IntPtr.Size);

        // Write the handlePtr into the userdata memory
        Marshal.WriteIntPtr(userdataPtr, handlePtr);

        // Set the metatable for the userdata
        luaL_getmetatable(L, "FrameMetaTable");
        lua_setmetatable(L, -2);

        // Add the Frame to the registry for later retrieval
        Frame._frameRegistry[userdataPtr] = minimap;

        // Assign the userdataPtr and LuaRegistryRef to the Frame instance
        minimap.UserdataPtr = userdataPtr;

        // Create a reference to the userdata in the registry
        lua_pushvalue(L, -1); // Push the userdata onto the stack
        var refIndex = luaL_ref(L, LUA_REGISTRYINDEX);
        minimap.LuaRegistryRef = refIndex;

        // Set the Minimap as a global variable in Lua
        lua_rawgeti(L, LUA_REGISTRYINDEX, refIndex); // Push the userdata
        lua_setglobal(L, "Minimap");
    }

    // Add the following method to initialize SlashCmdList
    public static void InitializeSlashCmdList(lua_State L)
    {
        lock (_luaLock)
        {
            try
            {
                lua_newtable(L); // Create a new table
                lua_setglobal(L, "SlashCmdList"); // Set it as the global 'SlashCmdList'

                //AnsiConsole.WriteLine("SlashCmdList table initialized.");
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }
    }

    private static void RegisterC_AddOns(lua_State L)
    {
        // Create a new table for C_AddOns
        lua_newtable(L); // Push a new table onto the stack

        // Push the GetAddOnMetadata function
        lua_pushcfunction(L, internal_GetAddOnMetadata);
        lua_setfield(L, -2, "GetAddOnMetadata"); // C_AddOns.GetAddOnMetadata = GetAddOnMetadata

        // Set the C_AddOns table as a global
        lua_setglobal(L, "C_AddOns"); // Pops the table from the stack
    }

    private static void RegisterC_ChatInfo(lua_State L)
    {
        // Create a new table for C_ChatInfo
        lua_newtable(L); // Push a new table onto the stack

        // Push the RegisterAddonMessagePrefix function
        lua_pushcfunction(L, internal_RegisterAddonMessagePrefix);
        lua_setfield(L, -2, "RegisterAddonMessagePrefix"); // C_ChatInfo.RegisterAddonMessagePrefix = RegisterAddonMessagePrefix

        // Set the C_ChatInfo table as a global
        lua_setglobal(L, "C_ChatInfo"); // Pops the table from the stack
    }
}