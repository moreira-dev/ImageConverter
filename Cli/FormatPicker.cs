using ImageConverter.Enums;
using ImageConverter.Models;
using Spectre.Console;

namespace ImageConverter.Cli;

/// <summary>
/// Lets the user pick one of the supported image formats
/// </summary>
public class FormatPicker
{
    private readonly ImageFormats _imageFormats;

    public FormatPicker(ImageFormats imageFormats)
    {
        _imageFormats = imageFormats;
    }

    /// <summary>
    /// Asks the user for an output format
    /// </summary>
    /// <param name="excludeFormat">A format to exclude from the list, e.g. the format of the source image</param>
    /// <returns>E.g. "PNG"</returns>
    public ImageFormat SelectOutputFormat(ImageFormat? excludeFormat = null)
    {
        IReadOnlyList<ImageFormat> formats = _imageFormats.SupportedFormats.Where(format => format != excludeFormat).ToList();

        return AnsiConsole.Prompt(
            new SelectionPrompt<ImageFormat>()
                .Title("[bold]Choose an output format:[/]")
                .AddChoices(formats));
    }
}
