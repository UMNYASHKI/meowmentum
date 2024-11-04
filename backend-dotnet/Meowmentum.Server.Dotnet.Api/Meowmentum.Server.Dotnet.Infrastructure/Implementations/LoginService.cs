using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Shared.Requests;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Infrastructure.Implementations
{
    public class LoginService : ILoginService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _jwtService;

        public LoginService(UserManager<AppUser> userManager, ITokenService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        public async Task<Result<string>> LoginAsync(Shared.Requests.LoginRequest request, CancellationToken token = default)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return Result.Failure<string>("Invalid email or password.");
            }

            var tokenString = _jwtService.GetToken(user);
            return Result.Success(tokenString);
        }
    }
}
