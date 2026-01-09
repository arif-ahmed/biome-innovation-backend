using Biome.Application.Users.Common;
using Biome.Domain.Roles;
using Biome.Domain.Users;
using Biome.SharedKernel.Primitives;
using MediatR;

namespace Biome.Application.Users.Queries.GetUserProfile;

internal sealed class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserProfileResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public GetUserProfileQueryHandler(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<Result<UserProfileResponse>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<UserProfileResponse>(new Error("User.NotFound", "User not found."));
        }

        var role = await _roleRepository.GetByIdAsync(user.RoleId, cancellationToken);
        string roleName = role?.Name ?? "Unknown";

        return new UserProfileResponse(
            user.Id,
            user.FirstName.Value,
            user.LastName.Value,
            user.Email.Value,
            roleName,
            user.IsEmailVerified);
    }
}
