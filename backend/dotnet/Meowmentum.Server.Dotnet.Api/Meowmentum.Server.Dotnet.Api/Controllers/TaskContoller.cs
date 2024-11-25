using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Requests.Task;
using Meowmentum.Server.Dotnet.Api.Helpers;
using Task = Meowmentum.Server.Dotnet.Core.Entities.Task;

namespace Meowmentum.Server.Dotnet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[ValidateModel]
public class TasksController(ITaskService taskService, IMapper mapper) : BaseController()
{
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest createTaskRequest, CancellationToken ct = default)
    {
        var task = mapper.Map<Task>(createTaskRequest);
        var result = await taskService.CreateTaskAsync(CurrentUserId, task, ct);

        if (result.IsSuccess)
            return Ok(result.Data);

        return BadRequest(result.ErrorMessage);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(long id, [FromBody] CreateTaskRequest updateRequest, CancellationToken ct = default)
    {
        var task = mapper.Map<Task>(updateRequest);
        task.Id = id;

        var result = await taskService.UpdateTaskAsync(CurrentUserId, task, ct);

        if (result.IsSuccess)
            return Ok(result.Data);

        return BadRequest(result.ErrorMessage);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(long id, CancellationToken ct = default)
    {
        var result = await taskService.DeleteTaskAsync(CurrentUserId, id, ct);

        if (result.IsSuccess)
            return Ok();

        return BadRequest(result.ErrorMessage);
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks([FromQuery] TaskFilterRequest filterRequest, CancellationToken ct = default)
    {
        var result = await taskService.GetTasksAsync(CurrentUserId, filterRequest, ct);

        if (result.IsSuccess)
            return Ok(result.Data);

        return BadRequest(result.ErrorMessage);
    }
}
