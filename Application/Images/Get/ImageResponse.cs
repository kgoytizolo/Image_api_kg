using Domain.Images;

namespace Application.Images.Get;

public sealed class ImageResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required ImageFileType ImageFileType { get; set; }
    public required string Path { get; set; }
    public int Height { get; set; } // Dimensions in pixels     
    public int Width { get; set; }  // Dimensions in pixels
    public long Size { get; set; }  // Size in bytes
    public Guid? UserId { get; set; }
}
