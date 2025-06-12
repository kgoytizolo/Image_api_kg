using Shared;

namespace Domain.Images;

public static class ImageErrors
{
    public static Error NotFound(Guid imageId) => Error.NotFound(
        "Image.NotFound",
        $"Image with ID '{imageId}' was not found."
    );

    public static Error InvalidImageFormat(string format) => Error.Problem(
        "Image.InvalidFormat",
        $"The provided image format '{format}' is not supported."
    );

    public static Error ImageUploadFailed(string reason) => Error.Failure(
        "Image.UploadFailed",
        $"Image upload failed: {reason}"
    );

    public static Error ImageResizeFailed(string reason) => Error.Failure(
        "Image.ResizeFailed",
        $"Image resize failed: {reason}"
    );
}
