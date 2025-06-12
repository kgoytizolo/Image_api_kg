using FluentValidation;
using Domain.Images;

namespace Application.Images.Upload;

public class UploadImageCommandValidator : AbstractValidator<UploadImageCommand>
{
    public UploadImageCommandValidator()
    {
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
        RuleFor(x => x.ImageFileType)
            .IsInEnum().WithMessage("Invalid image file type.");
        RuleFor(x => x.Path)
            .MaximumLength(255)
            .NotEmpty().WithMessage("Path is required.");
    }
}
