using ImageConverter.Enums;
using ImageConverter.Models.Converters;

namespace ImageConverter.Models;

/// <summary>
/// Helper model to quickly retrieve supported formats and extensions
/// </summary>
public class ImageFormats
{
    private readonly Dictionary<ImageFormat, string[]> _supportedFormats;
    
    private readonly Dictionary<string, ImageFormat> _formatsByExtensions;

    public ImageFormats(FormatConverter[] converterList)
    {
        _supportedFormats = GetSupportedFormats(converterList);
        _formatsByExtensions = GetFormatsByExtensions();
    }

    /// <summary>
    /// Creates a new dictionary to be cached for better performance
    /// </summary>
    private Dictionary<ImageFormat, string[]> GetSupportedFormats(FormatConverter[] converterList)
    {
        Dictionary<ImageFormat, string[]> formats = new Dictionary<ImageFormat, string[]>();
        
        foreach (FormatConverter converter in converterList)
        {
            formats.Add(converter.Format, converter.SupportedExtensions);
            
        }

        return formats;
    }
    
    /// <summary>
    /// Creates a new dictionary to be cached for better performance
    /// </summary>
    private Dictionary<string, ImageFormat> GetFormatsByExtensions()
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

    /// <summary>
    /// Returns a list of all supported extensions
    /// e.g. [ ".jpg", ".jpeg", ".png", ".webp" ]
    /// </summary>
    public IReadOnlyList<string> AllExtensions
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
    public IEnumerable<ImageFormat> SupportedFormats
    {
        get
        {
            return _supportedFormats.Keys;
        }
    }

    /// <summary>
    /// Returns a list of all extensions for the given format
    /// </summary>
    public IReadOnlyList<string>? GetExtensionsFor(ImageFormat format)
    {
        return _supportedFormats.GetValueOrDefault(format);
    }
    
    /// <summary>
    /// Returns the first extension for the given format.
    /// Usually used for output formats
    /// </summary>
    /// <exception cref="KeyNotFoundException">When using an ImageFormat without extensions. This should never happen</exception>   
    public string GetPrimaryExtensionFor(ImageFormat format)
    {
        return _supportedFormats[format].First();
    }

    /// <summary>
    /// Returns the format for a given extension
    /// </summary>
    /// <param name="extension">E.g. ".jpeg" or ".JPEG"</param>
    /// <returns>E.g. "JPG" or null</returns>
    public ImageFormat? GetFormatByExtension(string extension)
    {
        
        if (!_formatsByExtensions.TryGetValue(extension.ToLower(), out ImageFormat format))
        {
            return null;
        }

        return format;
    }

    /// <summary>
    /// Returns the format for a given file path
    /// </summary>
    /// <param name="filePath">E.g. "/foo/bar.jpg"</param>
    /// <returns>E.g. "JPG" or null</returns>
    public ImageFormat? GetFormatFromFilePath(string filePath)
    {
        return GetFormatByExtension(Path.GetExtension(filePath));
    }
}