using ImageConverter.Cli.Options;
using ImageConverter.Services;
using Spectre.Console;

namespace ImageConverter.Cli;

/// <summary>
/// Handles the app's menu
/// </summary>
public class CliManager
{
    
    private readonly IMenuOption[] _menuOptions;

    private readonly ImageConversion _conversionService;

    public CliManager(ImageConversion conversionService, IEnumerable<IMenuOption> menuOptions)
    {
        _conversionService = conversionService;
        _menuOptions = menuOptions.ToArray();
    }

    private void ShowTitle()
    {
        AnsiConsole.MarkupLine("[DarkViolet]Image[/] [bold DodgerBlue2]Converter[/]");
    }

    private void ShowDescription()
    {
        AnsiConsole.WriteLine("Convert images to other formats!");
        AnsiConsole.WriteLine("Supported formats:");
        AnsiConsole.WriteLine(string.Join(", ", _conversionService.ImageFormats.AllExtensions));
    }

    private IMenuOption AskForCommand()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<IMenuOption>()
                .Title("[bold]Choose an option:[/]")
                .UseConverter(option => option.DisplayText)
                .AddChoices(_menuOptions));
    }

    public void Run()
    {
        ShowTitle();
        ShowDescription();

        AnsiConsole.WriteLine();
        
        IMenuOption choice = AskForCommand();
        
        choice.Run();
    }
}