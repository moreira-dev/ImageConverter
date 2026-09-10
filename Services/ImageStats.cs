using ImageConverter.Enums;
using ImageConverter.Models;
using SixLabors.ImageSharp;

namespace ImageConverter.Services;

/// <summary>
/// The details we know about a single image.
/// Width and Height are null when the file cannot be read
/// </summary>
public record ImageStat(string FilePath, ImageFormat Format, long FileSizeInBytes, int? Width, int? Height);

/// <summary>
/// Main class to read the stats of the images inside a folder
/// </summary>
public class ImageStats
{
    private readonly ImageFormats _imageFormats;

    public ImageStats(ImageFormats imageFormats)
    {
        _imageFormats = imageFormats;
    }

    /// <summary>
    /// Reads the stats of every supported image in the given folder.
    /// </summary>
    /// <param name="folderPath">E.g. "/foo/bar"</param>
    /// <returns>The list of ImageStat, sorted by file name ASC</returns>
    public IReadOnlyList<ImageStat> GetStatsForFolder(string folderPath)
    {
        List<ImageStat> stats = new List<ImageStat>();

        foreach (string filePath in Directory.EnumerateFiles(folderPath).Order())
        {
            ImageFormat? format = _imageFormats.GetFormatFromFilePath(filePath);

            if (format == null)
            {
                continue;
            }

            stats.Add(GetStatsForFile(filePath, format.Value));
        }

        return stats;
    }

    /// <summary>
    /// Reads the stats of a single image
    /// </summary>
    /// <param name="filePath">E.g. "/foo/bar.jpeg"</param>
    /// <param name="format">E.g. "JPG"</param>
    private ImageStat GetStatsForFile(string filePath, ImageFormat format)
    {
        long fileSizeInBytes = new FileInfo(filePath).Length;
        int? width = null;
        int? height = null;

        try
        {
            // Get image metadata through ImageSharp
            ImageInfo imageInfo = Image.Identify(filePath);
            
            width = imageInfo.Width;
            height = imageInfo.Height;
        }
        catch (Exception exception) when (exception is ArgumentNullException or NotSupportedException or InvalidImageContentException or UnknownImageFormatException)
        {
           // Dimensions will be null for images that failed to parse
        }
        
        return new ImageStat(filePath, format, fileSizeInBytes, width, height);
    }
}
