using Meowmentum.Server.Dotnet.Shared.Requests;
using Meowmentum.Server.Dotnet.Shared.Responses;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Business.Abstractions;

public interface IReportService
{
    public Task<Result<FileContentResult>> GenerateCompletedTasksReport(DateTime startDate, DateTime endDate, long userId, CancellationToken ct);
    public Task<Result<FileContentResult>> GenerateTagReport(DateTime startDate, DateTime endDate, long userId, CancellationToken ct);
    public Task<Result<FileContentResult>> GenerateDeadlineReport(long userId, DateTime startDate, DateTime endDate, CancellationToken ct);
}
