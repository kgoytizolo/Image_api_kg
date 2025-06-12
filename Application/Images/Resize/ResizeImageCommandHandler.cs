using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Images;
using Shared;

namespace Application.Images.Resize;

internal sealed class ResizeImageCommandHandler(
    IApplicationDbContext context) : ICommandHandler<ResizeImageCommand, string>
{
    public async Task<Result<string>> Handle(ResizeImageCommand command, CancellationToken cancellationToken)
    {
        // Getting the file from the command and validating it
        string newFileNameOriginal = $"{command.Id}_{PredefinedImages.Original.ToString()}_{command.FileName}";
        string newFileNameResized = $"{command.Id}_{command.PredefinedImage.ToString()}_{command.FileName}";
        string basePath = FileManager.GenerateFilePath(Image.BaseDirectory, newFileNameResized);

        var ImageItem = new Image
        {
            Id = command.Id,
            Name = command.FileName ?? "",
            Description = command.Description ?? "",
            ImageFileType = command.ImageFileType,
            ContentType = command.ContentType,
            Path = basePath,
            Height = command.Height,
            Width = command.Width,
            Size = command.Size,
            UserId = command.UserId ?? Guid.Empty
        };

        try 
        {
            context.Images.Update(ImageItem);
            await context.SaveChangesAsync(cancellationToken);

            var resizedFileResult = FileManager.ResizeImageFile(
                command.Path, newFileNameOriginal, command?.FileName, command.Height, command.Id, command.PredefinedImage.ToString());

            if (resizedFileResult.IsSuccess)
            {
                return Result<Guid>.Success(ImageItem.Path);
            }
            else
            {
                return Result.Failure<string>(ImageErrors.ImageResizeFailed(resizedFileResult.Error.Description));
            }
        }
        catch (Exception ex) 
        {
            return Result.Failure<string>(ImageErrors.ImageResizeFailed(ex.Message));
        }

    }
}
