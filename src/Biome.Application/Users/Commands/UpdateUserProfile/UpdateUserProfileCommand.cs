using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Users.Commands.UpdateUserProfile;

public sealed record UpdateUserProfileCommand(Guid UserId, string FirstName, string LastName) : IRequest<Result>;
