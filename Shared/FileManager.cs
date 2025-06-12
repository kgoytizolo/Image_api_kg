using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Shared;

public class FileManager
{
    public static Result<bool> ResizeImageFile(string folderPath, string fileName, string originalFileName,
            int desiredHeight, Guid imageId, string imageDefinition)
    {
        string fullPath = Path.Combine(folderPath, fileName);
        var fileExists = EnsureFileExists(fullPath);

        if (!fileExists.IsSuccess)
        {
            return Result.Failure<bool>(fileExists.Error);
        }
        else
        {
            try
            {
                string newfullPath = Path.Combine(folderPath, $"{imageId}_{imageDefinition}_{originalFileName}");
                using (Image originalImage = Image.Load(fullPath))
                {
                    bool isVerticalRatio = (originalImage.Height > originalImage.Width) ? true : false;
                    var (newWidth, newHeight, isOkResponse) = GetImageSizeByRatio(
                        height: desiredHeight, isVerticalRatio: isVerticalRatio);
                    if (!isOkResponse)
                    {
                        return Result.Failure<bool>(new Error("InvalidDimensions", "Failed to calculate image dimensions, this operation is not allowed.", ErrorType.Failure));
                    }
                    if (originalImage.Height < newHeight)
                    {
                        return Result.Failure<bool>(new Error("InvalidDimensions", "New image height is greater than the original height, this operation is not allowed.", ErrorType.Failure));
                    }
                    if (originalImage.Width == originalImage.Height) 
                    { 
                        newWidth = desiredHeight;
                    }
                    originalImage.Mutate(x => x.Resize(newWidth, newHeight));
                    originalImage.Save(newfullPath);
                }
                return Result<bool>.Success(true);
             }
             catch (Exception ex)
             {
                return Result.Failure<bool>(new Error(ex.HResult.ToString(), ex.Message, ErrorType.Failure));
             }
        }
    }

    public static async Task<Result<bool>> UploadFileToFolder(string folderPath, string fileName, IFormFile file)
    {
        var folderExists = EnsureFolderExists(folderPath);

        if (folderExists.IsSuccess)
        {
            if (file.Length == 0)
            {
                return Result.Failure<bool>(new Error("Empty ", "The uploaded file is empty.", ErrorType.Failure));
            }
            else
            {
                try
                {
                    string filePath = Path.Combine(folderPath, fileName);
                    using FileStream stream = new(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);
                    return Result<bool>.Success(true);
                }
                catch (Exception ex)
                {
                    return Result.Failure<bool>(new Error(ex.HResult.ToString(), ex.Message, ErrorType.Failure));
                }
            }
        }
        else 
        {
            return Result.Failure<bool>(folderExists.Error);
        }
    }

    public static string GenerateFilePath(string folderPath, string fileName)
    {
        return Path.Combine(folderPath, fileName);
    }

    private static Result<bool> EnsureFolderExists(string folderPath)
    {
        if (string.IsNullOrEmpty(folderPath) && !Directory.Exists(folderPath))
        {
            try
            {
                Directory.CreateDirectory(folderPath);
                return Result<bool>.Success(true);
            }
            catch (IOException ioEx)
            {
                return Result.Failure<bool>(new Error(ioEx.HResult.ToString(), ioEx.Message, ErrorType.Failure));
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>(new Error(ex.HResult.ToString(), ex.Message, ErrorType.Failure));
            }
        }
        return Result<bool>.Success(true);
    }

    private static Result<bool> EnsureFileExists(string fullPath)
    {
        if (string.IsNullOrEmpty(fullPath) && !File.Exists(fullPath))
        {
            return Result.Failure<bool>(new Error("FileNotFound", "The specified file does not exist.", ErrorType.Failure));
        }
        return Result<bool>.Success(true);
    }

    private static (int width, int height, bool isOk) GetImageSizeByRatio(
        int? width = null, int? height = null, bool isVerticalRatio = true)
    {
        const double verticalRatio = 2.0 / 3.0;
        const double horizontalRatio = 3.0 / 2.0;
        double ratio = isVerticalRatio ? verticalRatio : horizontalRatio;

        if (width.HasValue)
        {
            int w = width.Value;
            int h = (int)Math.Round(w / ratio);
            return (w, h, true);
        }
        else if (height.HasValue)
        {
            int h = height.Value;
            int w = (int)Math.Round(h * ratio);
            return (w, h, true);
        }
        else
        {
            return (0, 0, false);
        }
    }

}
