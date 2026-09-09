using ImageConverter.Enums;
using ImageConverter.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;

namespace ImageConverter.Services;

/// <summary>
/// Converts images from one format to another. This is the only class that
/// talks to ImageSharp, so the rest of the app stays independent of it.
/// </summary>
public class ImageConversion
{
    // Source of truth for how each format is written. Update here when new formats are supported.
    private static readonly Dictionary<ImageFormat, ImageEncoder> _encoders =
        new Dictionary<ImageFormat, ImageEncoder>()
        {
            [ImageFormat.JPG] = new JpegEncoder(),
            [ImageFormat.PNG] = new PngEncoder(),
            [ImageFormat.WEBP] = new WebpEncoder()
        };

    /// <summary>
    /// Converts an image to a given format and saves it to a new file in the same folder
    /// </summary>
    /// <param name="sourcePath">E.g. "/foo/bar.jpg"</param>
    /// <param name="targetFormat">E.g. "PNG"</param>
    /// <returns>The path of the converted image on success. E.g. "/foo/bar.png"</returns>
    /// <exception>When the image could not be read or written</exception>
    public string Convert(string sourcePath, ImageFormat targetFormat)
    {
        ImageEncoder encoder = GetEncoderFor(targetFormat);
        string outputPath = BuildOutputPath(sourcePath, targetFormat);

        // See https://docs.sixlabors.com/articles/imagesharp/gettingstarted.html#dispose-images-promptly
        // And https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/using
        using Image image = Image.Load(sourcePath);
        image.Save(outputPath, encoder);

        return outputPath;
    }

    /// <summary>
    /// Returns the ImageSharp encoder used to handle the given format
    /// </summary>
    /// <exception cref="KeyNotFoundException">When using a ImageFormat without encoder. This should never happen</exception>
    private ImageEncoder GetEncoderFor(ImageFormat format)
    {
        return _encoders[format];
    }

    /// <summary>
    /// Creates the output path based on the source path and the target format.
    /// Returns the new path
    /// </summary>
    /// <param name="sourcePath">E.g. "/foo/bar.jpg"</param>
    /// <param name="targetFormat">E.g. "PNG"</param>
    /// <returns>E.g. "/foo/bar.png"</returns>
    private string BuildOutputPath(string sourcePath, ImageFormat targetFormat)
    {
        string outputExtension = ImageFormats.GetPrimaryExtensionFor(targetFormat);

        string directory = Path.GetDirectoryName(sourcePath) ?? "/";
        string fileName = Path.GetFileNameWithoutExtension(sourcePath);
        string outputPath = Path.Combine(directory, $"{fileName}{outputExtension}");

        // TODO what should we do if the outputPath already exists? - we override for now

        return outputPath;
    }
}