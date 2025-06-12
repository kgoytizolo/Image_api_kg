using Application.Abstractions.Messaging;

namespace Application.Images.Get;

public sealed record GetImageQuery(Guid ImageId) : IQuery<ImageResponse>;