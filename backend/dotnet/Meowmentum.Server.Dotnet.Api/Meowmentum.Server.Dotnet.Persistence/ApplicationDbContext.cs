using Meowmentum.Server.Dotnet.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Task = Meowmentum.Server.Dotnet.Core.Entities.Task;

namespace Meowmentum.Server.Dotnet.Persistence;

public class ApplicationDbContext(DbContextOptions options)
        : IdentityDbContext<AppUser, IdentityRole<long>, long>(options)
{
    public DbSet<Task> Tasks { get; set; }
    public DbSet<TimeInterval> TimeIntervals { get; set; }
    public DbSet<Tag> Tags { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {

        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

         
        base.OnModelCreating(builder);

        builder.Entity<Task>()
                .HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Task>()
            .HasOne(t => t.Tag)
            .WithMany()
            .HasForeignKey(t => t.TagId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TimeInterval>()
            .HasOne(ti => ti.Task)
            .WithMany(t => t.TimeIntervals)
            .HasForeignKey(ti => ti.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
