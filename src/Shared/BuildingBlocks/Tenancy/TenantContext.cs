using Contracts.Common;

namespace BuildingBlocks.Tenancy;

public class TenantContext : ITenantContext
{
    private Guid? _tenantId;

    public Guid TenantId => _tenantId
        ?? throw new InvalidOperationException("Tenant has not been set for this request.");

    public bool HasTenant => _tenantId.HasValue;

    public void SetTenant(Guid tenantId) => _tenantId = tenantId;
}