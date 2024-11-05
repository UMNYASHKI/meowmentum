using System.Net;

namespace Meowmentum.Server.Dotnet.Shared.Responses;

public record ExceptionResponse(HttpStatusCode StatusCode, string Description);
