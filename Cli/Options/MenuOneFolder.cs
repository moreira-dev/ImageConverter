using ImageConverter.Enums;
using ImageConverter.Services;
using Spectre.Console;

namespace ImageConverter.Cli.Options;

public class MenuOneFolder(ImageConversion conversionService) : IMenuOption
{
    public string DisplayText { get; } = "Convert all images in folder";

    private readonly FileBrowser _folderBrowser =
        new FileBrowser("Select a folder", conversionService.ImageFormats.AllExtensions);
    
    private ImageFormat AskForOutputFormat(ImageFormat? excludeFormat = null)
    {
        // We don't want to output an image to the same format as the input image
        IReadOnlyList<ImageFormat> supportedFormats = conversionService.ImageFormats.SupportedFormats.Where(format => format != excludeFormat).ToList();
        
        return AnsiConsole.Prompt(
            new SelectionPrompt<ImageFormat>()
                .Title("[bold]Choose an output format:[/]")
                .AddChoices(supportedFormats));
    }

    public void Run()
    {
        string? folderPath = _folderBrowser.SelectFolder();

        if (folderPath == null)
        {
            AnsiConsole.MarkupLine("[yellow]No folder selected.[/]");
            return;
        }
        
        ImageFormat targetFormat = AskForOutputFormat();
        
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