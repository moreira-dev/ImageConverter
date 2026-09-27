using ImageConverter.Enums;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;

namespace ImageConverter.Models.Converters;

public class PngConverter : FormatConverter
{
    public override ImageFormat Format { get; } = ImageFormat.PNG;
    public override string[] SupportedExtensions { get; } = new[] { ".png" };
    protected override ImageEncoder Encoder { get; } = new PngEncoder();
}