using ImageConverter.Cli;

namespace ImageConverter;

class Program
{
    public static void Main(string[] args)
    {
        CliManager cli = new CliManager();
        
        cli.Run();
    }
}