using AutoMapper;
using Meowmentum.Server.Dotnet.Api.Helpers;
using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Shared.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meowmentum.Server.Dotnet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[ValidateModel]
public class TagController(ITagService tagService) : BaseController()
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
    {
        var result = await tagService.GetAllAsync(CurrentUserId, ct);

        if (result.IsSuccess)
            return Ok(result.Data);

        return BadRequest(result.ErrorMessage);
    }

    [HttpGet("{tagId}")]
    public async Task<IActionResult> GetById(long tagId, CancellationToken ct = default)
    {
        var result = await tagService.GetByIdAsync(CurrentUserId, tagId, ct);

        if (result.IsSuccess)
            return Ok(result.Data);

        return NotFound(result.ErrorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TagRequest request, CancellationToken ct = default)
    {
        var tag = Mapper.Map<Tag>(request);
        var result = await tagService.CreateAsync(CurrentUserId, tag, ct);

        if (result.IsSuccess)
            return Ok();

        return BadRequest(result.ErrorMessage);
    }

    [HttpPut("{tagId}")]
    public async Task<IActionResult> Update(long tagId, [FromBody] TagRequest request, CancellationToken ct = default)
    {
        var tag = Mapper.Map<Tag>(request);
        var result = await tagService.UpdateAsync(CurrentUserId, tagId, tag, ct);

        if (result.IsSuccess)
            return Ok();

        return BadRequest(result.ErrorMessage);
    }

    [HttpDelete("{tagId}")]
    public async Task<IActionResult> Delete(long tagId, CancellationToken ct = default)
    {
        var result = await tagService.DeleteAsync(CurrentUserId, tagId, ct);

        if (result.IsSuccess)
            return Ok();

        return BadRequest(result.ErrorMessage);
    }
}
