namespace ImageConverter.Cli.Options;

public class MenuExit : IMenuOption
{
    public string DisplayText { get; } = "Exit";
    public void Run()
    {
        // Simply do nothing to finish the program
    }
}