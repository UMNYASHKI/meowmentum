using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Infrastructure.Extensions;
using Meowmentum.Server.Dotnet.Infrastructure.Helpers;
using Meowmentum.Server.Dotnet.Infrastructure.Implementations;
using Meowmentum.Server.Dotnet.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Meowmentum.Server.Dotnet.Infrastructure;

public static class RegisterLayerExtension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddJwtAuthentication(configuration);
        services.AddScoped<ITokenService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IOtpManager, OtpManager>();

        services.AddMemoryCache();
    }
}

