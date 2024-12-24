using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Business.Implementations;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meowmentum.Server.Dotnet.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportController(IReportService reportService) : BaseController()
    {
        [HttpPost("completed-tasks-report")]
        public async Task<IActionResult> GenerateCompletedTasksReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, CancellationToken ct = default)
        {
            var result = await reportService.GenerateCompletedTasksReport(startDate, endDate, CurrentUserId, ct);

            if (result.IsSuccess)
                return result.Data;

            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("tag-report")]
        public async Task<IActionResult> GenerateTasksByTagReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, CancellationToken ct = default)
        {
            var result = await reportService.GenerateTagReport(startDate, endDate, CurrentUserId, ct);

            if (result.IsSuccess)
                return result.Data;

            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("deadline-report")]
        public async Task<IActionResult> GenerateDeadlineCompletionReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, CancellationToken ct = default)
        {
            var result = await reportService.GenerateDeadlineReport(CurrentUserId, startDate, endDate, ct);

            if (result.IsSuccess)
                return result.Data;

            return BadRequest(result.ErrorMessage);
        }
    }
}
