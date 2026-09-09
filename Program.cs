using ImageConverter.Cli;
using ImageConverter.Models.Converters;
using ImageConverter.Services;

namespace ImageConverter;

internal class Program
{
    private static void Main(string[] args)
    {
        // Source of truth for supported formats. Update here when new formats are supported.
        FormatConverter[] supportedFormats = new FormatConverter[]
        {
            new JpgConverter(),
            new PngConverter(),
            new WebpConverter()
        };
        ImageConversion conversionService = new ImageConversion(supportedFormats);
        
        CliManager cli = new CliManager(conversionService);
        
        cli.Run();
    }
}