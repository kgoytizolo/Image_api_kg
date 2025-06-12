using FluentValidation;

namespace Application.Images.Resize;

public class ResizeImageCommandValidator : AbstractValidator<ResizeImageCommand>
{
    public ResizeImageCommandValidator()
    {
        RuleFor(x => x.ImageFileType)
            .IsInEnum().WithMessage("Invalid image file type.");
        RuleFor(x => x.Path)
            .MaximumLength(255)
            .NotEmpty().WithMessage("Path is required.");
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Image ID is required.");
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}
