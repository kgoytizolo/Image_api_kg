using Application.Images.Get;
using Application.Images.Resize;
using Application.Images.Upload;
using Domain.Images;
using ImageWebApi.Request.Images;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ImageWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(IMediator mediator, ILogger<ImagesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("upload")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Response.Images.Response>> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogError("File upload failed: No file provided.");
                return BadRequest("No file provided.");
            }
            if (file.Length > Image.MaxSizeInBytes)
            {
                _logger.LogError("File upload failed: File size exceeds the limit of 10 MB.");
                return BadRequest("File size exceeds the limit of 10 MB.");
            }
            if (string.IsNullOrEmpty(file.ContentType)) 
            {
                _logger.LogError("File upload failed: Invalid image file type '{ContentType}'.", file.ContentType);
                return BadRequest($"Invalid image file type '{file.ContentType}'.");
            }
            if (string.IsNullOrEmpty(file.FileName)) 
            { 
                _logger.LogError("File upload failed: File name is empty.");
                return BadRequest("File name is empty.");
            }
            _logger.LogInformation("Upload method called with file: {FileName}", file?.FileName);

            var command = new UploadImageCommand()
            {
                Id = Guid.NewGuid(),
                ImageFileType = Domain.Images.Image.DetermineImageFileType(file.ContentType),
                Path = GenerateFolderPath(),
                UploadedFile = file,
                UserId = new Guid("132dc49d-5925-463e-bb39-d86e26cdf6d4")   // To be replaced with actual user ID
            };
            var result = await _mediator.Send(command);
            
            if (result.IsSuccess)
            {
                string? newPath = result?.Value;
                Response.Images.Response response = 
                    new Response.Images.Response { ImageId = command.Id, Path = newPath ?? string.Empty };
                string uri = $"https://www.example.com/api/values/{response.ImageId}" + $"?version=\"1.0\"";
                _logger.LogInformation("The file and its copy has been created successfully");
                return Created(uri, response);
            }
            else
            {
                _logger.LogError("File upload failed.");
                return Problem(
                    "There was a problem whilst the image has been uploaded", "", StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("resize/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Response.Images.Response>> ResizeUploadedImage(Guid id,
            [FromBody] ResizeImageRequest resizeImageRequest)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Invalid ID provided for image resizing.");
                return BadRequest("Invalid ID provided.");
            }
            if (resizeImageRequest == null) 
            {
                _logger.LogError("ResizeImageRequest is null.");
                return BadRequest("ResizeImageRequest cannot be null.");
            }
            if (string.IsNullOrEmpty(resizeImageRequest?.TypeOfImageToResize))
            {
                _logger.LogError("Invalid image type provided for resizing.");
                return BadRequest("Invalid image type provided.");
            }
            if (resizeImageRequest?.DesiredHeight <= 0)
            {
                _logger.LogError("Invalid desired height provided for image resizing.");
                return BadRequest("Invalid desired height provided.");
            }

            _logger.LogInformation("Resize method called with file Id: {id}", id);

            var query = new GetImageQuery(id);
            var queryResult = await _mediator.Send(query);

            if (!queryResult.IsSuccess || queryResult.Value == null)
            {
                _logger.LogError("Image with ID {id} not found or internal error.", id);
                return NotFound(queryResult.Error);
            }

            var imageFound = queryResult.Value;
            PredefinedImages definedImage = Enum.Parse<PredefinedImages>(resizeImageRequest.TypeOfImageToResize, true);
            int newHeight = definedImage == PredefinedImages.Customized ? resizeImageRequest.DesiredHeight
                : Image.GetHeightIfPredefinedImageHasBeenSelected(definedImage);

            var commmand = new ResizeImageCommand
            {
                Id = id,
                PredefinedImage = definedImage,
                Path = GenerateFolderPath(),
                Size = imageFound.Size,
                Height = newHeight,
                FileName = imageFound.Name,
                ImageFileType = imageFound.ImageFileType,
                ContentType = Image.DetermineContentType(imageFound.ImageFileType),
                UserId = imageFound.UserId,
                Description = imageFound.Description
            };

            var result = await _mediator.Send(commmand);

            if (result.IsFailure)
            {
                return Problem(
                    $"There was an error during images's resizing. Error Code: {result.Error.Code}" +
                    $", Description: {result.Error.Description}",
                    "",
                    StatusCodes.Status500InternalServerError);
            }
            if (result.Value == null)
            {
                _logger.LogError("Image with ID {id} could not be resized.", id);
                return NotFound($"Image with ID {id} could not be resized.");
            }

            Response.Images.Response finalResponse = new Response.Images.Response
            {
                ImageId = id,
                Path = result.Value
            };

            _logger.LogInformation("Image with ID {id} has been resized successfully.", id);
            return Ok(finalResponse);
        }

        private string GenerateFolderPath() 
        {
            return Path.Combine(Directory.GetCurrentDirectory(), Image.BaseDirectory);
        }

    }
}
