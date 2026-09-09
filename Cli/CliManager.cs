using ImageConverter.Enums;
using ImageConverter.Models;
using ImageConverter.Services;
using Spectre.Console;

namespace ImageConverter.Cli;

public record MenuOption(MenuCommand CommandKey, string DisplayText);

public enum MenuCommand
{
    OneImage,
    OneFolder,
    ShowStats,
    Exit
}

/// <summary>
/// Handles the app's menu
/// </summary>
public class CliManager
{
    // Source of truth for menu options. Update here when new options are supported.
    private readonly MenuOption[] _menuOptions = new MenuOption[]
    {
        new MenuOption(MenuCommand.OneImage, "Convert an image"),
        new MenuOption(MenuCommand.OneFolder, "Convert all images in folder"),
        new MenuOption(MenuCommand.ShowStats, "Show all image stats in folder"),
        new MenuOption(MenuCommand.Exit, "Exit")
    };

    private readonly FileBrowser _imageBrowser = new FileBrowser("Select an image", ImageFormats.AllExtensions);

    private readonly ImageConversion _imageConversion = new ImageConversion();

    private void ShowTitle()
    {
        AnsiConsole.MarkupLine("[DarkViolet]Image[/] [bold DodgerBlue2]Converter[/]");
    }

    private void ShowDescription()
    {
        AnsiConsole.WriteLine("Convert images to other formats!");
        AnsiConsole.WriteLine("Supported formats:");
        AnsiConsole.WriteLine(string.Join(", ", ImageFormats.AllExtensions));
    }

    private MenuOption AskForCommand()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<MenuOption>()
                .Title("[bold]Choose an option:[/]")
                .UseConverter(option => option.DisplayText)
                .AddChoices(_menuOptions));
    }

    private ImageFormat AskForOutputFormat(ImageFormat? excludeFormat = null)
    {
        // We don't want to output an image to the same format as the input image
        IReadOnlyList<ImageFormat> supportedFormats = ImageFormats.SupportedFormats.Where(format => format != excludeFormat).ToList();
        
        return AnsiConsole.Prompt(
            new SelectionPrompt<ImageFormat>()
                .Title("[bold]Choose an output format:[/]")
                .AddChoices(supportedFormats));
    }

    private void ConvertOneImage()
    {
        string? imagePath = _imageBrowser.SelectFile();

        if (imagePath == null)
        {
            AnsiConsole.MarkupLine("[yellow]No image selected.[/]");

            return;
        }

        ImageFormat? sourceFormat = ImageFormats.GetFormatFromFilePath(imagePath);

        AnsiConsole.MarkupLine($"Selected [green]{Markup.Escape(imagePath)}[/] ([blue]{sourceFormat}[/])");

        ImageFormat targetFormat = AskForOutputFormat(sourceFormat);

        try
        {
            string outputPath = _imageConversion.Convert(imagePath, targetFormat);

            AnsiConsole.MarkupLine($"Converted to [green]{Markup.Escape(outputPath)}[/]");
        }
        catch (Exception exception)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(exception.Message)}[/]");
        }
    }

    public void Run()
    {
        ShowTitle();
        ShowDescription();

        AnsiConsole.WriteLine();
        
        MenuOption choice = AskForCommand();

        switch (choice.CommandKey)
        {
            case MenuCommand.OneImage:
                ConvertOneImage();
                break;
            default:
                AnsiConsole.MarkupLine($"Chosen [blue]{choice.CommandKey}[/]");
                break;
        }
    }
}