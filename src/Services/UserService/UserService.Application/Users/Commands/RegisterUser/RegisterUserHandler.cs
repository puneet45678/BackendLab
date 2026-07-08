using Contracts.Common;
using MediatR;
using UserService.Application.Common.Models;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;

namespace UserService.Application.Users.Commands.RegisterUser;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _repo;
    private readonly ITenantContext _tenant;

    public RegisterUserHandler(IUserRepository repo, ITenantContext tenant)
    {
        _repo = repo;
        _tenant = tenant;
    }

    public async Task<Result<UserDto>> Handle(RegisterUserCommand cmd, CancellationToken ct)
    {
        if (await _repo.ExistsByEmailAsync(cmd.Email, ct))
            return Result<UserDto>.Failure("Email already registered for this tenant");

        var user = User.Create(cmd.FullName, cmd.Email, _tenant.TenantId);

        await _repo.AddAsync(user, ct);

        return Result<UserDto>.Success(new UserDto(
            user.Id,
            user.FullName,
            user.Email,
            user.TenantId,
            user.CreatedAt
        ));
    }
}