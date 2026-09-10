using ImageConverter.Cli;
using ImageConverter.Cli.Options;
using ImageConverter.Models.Converters;
using ImageConverter.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ImageConverter;

internal abstract class Program
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
        ImageStats statsService = new ImageStats(conversionService.ImageFormats);
        
        // Dependency injection setup
        // TODO consider moving to Extension Members
        ServiceCollection services = new ServiceCollection();
        services.AddSingleton<ImageConversion>(conversionService);
        services.AddSingleton<ImageStats>(statsService);
        
        // New menu options here
        services.AddSingleton<IMenuOption, MenuOneImage>();
        services.AddSingleton<IMenuOption, MenuOneFolder>();
        services.AddSingleton<IMenuOption, MenuShowStats>();
        services.AddSingleton<IMenuOption, MenuExit>();
        services.AddSingleton<CliManager>();
        
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        
        CliManager cli = serviceProvider.GetRequiredService<CliManager>();
        
        cli.Run();
    }
}