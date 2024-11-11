using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Shared.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Meowmentum.Server.Dotnet.Infrastructure.Implementations;

public class JwtService(IOptions<JwtOptions> jwtSettingOptions) : ITokenService
{
    private readonly JwtOptions _jwtOptions = jwtSettingOptions.Value;

    public string GetToken(AppUser user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.Id.ToString()),
            new (ClaimTypes.Name, user.UserName!),
            new (ClaimTypes.Email, user.Email!),
        };

        var expires = DateTime.UtcNow.AddDays(_jwtOptions.TokenValidityFromDays);

        var secToken = new JwtSecurityToken(_jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: expires,
            signingCredentials: credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(secToken);

        return token;
    }
}
