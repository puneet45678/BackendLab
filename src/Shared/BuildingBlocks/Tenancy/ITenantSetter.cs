namespace BuildingBlocks.Tenancy;

public interface ITenantSetter
{
    void SetTenant(Guid tenantId);
}
