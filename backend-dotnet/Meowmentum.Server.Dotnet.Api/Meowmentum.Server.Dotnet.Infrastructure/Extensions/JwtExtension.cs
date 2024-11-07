﻿using Meowmentum.Server.Dotnet.Infrastructure.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Meowmentum.Server.Dotnet.Infrastructure.Extensions;

public static class JwtExtension
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtIssuer = configuration.GetSection("JwtSettings:Issuer").Get<string>();
        var jwtAudience = configuration.GetSection("JwtSettings:Audience").Get<string>();
        var jwtKey = configuration.GetSection("JwtSettings:Key").Get<string>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? ""))
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var tokenString = context.SecurityToken switch
                        {
                            JwtSecurityToken jwtToken => jwtToken.RawData,
                            JsonWebToken jsonWebToken => jsonWebToken.EncodedToken,
                            _ => null
                        };

                        if (string.IsNullOrEmpty(tokenString))
                        {
                            context.Fail(ResultMessages.User.InvalidToken);
                            return;
                        }

                        var tokenBlackListManager = context.HttpContext.RequestServices.GetRequiredService<ITokenBlackListManager>();
                        var isBlacklisted = await tokenBlackListManager.IsTokenBlacklisted(tokenString, context.HttpContext.RequestAborted);

                        if (isBlacklisted.IsSuccess && isBlacklisted.Data)
                        {
                            context.Fail(ResultMessages.User.TokenBlacklisted);
                            return;
                        }
                    }
                };
            });

        return services;
    }
}