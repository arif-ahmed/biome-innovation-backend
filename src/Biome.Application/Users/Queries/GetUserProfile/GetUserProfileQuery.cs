using Biome.Application.Users.Common;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Users.Queries.GetUserProfile;

public sealed record GetUserProfileQuery(Guid UserId) : IRequest<Result<UserProfileResponse>>;
