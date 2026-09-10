using ImageConverter.Services;
using Spectre.Console;

namespace ImageConverter.Cli.Options;

// See primary constructors
public class MenuShowStats(ImageConversion conversionService, ImageStats statsService) : IMenuOption
{
    public string DisplayText { get; } = "Show all image stats in folder";

    private readonly FileBrowser _folderBrowser =
        new FileBrowser("Select a folder", conversionService.ImageFormats.AllExtensions);

    /// <summary>
    /// Formats the image size for readability
    /// </summary>
    /// <returns>E.g. "1920 x 1080"</returns>
    private string FormatDimensions(ImageStat stat)
    {
        if (stat.Width == null || stat.Height == null)
        {
            return "[red]Unknown[/]";
        }

        return $"{stat.Width} x {stat.Height}";
    }

    /// <summary>
    /// Turns an amount of bytes into something easier to read
    /// </summary>
    /// <param name="bytes">E.g. 2048</param>
    /// <returns>E.g. "2 KB"</returns>
    private string FormatFileSize(long bytes)
    {
        // Thanks to https://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net
        string[] units = new string[] { "B", "KB", "MB", "GB" };

        double size = bytes;
        int unit = 0;

        while (size >= 1024 && unit < units.Length - 1)
        {
            size /= 1024;
            unit++;
        }

        return $"{size:0.#} {units[unit]}";
    }

    /// <summary>
    /// Prints a table with the stats of all images in the list
    /// </summary>
    private void ShowStatsTable(IReadOnlyList<ImageStat> stats)
    {
        Table table = new Table();

        table.AddColumn("Image");
        table.AddColumn("Format");
        table.AddColumn("Dimensions");
        table.AddColumn("File size");

        foreach (ImageStat stat in stats)
        {
            table.AddRow(
                Markup.Escape(Path.GetFileName(stat.FilePath)),
                stat.Format.ToString(),
                FormatDimensions(stat),
                FormatFileSize(stat.FileSizeInBytes));
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine($"[grey]{stats.Count} image(s)[/]");
    }

    public void Run()
    {
        string? folderPath = _folderBrowser.SelectFolder();

        if (folderPath == null)
        {
            AnsiConsole.MarkupLine("[yellow]No folder selected.[/]");
            return;
        }

        try
        {
            IReadOnlyList<ImageStat> stats = statsService.GetStatsForFolder(folderPath);

            if (stats.Count == 0)
            {
                AnsiConsole.MarkupLine($"[yellow]No images found in {Markup.Escape(folderPath)}[/]");
                return;
            }

            ShowStatsTable(stats);
        }
        catch (Exception exception)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(exception.Message)}[/]");
        }
    }
}