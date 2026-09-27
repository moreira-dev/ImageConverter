namespace ImageConverter.Cli.Options;

public interface IMenuOption
{
    public string DisplayText { get; }

    /// <summary>
    /// Whether choosing this option closes the app instead of returning to the menu
    /// </summary>
    public bool ExitsApp
    {
        get { return false; }
    }

    public void Run();
}