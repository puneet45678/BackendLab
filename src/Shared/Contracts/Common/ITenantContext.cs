namespace Contracts.Common;

public interface ITenantContext
{
    Guid TenantId { get; }
    bool HasTenant { get; }
}