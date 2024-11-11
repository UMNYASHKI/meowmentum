using Meowmentum.Server.Dotnet.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Meowmentum.Server.Dotnet.Persistence;

public class ApplicationDbContext(DbContextOptions options)
        : IdentityDbContext<AppUser, IdentityRole<long>, long>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        base.OnModelCreating(builder);
    }
}
