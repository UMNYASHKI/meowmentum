using Meowmentum.Server.Dotnet.Infrastructure.HelperServices.Mappings;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Infrastructure.Extensions;

public static class AutoMapperExtension
{
    public static IServiceCollection AddAutoMapperWithMappings(this IServiceCollection services, string assemblyPrefix)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName != null && a.FullName.StartsWith(assemblyPrefix))
            .ToArray();

        services.AddAutoMapper(config =>
        {
            foreach (var assembly in assemblies)
            {
                config.AddProfile(new AssemblyMappingProfile(assembly));
            }
        });

        return services;
    }
}
