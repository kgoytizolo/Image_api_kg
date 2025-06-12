using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Images;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Application.Images.Get;
internal sealed class GetImageQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetImageQuery, ImageResponse>
{
    public async Task<Result<ImageResponse>> Handle(GetImageQuery query, CancellationToken cancellationToken)
    {
        ImageResponse? image = await context.Images
            .Where(i => i.Id == query.ImageId)
            .Select(i => new ImageResponse
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                Path = i.Path,
                UserId = i.UserId,
                ImageFileType = i.ImageFileType,
                Height = i.Height,
                Width = i.Width,
                Size = i.Size
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (image is null)
        {
            return Result.Failure<ImageResponse>(ImageErrors.NotFound(query.ImageId));
        }
        return Result.Success(image);
    }
}
