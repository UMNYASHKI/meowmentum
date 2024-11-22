using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Meowmentum.Server.Dotnet.Api.Helpers;

public class UserAuthorizationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userIdFromRoute = context.ActionArguments["userId"] as long?;

        if (!userIdFromRoute.HasValue)
        {
            context.Result = new BadRequestObjectResult("UserId is required.");
            return;
        }

        var currentUserId = long.Parse(context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        if (userIdFromRoute.Value != currentUserId)
        {
            context.Result = new UnauthorizedObjectResult("You are not authorized to access this resource.");
            return;
        }

        await next();
    }
}
