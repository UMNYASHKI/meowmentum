using AutoMapper;
using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Infrastructure.Implementations;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Meowmentum.Server.Dotnet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController() : ControllerBase
    {
        private ICurrentUserService _currentUserService;
        private IMapper _mapper;

        protected ICurrentUserService CurrentUserService =>
            _currentUserService ??= HttpContext.RequestServices.GetService<ICurrentUserService>();

        protected IMapper Mapper =>
            _mapper ??= HttpContext.RequestServices.GetService<IMapper>();

        protected AppUser CurrentUser => CurrentUserService.GetCurrentUser().Result.Data;
        protected long CurrentUserId => CurrentUserService.GetCurrentUserId().Result.Data;

        protected ActionResult HandleResult<T>(Result<T> result)
        {
            if (result == null)
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);

            if (result.IsSuccess && result.Data != null)
                return Ok(result.Data);

            if (result.IsSuccess && result.Data == null)
                return NotFound();

            return BadRequest(result.Message);
        }
    }
}
