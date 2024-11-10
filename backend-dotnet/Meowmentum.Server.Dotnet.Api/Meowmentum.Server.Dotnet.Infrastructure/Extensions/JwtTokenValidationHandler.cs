using Meowmentum.Server.Dotnet.Infrastructure.Abstractions;
using Meowmentum.Server.Dotnet.Infrastructure.HelperServices;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Infrastructure.Extensions;

public class JwtTokenValidationHandler(ITokenBlackListManager tokenBlackListManager) : JwtBearerEvents
{
    public async Task OnTokenValidated(TokenValidatedContext context)
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

        var isBlacklisted = await tokenBlackListManager.IsTokenBlacklisted(tokenString, context.HttpContext.RequestAborted);

        if (isBlacklisted.IsSuccess && isBlacklisted.Data)
        {
            context.Fail(ResultMessages.User.TokenBlacklisted);
        }
    }
}
