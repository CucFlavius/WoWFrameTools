using LuaNET.Lua51;
using Spectre.Console;
using WoWFrameTools.Widgets;
using static LuaNET.Lua51.Lua;

namespace WoWFrameTools;

public static class Log
{
    private const bool enableEventTriggerLogging = true;
    private const bool enableEventRegisterLogging = true;
    private const bool enableScriptSetLogging = true;
    private const bool enableEventUnRegisterLogging = true;
    private const bool enableChatLogging = false;
    private const bool enableHookScriptLogging = false;
    private const bool enablePrintLogging = true;
    private const bool enableRemoveScriptLogging = true;
    private const bool enableProcessFileLogging = false;
    private const bool enableAddonMessageLogging = false;
    
    private const bool enableFrameCreationLogging = false;
    private const bool enableCreateTextureLogging = false;
    private const bool enableCreateFontStringLogging = false;
    private const bool enableCreateLineLogging = false;

    public static void EventTrigger(string eventName, string? param = null, Frame? frame = null)
    {
        if (!enableEventTriggerLogging)
            return;

        if (param != null)
            AnsiConsole.MarkupLine($"Triggering global event: [yellow]'{eventName}'[/] with param: [blue]{param}[/] for frame {frame}");
        else
            AnsiConsole.MarkupLine($"Triggering global event: [yellow]'{eventName}'[/] for frame {frame}");
    }

    public static void EventRegister(string eventName, Frame frame)
    {
        if (!enableEventRegisterLogging)
            return;

        AnsiConsole.MarkupLine($"Registered event: [yellow]'{eventName}'[/] for frame {frame}");
    }

    public static void ScriptSet(string scriptTypeName, ScriptObject frame)
    {
        if (!enableScriptSetLogging)
            return;

        AnsiConsole.MarkupLine($"ScriptSet [green]'{scriptTypeName}'[/] set successfully to frame {frame}");
    }

    public static void CreateFrame(Frame frame)
    {
        if (!enableFrameCreationLogging)
            return;

        AnsiConsole.MarkupLine($"[green]Created Frame: {frame}[/]");
    }

    public static void UnregisterEvent(string eventName, Frame frame)
    {
        if (!enableEventUnRegisterLogging)
            return;

        AnsiConsole.MarkupLine($"Unregistered event: [yellow]'{eventName}'[/] for frame {frame}");
    }

    public static void ChatMessage(string message, string chatType, string? language, string? target)
    {
        if (!enableChatLogging)
            return;

        // Perform the chat message sending action
        // For demonstration, we'll log the message to the console
        var logMessage = $"[ChatType: {chatType.ToUpper()}] [Language: {language}]";
        if (!string.IsNullOrEmpty(target)) logMessage += $" [Target: {target}]";

        logMessage += $" Message: {message}";

        AnsiConsole.MarkupLine(logMessage);
    }

    public static void HookScript(string scriptTypeName, ScriptObject frame)
    {
        if (!enableHookScriptLogging)
            return;

        AnsiConsole.WriteLine($"Hooked script '{scriptTypeName}'.");
    }

    public static void Print(string toString)
    {
        if (!enablePrintLogging)
            return;
        AnsiConsole.WriteLine(toString);
    }

    public static void RemoveScript(string? scriptTypeName, ScriptObject? frame)
    {
        if (!enableRemoveScriptLogging)
            return;

        AnsiConsole.MarkupLine($"Script [green]'{scriptTypeName}'[/] removed successfully.");
    }

    public static void ProcessFile(string relativePath)
    {
        if (!enableProcessFileLogging)
            return;

        AnsiConsole.MarkupLine($"Process: [blue]{relativePath}[/]");
    }

    public static void AddonMessage(string message, string prefix, string channel, string? target)
    {
        if (!enableAddonMessageLogging)
            return;

        // Perform the addon message sending action
        // For demonstration, we'll log the message to the console
        var logMessage = $"[AddonMessage] [Prefix: {prefix}] [Channel: {channel.ToUpper()}]";
        if (!string.IsNullOrEmpty(target)) logMessage += $" [Target: {target}]";

        logMessage += $" Message: {message}";

        AnsiConsole.MarkupLine(logMessage);
    }
    
    public static void Warn(string p0)
    {
        AnsiConsole.MarkupLine($"[yellow]Warning: {p0}[/]");
    }

    public static void Exception(Exception exception)
    {
        AnsiConsole.WriteException(exception);
    }
    
    public static void Exception(lua_State L, Exception ex, string context)
    {
        // Retrieve the current Lua file and line number
        lua_Debug ar = new lua_Debug();
        lua_getstack(L, 1, ar);
        lua_getinfo(L, "Sl", ar);
        string luaFile = ar.source;
        int luaLine = ar.currentline;

        // Log the exception details along with the context, file, and line number
        AnsiConsole.MarkupLine($"[red]Exception in {context} at {luaFile}:{luaLine}[/]");
        AnsiConsole.WriteException(ex);
    }

    public static void Error(string message)
    {
        AnsiConsole.MarkupLine($"[red]Error: {message}[/]");
    }

    public static void ErrorL(lua_State L, string message)
    {
        AnsiConsole.MarkupLine($"[red]Error: {message}[/]");
        lua_pushstring(L, message);
        lua_error(L);
    }
    
    public static void Debug(string s)
    {
        AnsiConsole.MarkupLine($"[blue]Debug: {s}[/]");
    }

    public static void CreateTexture(Texture texture)
    {
        if (!enableCreateTextureLogging)
            return;

        AnsiConsole.MarkupLine($"[green]Created Texture: {texture}[/]");
    }

    public static void CreateFontString(FontString fontString)
    {
        if (!enableCreateFontStringLogging)
            return;

        AnsiConsole.MarkupLine($"[green]Created FontString: {fontString}[/]");
    }

    public static void CreateLine(Line line)
    {
        if (!enableCreateLineLogging)
            return;
        AnsiConsole.MarkupLine($"[green]Created Line: {line}[/]");
    }

    public static void CreateActor(ModelSceneActor actor)
    {
        AnsiConsole.MarkupLine($"[green]Created ModelSceneActor: {actor}[/]");
    }
}