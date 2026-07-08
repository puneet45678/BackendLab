namespace UserService.Application.Users.Commands.RegisterUser;

public record UserDto(
    Guid Id,
    string FullName,
    string Email,
    Guid TenantId,
    DateTime CreatedAt
);