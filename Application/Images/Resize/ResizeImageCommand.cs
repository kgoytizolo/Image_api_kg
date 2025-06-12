using Application.Abstractions.Messaging;
using Domain.Images;

namespace Application.Images.Resize
{
    public sealed class ResizeImageCommand : ICommand<string>
    {
        public required Guid Id { get; set; }
        public required string FileName { get; set; }
        public string? Description { get; set; }
        public required ImageFileType ImageFileType { get; set; }
        public required PredefinedImages PredefinedImage { get; set; }
        public required string Path { get; set; }
        public required string ContentType { get; set; }
        public int Height { get; set; } // Dimensions in pixels     
        public int Width { get; set; }  // Dimensions in pixels
        public long Size { get; set; }  // Size in bytes
        public Guid? UserId { get; set; }
    }
}
