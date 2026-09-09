using Spectre.Console;

namespace ImageConverter.Cli;

/// <summary>
/// Lets the user walk through the folders on their computer and pick a file,
/// instead of having to type or paste a full path
/// </summary>
public class FileBrowser
{
    private enum EntryKind
    {
        Directory,
        File,
        Cancel
    }

    /// <summary>
    /// Represents a single row in the File Browser
    /// </summary>
    private record BrowseEntry(string DisplayText, string Path, EntryKind Kind);

    private const int PageSize = 15;

    private readonly string _title;
    private readonly IReadOnlyList<string> _allowedExtensions;

    // Kept between calls so the browser reopens where the user left off.
    private string _currentDirectory;

    public FileBrowser(string title, IReadOnlyList<string> allowedExtensions, string? startDirectory = null)
    {
        _title = title;
        _allowedExtensions = allowedExtensions;
        _currentDirectory = startDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    }

    /// <summary>
    /// Shows the browser until the user selects a file or cancels
    /// Returns the full path of the chosen file, or null when they cancelled
    /// </summary>
    /// <returns>E.g. "/foo/bar.jpeg"</returns>
    public string? SelectFile()
    {
        while (true)
        {
            List<BrowseEntry> entries;

            try
            {
                entries = ListEntries(_currentDirectory);
            }
            catch (Exception exception) when (exception is UnauthorizedAccessException or IOException)
            {
                if (!LeaveUnreadableDirectory(exception)) return null;

                continue;
            }

            BrowseEntry choice = AskForEntry(entries);

            switch (choice.Kind)
            {
                case EntryKind.Directory:
                    _currentDirectory = choice.Path;
                    break;
                case EntryKind.File:
                    return choice.Path;
                case EntryKind.Cancel:
                    return null;
            }
        }
    }

    /// <summary>
    /// Builds the rows shown for a given folder
    /// </summary>
    /// <param name="directory">E.g. "/foo/bar"</param>   
    /// <returns>A list of BrowseEntry rows</returns>
    private List<BrowseEntry> ListEntries(string directory)
    {
        List<BrowseEntry> entries = new List<BrowseEntry>();

        DirectoryInfo? parent = Directory.GetParent(directory);
        if (parent != null)
        {
            entries.Add(new BrowseEntry("[grey].. (up one level)[/]", parent.FullName, EntryKind.Directory));
        }

        foreach (string subDirectory in Directory.EnumerateDirectories(directory).Order())
        {
            if (IsHidden(subDirectory)) continue;

            string name = Markup.Escape(Path.GetFileName(subDirectory));
            entries.Add(new BrowseEntry($"[DodgerBlue2]{name}/[/]", subDirectory, EntryKind.Directory));
        }

        foreach (string file in Directory.EnumerateFiles(directory).Order())
        {
            if (IsHidden(file) || !IsAllowed(file)) continue;

            entries.Add(new BrowseEntry(Markup.Escape(Path.GetFileName(file)), file, EntryKind.File));
        }

        entries.Add(new BrowseEntry("[red]Cancel[/]", string.Empty, EntryKind.Cancel));

        return entries;
    }

    private BrowseEntry AskForEntry(List<BrowseEntry> entries)
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<BrowseEntry>()
                .Title($"[bold]{Markup.Escape(_title)}[/]\n[grey]{Markup.Escape(_currentDirectory)}[/]")
                .PageSize(PageSize)
                .MoreChoicesText("[grey](move up and down to see more)[/]")
                .UseConverter(entry => entry.DisplayText)
                .AddChoices(entries));
    }

    /// <summary>
    /// Reports a folder we are not allowed to read and steps back up to its
    /// parent
    /// </summary>
    /// <returns>Returns false when there is no parent left to fall back to</returns>   
    private bool LeaveUnreadableDirectory(Exception exception)
    {
        AnsiConsole.MarkupLine($"[red]Cannot open that folder:[/] {Markup.Escape(exception.Message)}");

        DirectoryInfo? parent = Directory.GetParent(_currentDirectory);
        if (parent == null) return false;

        _currentDirectory = parent.FullName;

        return true;
    }

    private bool IsAllowed(string filePath)
    {
        return _allowedExtensions.Contains(Path.GetExtension(filePath).ToLowerInvariant());
    }

    private bool IsHidden(string path)
    {
        return Path.GetFileName(path).StartsWith(".");
    }
}
