using Application.Abstractions.Messaging;
using Domain.Images;
using Microsoft.AspNetCore.Http;

namespace Application.Images.Upload;

public sealed class UploadImageCommand : ICommand<string>
{
    public Guid Id { get; set; } // Unique identifier for the image
    public string? Description { get; set; }
    public required ImageFileType ImageFileType { get; set; }
    public required string Path { get; set; }
    public int Height { get; set; } // Dimensions in pixels     
    public int Width { get; set; }  // Dimensions in pixels
    public IFormFile? UploadedFile { get; set; } // The uploaded file
    public Guid? UserId { get; set; }
}
