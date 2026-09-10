namespace ImageConverter.Cli.Options;

public interface IMenuOption
{
    public string DisplayText { get; }

    public void Run();
}