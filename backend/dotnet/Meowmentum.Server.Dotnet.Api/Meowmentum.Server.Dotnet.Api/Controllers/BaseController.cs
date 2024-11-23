using AutoMapper;
using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Infrastructure.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Meowmentum.Server.Dotnet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController(ICurrentUserService currentUserService, IMapper mapper) : ControllerBase
    {
        protected AppUser CurrentUser => currentUserService.GetCurrentUser().Result.Data;
        protected long CurrentUserId => currentUserService.GetCurrentUserId().Result.Data;
        protected IMapper Mapper => mapper;
    }
}
