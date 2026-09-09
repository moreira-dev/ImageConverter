using ImageConverter.Models;
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

    private void ConvertOneImage()
    {
        string? imagePath = _imageBrowser.SelectFile();

        if (imagePath == null)
        {
            AnsiConsole.MarkupLine("[yellow]No image selected.[/]");

            return;
        }

        var format = ImageFormats.GetFormatFromFilePath(imagePath);

        AnsiConsole.MarkupLine($"Selected [green]{Markup.Escape(imagePath)}[/] ([blue]{format}[/])");
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