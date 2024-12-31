using Spectre.Console;

namespace WoWFrameTools;

public static class Log
{
    private const bool enableEventTriggerLogging = true;
    private const bool enableEventRegisterLogging = false;
    private const bool enableScriptSetLogging = false;
    private const bool enableEventUnRegisterLogging = false;
    private const bool enableChatLogging = false;
    private const bool enableHookScriptLogging = false;
    private const bool enablePrintLogging = true;
    private const bool enableRemoveScriptLogging = false;
    private const bool enableProcessFileLogging = false;
    private const bool enableAddonMessageLogging = false;
    
    private const bool enableFrameCreationLogging = false;
    private const bool enableCreateTextureLogging = false;
    private const bool enableCreateFontStringLogging = false;

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

    public static void ScriptSet(string scriptTypeName, Frame frame)
    {
        if (!enableScriptSetLogging)
            return;

        AnsiConsole.MarkupLine($"Script [green]'{scriptTypeName}'[/] set successfully to frame {frame}");
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

    public static void HookScript(string scriptTypeName, Frame frame)
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

    public static void RemoveScript(string scriptTypeName, Frame frame)
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

    public static void CreateFontString(FontString fontString)
    {
        if (!enableCreateFontStringLogging)
            return;

        AnsiConsole.MarkupLine($"[green]Created FontString {fontString}[/]");
    }

    public static void CreateTexture(Texture texture)
    {
        if (!enableCreateTextureLogging)
            return;

        AnsiConsole.MarkupLine($"[green]Created Texture {texture}[/]");
    }

    public static void Warn(string p0)
    {
        AnsiConsole.MarkupLine($"[yellow]Warning: {p0}[/]");
    }

    public static void CreateLine(Line line)
    {
        //AnsiConsole.MarkupLine($"[green]Created Line {line}[/]");
    }
}