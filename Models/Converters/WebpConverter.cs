using ImageConverter.Enums;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Webp;

namespace ImageConverter.Models.Converters;

public class WebpConverter : FormatConverter
{
    public override ImageFormat Format { get; } = ImageFormat.WEBP;
    public override string[] SupportedExtensions { get; } = new[] { ".webp" };
    protected override ImageEncoder Encoder { get; } = new WebpEncoder();
}