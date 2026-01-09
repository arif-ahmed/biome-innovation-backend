namespace Biome.Application.Users.Commands.RegisterUser;

using Biome.SharedKernel.Primitives;
using MediatR;

public sealed record RegisterUserCommand(string Email, string Password, string FirstName, string LastName)
    : IRequest<Result<Guid>>;
