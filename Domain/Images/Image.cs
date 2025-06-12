using Domain.Users;

namespace Domain.Images;

public sealed class Image
{
    public const int MaxSizeInBytes = 10000000;                     // 10 MB  
    public const int MinResolutionWidthInPixels = 32;               // Minimum width in pixels for social media icons
    public const int MinResolutionHeightInPixels = 32;              // Minimum height in pixels for social media icons
    public const string BaseDirectory = "Images";                   // Base logical directory for image uploads

    public Image() {}

    public static int GetHeightIfPredefinedImageHasBeenSelected(PredefinedImages definedImage)
    {
        List<PredefinedImageDimensions>? predefinedImageDimensionsList = GetPredefinedImageDimensionsList();
        if (predefinedImageDimensionsList != null)
        {
            PredefinedImageDimensions? dimension =
                predefinedImageDimensionsList.FirstOrDefault(pdi => pdi.PredefinedImage == definedImage);
            return dimension?.Height ?? 0;
        }
        return 0;
    }

    public static bool IsImageFileTypeValid(ImageFileType imageFileType)
    {
        return Enum.IsDefined(imageFileType);
    }

    public static bool IsSizeValid(long sizeInBytes)
    {
        return sizeInBytes > 0 && sizeInBytes <= MaxSizeInBytes;
    }

    public static bool IsResolutionValid(int widthInPixels, int heightInPixels)
    {
        return widthInPixels >= MinResolutionWidthInPixels && heightInPixels >= MinResolutionHeightInPixels;
    }

    public static ImageFileType DetermineImageFileType(string contentType)
    {
        return contentType switch
        {
            "image/jpeg" => ImageFileType.Jpeg,
            "image/png" => ImageFileType.Png,
            "image/gif" => ImageFileType.Gif,
            "image/bmp" => ImageFileType.Bmp,
            "image/tiff" => ImageFileType.Tiff,
            "image/svg+xml" => ImageFileType.Svg,
            "image/webp" => ImageFileType.Webp,
            "image/heic" => ImageFileType.Heic,
            "image/avif" => ImageFileType.Avif,
            _ => ImageFileType.Unknown
        };
    }

    public static string DetermineContentType(ImageFileType imageFileType)
    {
        return imageFileType switch
        {
            ImageFileType.Jpeg => "image/jpeg",
            ImageFileType.Png => "image/png",
            ImageFileType.Gif => "image/gif",
            ImageFileType.Bmp => "image/bmp",
            ImageFileType.Tiff => "image/tiff",
            ImageFileType.Svg => "image/svg+xml",
            ImageFileType.Webp => "image/webp",
            ImageFileType.Heic => "image/heic",
            ImageFileType.Avif => "image/avif",
            _ => "application/octet-stream" // Default for unknown types
        };
    }

    private static List<PredefinedImageDimensions> GetPredefinedImageDimensionsList()
    {
        List<PredefinedImageDimensions> list = new List<PredefinedImageDimensions>
            {
                new PredefinedImageDimensions { PredefinedImage = PredefinedImages.Thumbnail, Width = 160, Height = 160 },
                new PredefinedImageDimensions { PredefinedImage = PredefinedImages.SocialMediaIcon, Width = 32, Height = 32 },
                new PredefinedImageDimensions { PredefinedImage = PredefinedImages.Small, Width = 1200, Height = 1800 },
                new PredefinedImageDimensions { PredefinedImage = PredefinedImages.Medium, Width = 1500, Height = 2100 },
                new PredefinedImageDimensions { PredefinedImage = PredefinedImages.Large, Width = 2400, Height = 3800 },
            };
        return list;
    }

    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required ImageFileType ImageFileType { get; set; }
    public required string Path { get; set; }
    public string? ContentType { get; set; }
    public int Height { get; set; } // Dimensions in pixels     
    public int Width { get; set; }  // Dimensions in pixels
    public long Size { get; set; }  // Size in bytes
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
