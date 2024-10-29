using Microsoft.OpenApi.Models;
using System.Reflection;
using Meowmentum.Server.Dotnet.Shared.Constants;

namespace Meowmentum.Server.Dotnet.Api.Extensions;

public static class SwaggerGenerationExtension
{
    public static IServiceCollection AddSwaggerGeneration(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // TODO: If we add in the future summary comments for each endpoint of api
            // options.IncludeXmlComments(GetSwaggerXmlCommentsDocumentPath(), includeControllerXmlComments: true);

            var playerAuthenticationSchema =
                GetJwtBearerOpenApiSecuritySchemeByAuthenticationSchema(AuthenticationSchema.User);

            options.AddSecurityDefinition(AuthenticationSchema.User, playerAuthenticationSchema);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    playerAuthenticationSchema,
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    private static OpenApiSecurityScheme GetJwtBearerOpenApiSecuritySchemeByAuthenticationSchema(string authenticationSchema)
    {
        return new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = authenticationSchema
            },
            Name = authenticationSchema,
            Description = $"JWT Authorization for {authenticationSchema}. Example: \"Bearer {{token}}\"",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT"
        };
    }

    private static string GetSwaggerXmlCommentsDocumentPath()
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

        return xmlPath;
    }
}