using AutoMapper;
using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Business.Implementations;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Shared.Requests;
using Meowmentum.Server.Dotnet.Shared.Requests.Task;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meowmentum.Server.Dotnet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TimerController(ITimeService timeService) : BaseController()
    {
        [HttpPost("start")]
        public async Task<IActionResult> StartTimer(long taskId, CancellationToken ct = default)
        {
            var result = await timeService.StartTimerAsync(CurrentUserId, taskId, ct);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopTimer(long taskId, CancellationToken ct = default)
        {
            var result = await timeService.StopTimerAsync(CurrentUserId, taskId, ct);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("log")]
        public async Task<IActionResult> ManualLogTime([FromBody] ManualLogRequest logRequest, CancellationToken ct)
        {
            var result = await timeService.ManualLogTimeAsync(CurrentUserId, logRequest, ct);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTimer(long id, TimerUpdateRequest updateRequest, CancellationToken ct = default)
        {
            var result = await timeService.UpdateTimerAsync(id, CurrentUserId, updateRequest, ct);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimer(long id, CancellationToken ct = default)
        {
            var result = await timeService.DeleteTimerAsync(CurrentUserId, id, ct);

            if (result.IsSuccess)
                return Ok(result.Data);

            return BadRequest(result.ErrorMessage);
        }

        [HttpGet]
        public async Task<IActionResult> GetTimers([FromQuery] long? taskId, [FromQuery] long? timeIntervalId, CancellationToken ct = default)
        {
            var result = await timeService.GetTimersAsync(CurrentUserId, taskId, timeIntervalId, ct);

            if (result.IsSuccess)
                return Ok(result.Data);

            return BadRequest(result.ErrorMessage);
        }
    }
}
