using Grpc.Core;
using Grpc.Net.ClientFactory;
using Meowmentum.Server.Dotnet.Proto.Email;
using Meowmentum.Server.Dotnet.Shared.Options.Grpc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Meowmentum.Server.Dotnet.Infrastructure.Extensions;

public static class GrpcConfiguration
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        var emailOptions = configuration.GetSection(EmailOptions.SectionName).Get<EmailOptions>();
        services.AddGrpcClient<EmailService.EmailServiceClient>(o =>
        {
            o.Address = new Uri(emailOptions.Address);
        })
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            return handler;
        });

        return services;
    }
    
    public static IServiceCollection AddGrpcServices(this IServiceCollection services)
    {
        return services;
    }
}
