using Application.Images.Get;
using Application.Images.Resize;
using Application.Images.Upload;
using Domain.Images;
using ImageWebApi.Controllers;
using ImageWebApi.Request.Images;
using ImageWebApi.Response.Images;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Shared;

namespace ImageAppTest.Controllers
{
    public class ImagesControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<ImagesController>> _loggerMock;
        private readonly ImagesController _controller;

        public ImagesControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<ImagesController>>();
            _controller = new ImagesController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Upload_Image_ReturnsResult_Created()
        {
            // Arrange
            string newPath = "Images/myImage.jpg";

            _mediatorMock.Setup(m => m.Send(It.IsAny<UploadImageCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(newPath);

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(6000);
            fileMock.Setup(f => f.ContentType).Returns("image/png");
            fileMock.Setup(f => f.FileName).Returns("myImage.jpg");

            // Act
            var response = await _controller.Upload(fileMock.Object);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(response?.Result);
        }

        [Fact]
        public async Task Upload_Image_ReturnsResult_InternalServerError()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UploadImageCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<string>(
                    Error.Failure("Upload Image Error","Error when uploading the image")));

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(6000);
            fileMock.Setup(f => f.ContentType).Returns("image/png");
            fileMock.Setup(f => f.FileName).Returns("myImage.jpg");

            // Act
            var response = await _controller.Upload(fileMock.Object);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(response?.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult?.StatusCode);
        }

        [Fact]
        public async Task Upload_ReturnsBadRequest_WhenFileIsNull()
        {
            // Act
            var result = await _controller.Upload(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Upload_ReturnsBadRequest_WhenFileIsEmpty()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);

            var result = await _controller.Upload(fileMock.Object);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Upload_ReturnsBadRequest_WhenFileSizeExceedsLimit()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(Image.MaxSizeInBytes + 1);

            var result = await _controller.Upload(fileMock.Object);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Upload_ReturnsBadRequest_WhenContentTypeIsEmpty()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(100);
            fileMock.Setup(f => f.ContentType).Returns(string.Empty);

            var result = await _controller.Upload(fileMock.Object);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Upload_ReturnsBadRequest_WhenFileNameIsEmpty()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(100);
            fileMock.Setup(f => f.ContentType).Returns("image/png");
            fileMock.Setup(f => f.FileName).Returns(string.Empty);

            var result = await _controller.Upload(fileMock.Object);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ResizeUploadedImage_ReturnsBadRequest_WhenIdIsEmpty()
        {
            var request = new ResizeImageRequest { TypeOfImageToResize = "Customized", DesiredHeight = 100 };
            var result = await _controller.ResizeUploadedImage(Guid.Empty, request);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ResizeUploadedImage_ReturnsBadRequest_WhenRequestIsNull()
        {
            var result = await _controller.ResizeUploadedImage(Guid.NewGuid(), null);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ResizeUploadedImage_ReturnsBadRequest_WhenTypeOfImageToResizeIsEmpty()
        {
            var request = new ResizeImageRequest { TypeOfImageToResize = "", DesiredHeight = 100 };
            var result = await _controller.ResizeUploadedImage(Guid.NewGuid(), request);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ResizeUploadedImage_ReturnsBadRequest_WhenDesiredHeightIsInvalid()
        {
            var request = new ResizeImageRequest { TypeOfImageToResize = "Customized", DesiredHeight = 0 };
            var result = await _controller.ResizeUploadedImage(Guid.NewGuid(), request);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ResizeUploadedImage_ReturnsNotFound_WhenImageNotFound()
        {
            var id = Guid.NewGuid();
            var request = new ResizeImageRequest { TypeOfImageToResize = "Customized", DesiredHeight = 100 };
            // Mock the mediator to return a not found result
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetImageQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Result<ImageResponse>(
                    null, false, Error.NotFound("", "")));

            var result = await _controller.ResizeUploadedImage(id, request);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
    }
}