namespace ImageConverter.Cli.Options;

public class MenuExit : IMenuOption
{
    public string DisplayText { get; } = "Exit";
    public bool ExitsApp { get; } = true;
    public void Run()
    {
        // Simply do nothing to finish the program
    }
}