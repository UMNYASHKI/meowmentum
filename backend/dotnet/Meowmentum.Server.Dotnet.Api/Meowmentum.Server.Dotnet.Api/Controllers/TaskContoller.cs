using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Requests.Task;
using Microsoft.AspNetCore.Authorization;
using Meowmentum.Server.Dotnet.Shared.Results;

namespace Meowmentum.Server.Dotnet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController(ITaskService taskService) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest createTaskRequest, CancellationToken ct = default)
        {
            var result = await taskService.CreateTaskAsync(createTaskRequest, ct);

            if (result.IsSuccess)
                return Ok(result.Data);

            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(long id, [FromBody] CreateTaskRequest updateRequest, CancellationToken ct = default)
        {
            var result = await taskService.UpdateTaskAsync(id, updateRequest, ct);

            if (result.IsSuccess)
                return Ok(result.Data);

            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(long id, CancellationToken ct = default)
        {
            var result = await taskService.DeleteTaskAsync(id, ct);

            if (result.IsSuccess)
                return NoContent();

            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("tasks")]
        public async Task<IActionResult> GetTasks([FromQuery] TaskFilterRequest filterRequest, CancellationToken ct = default)
        {
            var result = await taskService.GetTasksAsync(filterRequest, ct);

            if (result.IsSuccess)
                return Ok(result.Data);

            return BadRequest(result.ErrorMessage);
        }
    }
}
