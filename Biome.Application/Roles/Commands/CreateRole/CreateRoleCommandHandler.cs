namespace Biome.Application.Roles.Commands.CreateRole;

using Biome.Domain.Roles;
using Biome.SharedKernel.Abstractions;
using Biome.SharedKernel.Primitives;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal sealed class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<Guid>>
{
    private readonly IRoleRepository _roleRepository;

    public CreateRoleCommandHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        // 1. Check if role name already exists
        if (await _roleRepository.GetByNameAsync(request.Name, cancellationToken) is not null)
        {
            return Result.Failure<Guid>(new Error("Role.Exists", $"Role '{request.Name}' already exists."));
        }

        // 2. Create Aggregate
        var role = Role.Create(request.Name, request.Description);

        // 3. Persist
        _roleRepository.Add(role);
        
        // In a real EF implementation, we would await _roleRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        // For InMemory, Add() is sufficient if static dictionary is used.

        return role.Id;
    }
}
