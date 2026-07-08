using Contracts.Common;
namespace UserService.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    private User() { }

    public static User Create(string fullName, string email, Guid tenantId)
    {
        return new User
        {
            FullName = fullName,
            Email = email,
            TenantId = tenantId
        };
    }
}