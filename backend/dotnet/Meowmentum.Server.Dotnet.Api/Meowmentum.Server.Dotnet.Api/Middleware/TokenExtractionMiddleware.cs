namespace Meowmentum.Server.Dotnet.Api.Middleware;

public class TokenExtractionMiddleware
{
    private readonly RequestDelegate _next;

    public TokenExtractionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Replace("Bearer ", string.Empty);
            context.Items["JwtToken"] = token;
        }

        await _next(context);
    }
}
