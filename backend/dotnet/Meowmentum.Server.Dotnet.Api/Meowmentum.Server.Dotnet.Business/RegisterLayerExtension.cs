
using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Business.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Meowmentum.Server.Dotnet.Business;

public static class RegisterLayerExtension
{
    public static void AddBusiness(this IServiceCollection services)
    {
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<ITaskService, TaskService>();
    }
}

