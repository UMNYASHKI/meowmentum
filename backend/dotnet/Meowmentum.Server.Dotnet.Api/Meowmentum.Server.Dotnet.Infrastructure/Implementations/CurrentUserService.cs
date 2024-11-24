using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Meowmentum.Server.Dotnet.Infrastructure.Implementations;

public class CurrentUserService(UserManager<AppUser> userManager, IHttpContextAccessor contextAccessor) : ICurrentUserService
{
    public async Task<Result<AppUser>> GetCurrentUser(CancellationToken ct = default)
    {
        try
        {
            var principal = contextAccessor?.HttpContext?.User;
            if (principal == null)
            {
                return Result.Failure<AppUser>(ResultMessages.User.NoCurrentUser);
            }

            var user = await userManager.GetUserAsync(principal);
            if (user is null)
            {
                return Result.Failure<AppUser>(ResultMessages.User.FailedToGetCurrentUser);
            }

            return Result.Success(user);
        }
        catch (Exception ex)
        {
            return Result.Failure<AppUser>($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<Result<long>> GetCurrentUserId(CancellationToken ct = default)
    {
        try
        {
            var userResult = await GetCurrentUser(ct);
            if (!userResult.IsSuccess)
            {
                return Result.Failure<long>(ResultMessages.User.FailedToGetUserId);
            }

            return Result.Success(userResult.Data.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<long>($"Unexpected error: {ex.Message}");
        }
    }
}
