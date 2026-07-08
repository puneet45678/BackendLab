using MediatR;
using UserService.Application.Common.Models;

namespace UserService.Application.Users.Commands.RegisterUser;

public record RegisterUserCommand(
    string FullName,
    string Email
) : IRequest<Result<UserDto>>;