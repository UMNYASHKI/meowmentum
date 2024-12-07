using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Task = Meowmentum.Server.Dotnet.Core.Entities.Task;

namespace Meowmentum.Server.Dotnet.Persistence;

public class ApplicationDbContext(DbContextOptions options)
        : IdentityDbContext<AppUser, IdentityRole<long>, long>(options)
{
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TaskTag> TaskTags { get; set; }
    public DbSet<TimeInterval> TimeIntervals { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {

        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

         
        base.OnModelCreating(builder);

        builder.Entity<Task>()
                .HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TaskTag>()
            .HasKey(tt => new { tt.TaskId, tt.TagId });

        builder.Entity<TaskTag>()
            .HasOne(tt => tt.Task)
            .WithMany(t => t.TaskTags)
            .HasForeignKey(tt => tt.TaskId);

        builder.Entity<TaskTag>()
            .HasOne(tt => tt.Tag)
            .WithMany(t => t.TaskTags)
            .HasForeignKey(tt => tt.TagId);

        builder.Entity<TimeInterval>()
            .HasOne(ti => ti.Task)
            .WithMany(t => t.TimeIntervals)
            .HasForeignKey(ti => ti.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
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
