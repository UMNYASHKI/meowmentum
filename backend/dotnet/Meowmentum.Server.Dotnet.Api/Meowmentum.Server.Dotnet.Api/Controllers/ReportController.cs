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
        [HttpGet("completed-tasks")]
        public async Task<IActionResult> GetCompletedTasksReport(DateTime startDate, DateTime endDate,CancellationToken ct = default)
        {
            var result = await reportService.GenerateCompletedTasksReport(startDate, endDate, CurrentUserId, ct);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            var fileModel = result.Data;

            return File(fileModel.Content, fileModel.ContentType, fileModel.FileName);
        }

        [HttpGet("tag-report")]
        public async Task<IActionResult> GenerateTasksByTagReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, CancellationToken ct = default)
        {
            var result = await reportService.GenerateTagReport(startDate, endDate, CurrentUserId, ct);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            var fileModel = result.Data;

            return File(fileModel.Content, fileModel.ContentType, fileModel.FileName);
        }

        [HttpGet("deadline-report")]
        public async Task<IActionResult> GenerateDeadlineCompletionReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, CancellationToken ct = default)
        {
            var result = await reportService.GenerateDeadlineReport(CurrentUserId, startDate, endDate, ct);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            var fileModel = result.Data;

            return File(fileModel.Content, fileModel.ContentType, fileModel.FileName);
        }
    }
}
