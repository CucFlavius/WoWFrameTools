namespace WoWFrameTools;

public class Toc
{
    public Toc(string path)
    {
        var lines = File.ReadAllLines(path);
        foreach (var line in lines)
            if (line.StartsWith("##"))
            {
                var parts = line.Substring(2).Split(':', 2, StringSplitOptions.TrimEntries);
                if (parts.Length == 2)
                {
                    var key = parts[0];
                    var value = parts[1];

                    switch (key)
                    {
                        case "Interface":
                            Interface = value;
                            break;
                        case "Title":
                            Title = value;
                            break;
                        case "Notes":
                            Notes = value;
                            break;
                        case "Author":
                            Author = value;
                            break;
                        case "DefaultState":
                            DefaultState = value;
                            break;
                        case "LoadOnDemand":
                            LoadOnDemand = value == "1";
                            break;
                        case "SavedVariables":
                            SavedVariables.AddRange(value.Split(' ', StringSplitOptions.RemoveEmptyEntries));
                            break;
                        case "X-eMail":
                            Email = value;
                            break;
                        case "X-Website":
                            Website = value;
                            break;
                        case "X-License":
                            License = value;
                            break;
                        case "IconTexture":
                            IconTexture = value;
                            break;
                        case "Version":
                            Version = value;
                            break;
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(line))
            {
                CodePaths.Add(line.Trim());
            }
    }

    public string Interface { get; private set; }
    public string Title { get; private set; }
    public string Notes { get; private set; }
    public string Author { get; private set; }
    public string DefaultState { get; private set; }
    public bool LoadOnDemand { get; private set; }
    public List<string> SavedVariables { get; } = new();
    public string Email { get; private set; }
    public string Website { get; private set; }
    public string License { get; private set; }
    public string IconTexture { get; private set; }
    public string Version { get; private set; }
    public List<string> CodePaths { get; } = new();
}