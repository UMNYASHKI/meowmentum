using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Meowmentum.Server.Dotnet.Persistence;

public class ApplicationDbContext(DbContextOptions options)
        : IdentityDbContext<AppUser, IdentityRole<long>, long>(options)
{
    public DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        base.OnModelCreating(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker.Entries()
            .Where(e => e.Entity is ITrackingEntity && (e.State == EntityState.Added || e.State == EntityState.Modified))
            .ToList();

        foreach (var entity in entities)
        {
            var trackingEntity = (ITrackingEntity)entity.Entity;

            if (entity.State == EntityState.Added)
            {
                trackingEntity.CreatedDate = DateTime.UtcNow;
            }

            if (entity.State == EntityState.Modified)
            {
                trackingEntity.UpdatedDate = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
