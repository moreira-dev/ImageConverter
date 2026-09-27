using ImageConverter.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace ImageConverter.Models.Converters;

public abstract class FormatConverter
{
    public abstract ImageFormat Format { get; }
    public abstract string[] SupportedExtensions { get; }
    protected abstract ImageEncoder Encoder { get; }

    public void Convert(string sourcePath, string outputPath)
    {
        using Image image = Image.Load(sourcePath);
        image.Save(outputPath, Encoder);
    }
}