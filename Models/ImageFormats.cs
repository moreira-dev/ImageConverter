using ImageConverter.Enums;

namespace ImageConverter.Models;

public static class ImageFormats
{
    // Source of truth for supported formats. Update here when new formats are supported.
    private static readonly Dictionary<ImageFormat, string[]> _supportedFormats = new Dictionary<ImageFormat, string[]>()
    {
        [ImageFormat.JPG] = [".jpg", ".jpeg"],
        [ImageFormat.PNG] = [".png"],
        [ImageFormat.WEBP] = [".webp"]
    };
    
    private static readonly Dictionary<string, ImageFormat> _formatsByExtensions;

    static ImageFormats()
    {
        _formatsByExtensions = GetFormatsByExtensions();
    }

    /// <summary>
    /// Returns a list of all supported extensions
    /// e.g. [".jpg", ".jpeg", ".png", ".webp"]
    /// </summary>
    public static IReadOnlyList<string> AllExtensions
    {
        get
        {
            return _formatsByExtensions.Keys.ToList();
        }
    }

    /// <summary>
    /// Returns a list of all supported formats
    /// e.g. ["JPG", "PNG", "WEBP"]
    /// </summary>
    public static IEnumerable<ImageFormat> SupportedFormats
    {
        get
        {
            return _supportedFormats.Keys;
        }
    }

    /// <summary>
    /// Returns a list of all extensions for the given format
    /// </summary>
    public static IReadOnlyList<string>? GetExtensionsFor(ImageFormat format)
    {
        return _supportedFormats.GetValueOrDefault(format);
    }
    
    /// <summary>
    /// Returns the first extension for the given format.
    /// Usually used for output formats
    /// </summary>
    /// <exception cref="KeyNotFoundException">When using a ImageFormat without extensions. This should never happen</exception>   
    public static string GetPrimaryExtensionFor(ImageFormat format)
    {
        return _supportedFormats[format].First();
    }

    /// <summary>
    /// Returns the format for a given extension
    /// </summary>
    /// <param name="extension">E.g. ".jpeg"</param>
    /// <returns>E.g. "JPG" or null</returns>
    public static ImageFormat? GetFormatByExtension(string extension)
    {
        
        return _formatsByExtensions.GetValueOrDefault(extension);
    }

    /// <summary>
    /// Returns the format for a given file path
    /// </summary>
    /// <param name="filePath">E.g. "/foo/bar.jpg"</param>
    /// <returns>E.g. "JPG" or null</returns>
    public static ImageFormat? GetFormatFromFilePath(string filePath)
    {
        return GetFormatByExtension(Path.GetExtension(filePath));
    }

    /// <summary>
    /// Creates a new dictionary to be cached for better performance
    /// </summary>
    private static Dictionary<string, ImageFormat> GetFormatsByExtensions()
    {
        Dictionary<string, ImageFormat> formats = new Dictionary<string, ImageFormat>();

        foreach (KeyValuePair<ImageFormat, string[]> format in _supportedFormats)
        {
            foreach (string extension in format.Value)
            {
                formats[extension] = format.Key;
            }
        }

        return formats;
    }
}