using Contracts.Common;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence;

public class UserDbContext : DbContext
{
    private readonly ITenantContext _tenant;

    public DbSet<User> Users => Set<User>();

    public UserDbContext(DbContextOptions<UserDbContext> options, ITenantContext tenant)
        : base(options)
    {
        _tenant = tenant;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(u => new { u.Email, u.TenantId }).IsUnique();

            // global query filter — every query auto-scoped to tenant + soft delete
            entity.HasQueryFilter(u =>
                u.TenantId == _tenant.TenantId && !u.IsDeleted);
        });
    }
}