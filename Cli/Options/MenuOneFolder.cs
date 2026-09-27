using ImageConverter.Enums;
using ImageConverter.Services;
using Spectre.Console;

namespace ImageConverter.Cli.Options;

public class MenuOneFolder(ImageConversion conversionService) : IMenuOption
{
    public string DisplayText { get; } = "Convert all images in folder";

    private readonly FileBrowser _folderBrowser =
        new FileBrowser("Select a folder", conversionService.ImageFormats.AllExtensions);

    private readonly FormatPicker _formatPicker = new FormatPicker(conversionService.ImageFormats);

    public void Run()
    {
        string? folderPath = _folderBrowser.SelectFolder();

        if (folderPath == null)
        {
            AnsiConsole.MarkupLine("[yellow]No folder selected.[/]");
            return;
        }
        
        ImageFormat targetFormat = _formatPicker.SelectOutputFormat();
        
        foreach (string filePath in Directory.EnumerateFiles(folderPath).Order())
        {
            ImageFormat? format = conversionService.ImageFormats.GetFormatFromFilePath(filePath);

            // We are not converting images that are already in the target format
            if (format == null || format == targetFormat)
            {
                continue;
            }

            try
            {
                string outputPath = conversionService.Convert(filePath, targetFormat);

                AnsiConsole.MarkupLine($"[green]{Markup.Escape(Path.GetFileName(outputPath))}[/] created.");
            }
            catch (Exception exception)
            {
                AnsiConsole.MarkupLine($"[red]{Markup.Escape(exception.Message)}[/]");
            }
        }
    }
}