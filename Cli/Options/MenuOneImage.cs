using ImageConverter.Enums;
using ImageConverter.Services;
using Spectre.Console;

namespace ImageConverter.Cli.Options;

// See primary constructors
public class MenuOneImage(ImageConversion conversionService): IMenuOption
{
    public string DisplayText { get; } = "Convert one image";
    
    private readonly FileBrowser _imageBrowser = new FileBrowser("Select an image", conversionService.ImageFormats.AllExtensions);
    
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
        string? imagePath = _imageBrowser.SelectFile();

        if (imagePath == null)
        {
            AnsiConsole.MarkupLine("[yellow]No image selected.[/]");

            return;
        }

        ImageFormat? sourceFormat = conversionService.ImageFormats.GetFormatFromFilePath(imagePath);

        AnsiConsole.MarkupLine($"Selected [green]{Markup.Escape(imagePath)}[/] ([blue]{sourceFormat}[/])");

        ImageFormat targetFormat = AskForOutputFormat(sourceFormat);

        try
        {
            string outputPath = conversionService.Convert(imagePath, targetFormat);

            AnsiConsole.MarkupLine($"Converted to [green]{Markup.Escape(outputPath)}[/]");
        }
        catch (Exception exception)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(exception.Message)}[/]");
        }
    }
}