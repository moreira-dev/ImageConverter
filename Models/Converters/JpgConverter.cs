using ImageConverter.Enums;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace ImageConverter.Models.Converters;

public class JpgConverter : FormatConverter
{
    public override ImageFormat Format { get; } = ImageFormat.JPG;
    public override string[] SupportedExtensions { get; } = new[] { ".jpg", ".jpeg" };
    protected override ImageEncoder Encoder { get; } = new JpegEncoder();
}