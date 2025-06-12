using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Images;
using Shared;

namespace Application.Images.Upload;

internal sealed class UploadImageCommandHandler(
    IApplicationDbContext context) : ICommandHandler<UploadImageCommand, string>
{
    public async Task<Result<string>> Handle(UploadImageCommand command, CancellationToken cancellationToken)
    {
        // Getting the file from the command and validating it
        string newFileName = $"{command.Id}_{command.UploadedFile?.FileName}";
        string newFileNameOriginal = $"{command.Id}_{PredefinedImages.Original.ToString()}_{command.UploadedFile?.FileName}";
        string newPath = FileManager.GenerateFilePath(command.Path, newFileNameOriginal);
        string basePath = FileManager.GenerateFilePath(Image.BaseDirectory, newFileNameOriginal);

        var ImageItem = new Image
        {
            Id = command.Id,
            Name = command.UploadedFile?.FileName ?? "",
            Description = command.Description ?? "",
            ImageFileType = command.ImageFileType,
            ContentType = command.UploadedFile?.ContentType,
            Path = basePath,
            Height = command.Height,
            Width = command.Width,
            Size = command.UploadedFile?.Length ?? 0,
            UserId = command.UserId ?? Guid.Empty
        };

        try
        {
            context.Images.Add(ImageItem);
            await context.SaveChangesAsync(cancellationToken);

            var uploadedFileResult = await FileManager.UploadFileToFolder(command.Path, newFileNameOriginal, command.UploadedFile);
            var uploadedFileCopyResult = await FileManager.UploadFileToFolder(command.Path, newFileName, command.UploadedFile);

            if (uploadedFileResult.IsSuccess && uploadedFileCopyResult.IsSuccess)
            {
                return Result<Guid>.Success(ImageItem.Path);
            }
            else
            {
                string errorMessage = !string.IsNullOrEmpty(uploadedFileResult.Error.Description) ? uploadedFileResult.Error.Description
                                        : uploadedFileCopyResult.Error.Description;
                return Result.Failure<string>(ImageErrors.ImageUploadFailed(errorMessage));
            }
        }
        catch (Exception ex)
        {
            return Result.Failure<string>(ImageErrors.ImageUploadFailed(ex.Message));
        }
    }
}
