using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Tenancy;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ITenantSetter tenantSetter)
    {
        var claim = context.User.FindFirst("tenant_id");

        if (claim != null && Guid.TryParse(claim.Value, out var tenantId))
            tenantSetter.SetTenant(tenantId);  

        await _next(context);
    }
}