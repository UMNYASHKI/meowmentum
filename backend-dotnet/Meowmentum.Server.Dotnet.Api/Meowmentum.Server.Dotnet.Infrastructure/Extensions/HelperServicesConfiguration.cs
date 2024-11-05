using Meowmentum.Server.Dotnet.Infrastructure.Abstractions;
using Meowmentum.Server.Dotnet.Infrastructure.HelperServices;
using Meowmentum.Server.Dotnet.Shared.Options.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Infrastructure.Extensions;

public static class HelperServicesConfiguration
{
    public static IServiceCollection AddHelperServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OtpDbConfig>(configuration.GetSection(OtpDbConfig.SectionName));
        services.AddScoped<IOtpManager, OtpManager>();

        services.Configure<TokenBlacklistDbConfig>(configuration.GetSection(TokenBlacklistDbConfig.SectionName));
        services.AddScoped<ITokenBlackListManager, TokenBlackListManger>();

        return services;
    }
}
