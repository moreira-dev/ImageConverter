
namespace ImageConverter.Cli.Options;

public class MenuOneFolder: IMenuOption
{
    public string DisplayText { get; } = "Convert all images in folder";
    public void Run()
    {
        throw new NotImplementedException();
    }
}