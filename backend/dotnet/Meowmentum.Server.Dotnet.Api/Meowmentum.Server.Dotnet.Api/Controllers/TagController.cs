using Meowmentum.Server.Dotnet.Api.Helpers;
using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meowmentum.Server.Dotnet.Api.Controllers;

[ServiceFilter(typeof(UserAuthorizationFilter))]
[Route("api/users/{userId}/[controller]")]
[ApiController]
[Authorize]
[ValidateModel]
public class TagController(ITagService tagService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromRoute] long userId, CancellationToken ct = default)
    {
        var result = await tagService.GetAllAsync(userId, ct);

        if (result.IsSuccess)
            return Ok(result.Data);

        return BadRequest(result.ErrorMessage);
    }

    [HttpGet("{tagId}")]
    public async Task<IActionResult> GetById([FromRoute] long userId, long tagId, CancellationToken ct = default)
    {
        var result = await tagService.GetByIdAsync(userId, tagId, ct);

        if (result.IsSuccess)
            return Ok(result.Data);

        return NotFound(result.ErrorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromRoute] long userId, [FromBody] TagRequest request, CancellationToken ct = default)
    {
        var result = await tagService.CreateAsync(userId, request, ct);

        if (result.IsSuccess)
            return Ok();

        return BadRequest(result.ErrorMessage);
    }

    [HttpPut("{tagId}")]
    public async Task<IActionResult> Update([FromRoute] long userId, long tagId, [FromBody] TagRequest request, CancellationToken ct = default)
    {
        var result = await tagService.UpdateAsync(userId, tagId, request, ct);

        if (result.IsSuccess)
            return Ok();

        return BadRequest(result.ErrorMessage);
    }

    [HttpDelete("{tagId}")]
    public async Task<IActionResult> Delete([FromRoute] long userId, long tagId, CancellationToken ct = default)
    {
        var result = await tagService.DeleteAsync(userId, tagId, ct);

        if (result.IsSuccess)
            return Ok();

        return BadRequest(result.ErrorMessage);
    }
}
