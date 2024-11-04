using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Shared.Requests;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.AspNetCore.Identity;

public class LoginService : ILoginService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _jwtService;

    public LoginService(UserManager<AppUser> userManager, ITokenService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<Result<string>> LoginAsync(LoginRequest request, CancellationToken token = default)
    {
        try
        {
            token.ThrowIfCancellationRequested();

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return Result.Failure<string>("Invalid email or password.");
            }

            var tokenString = _jwtService.GetToken(user);
            return Result.Success(tokenString);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<string>("Operation was canceled.");
        }
        catch (Exception ex)
        {
            return Result.Failure<string>($"Unexpected error: {ex.Message}");
        }
    }
}
