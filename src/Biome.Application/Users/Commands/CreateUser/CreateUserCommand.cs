namespace Biome.Application.Users.Commands.CreateUser;

using Biome.SharedKernel.Primitives;
using MediatR;

public sealed record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Role) : IRequest<Result<Guid>>;
