using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Meowmentum.Server.Dotnet.Api.Controllers;

//[Authorize(Roles=[Role from constants])]
[Route("api/v1/[controller]")]
public class TestController : Controller
{
    [HttpGet]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public IActionResult Index()
    {
        //validation

        //logic

        //result
        return Ok();
    }
}
