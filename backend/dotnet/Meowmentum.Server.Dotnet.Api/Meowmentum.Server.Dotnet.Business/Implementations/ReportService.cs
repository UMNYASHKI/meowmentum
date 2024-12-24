using ClosedXML.Excel;
using Meowmentum.Server.Dotnet.Infrastructure.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Task = Meowmentum.Server.Dotnet.Core.Entities.Task;
using Meowmentum.Server.Dotnet.Business.Abstractions;
using Microsoft.AspNetCore.Mvc;
using TaskStatus = Meowmentum.Server.Dotnet.Core.Entities.TaskStatus;
using Meowmentum.Server.Dotnet.Shared.Results;

namespace Meowmentum.Server.Dotnet.Business.Implementations;

public class ReportService(
    IRepository<AppUser> userRepository,
    IRepository<Task> taskRepository,
    IRepository<Tag> tagRepository,
    IRepository<TimeInterval> timeIntervalRepository) : IReportService
{
    private const string FontNameBold = "Bahnschrift SemiBold";
    private const int HeaderFontSize = 12;

    private void ApplyHeaderStyle(IXLCell cell)
    {
        cell.Style.Font.Bold = true;
        cell.Style.Font.FontSize = HeaderFontSize;
        cell.Style.Font.FontName = FontNameBold;
        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        cell.Style.Fill.SetBackgroundColor(XLColor.LightGray);
        cell.Style.Alignment.WrapText = true;
    }

    private void ApplyTableHeaderStyle(IXLRow row)
    {
        row.Style.Font.Bold = true;
        row.Style.Font.FontName = FontNameBold;
    }

    private void ApplyTableBorder(IXLRange targetRange, XLBorderStyleValues borderStyle)
    {
        if (targetRange == null)
            throw new ArgumentNullException(nameof(targetRange), "The range cannot be null.");

        targetRange.Style.Border.OutsideBorder = borderStyle;

        if (borderStyle != XLBorderStyleValues.Thick)
        {
            targetRange.Style.Border.InsideBorder = borderStyle;
        }
    }

    private void ApplyCellBorders(IXLRange range)
    {
        range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
        range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
        range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
    }

    private void SetRow(IXLRow row, params string[] headers)
    {
        for (int i = 0; i < headers.Length; i++)
        {
            row.Cell(i + 1).Value = headers[i];
        }
    }

    private FileContentResult GenerateExcelReport(XLWorkbook workbook, string fileName)
    {
        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);

        return new FileContentResult(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = fileName
        };
    }

    public async Task<Result<FileContentResult>> GenerateCompletedTasksReport(DateTime startDate, DateTime endDate, long userId, CancellationToken ct = default)
    {
        var userResult = await userRepository.GetByIdAsync(userId);
        var user = userResult.Data;

        var completedTasksResult = await taskRepository.GetAllAsync(
            t => t.UserId == userId && t.Status == TaskStatus.Completed &&
                 t.CompletedAt >= startDate.ToUniversalTime() && t.CompletedAt <= endDate.ToUniversalTime());

        if (!completedTasksResult.IsSuccess || completedTasksResult.Data == null)
            return Result.Failure<FileContentResult>(ResultMessages.Task.NoCompletedTasks);

        var tasks = completedTasksResult.Data;

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Completed Tasks");

        worksheet.Cell(1, 1).Value = $"Completed tasks for user: {user.UserName} from {startDate.ToShortDateString()} to {endDate.ToShortDateString()}";
        ApplyHeaderStyle(worksheet.Cell(1, 1));
        worksheet.Row(1).Height = 30;
        worksheet.Range("A1:F1").Merge();

        SetRow(worksheet.Row(2), "Title", "CreatedAt", "Priority", "Deadline", "CompletedAt", "Time spent (hours)");

        ApplyTableHeaderStyle(worksheet.Row(2));

        int row = 3;
        double globalTimeSpent = 0;

        foreach (var task in tasks)
        {
            var timeIntervalsResult = await timeIntervalRepository.GetAllAsync(
                filter: ti => ti.TaskId == task.Id &&
                              ti.EndTime.HasValue &&
                              ti.EndTime.Value >= startDate.ToUniversalTime() &&
                              ti.EndTime.Value <= endDate.ToUniversalTime()
            );

            if (!timeIntervalsResult.IsSuccess)
                return Result.Failure<FileContentResult>(ResultMessages.Timer.ErrorRetrieve +  $"{task.Id}.");

            var totalTimeSpent = timeIntervalsResult.Data?.Sum(ti => (ti.EndTime.Value - ti.StartTime).TotalHours) ?? 0;
            globalTimeSpent += totalTimeSpent;

            SetRow(worksheet.Row(row),
                task.Title,
                task.CreatedAt.ToString("HH:mm yyyy-MM-dd"),
                task.Priority.ToString() ?? "-",
                task.Deadline?.ToString("HH:mm yyyy-MM-dd"),
                task.CompletedAt?.ToString("HH:mm yyyy-MM-dd"),
                totalTimeSpent.ToString("0.00"));

            row++;
        }

        worksheet.Cell(row, 1).Value = $"Completed Tasks: {tasks.Count()}";
        worksheet.Range(row, 1, row, 5).Merge();
        ApplyHeaderStyle(worksheet.Cell(row, 1));

        worksheet.Cell(row, 6).Value = $"{globalTimeSpent.ToString("0.00")}";
        ApplyHeaderStyle(worksheet.Cell(row, 6));
        worksheet.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

        worksheet.Row(row).Height = 20;
        worksheet.Columns().AdjustToContents();

        ApplyTableBorder(worksheet.Range($"A1:F{row}"), XLBorderStyleValues.Thick);

        return Result.Success(GenerateExcelReport(workbook, "CompletedTasksReport.xlsx"));
    }

    public async Task<Result<FileContentResult>> GenerateTagReport(DateTime startDate, DateTime endDate, long userId, CancellationToken ct = default)
    {
        var userResult = await userRepository.GetByIdAsync(userId);
        var user = userResult.Data;

        var tagsResult = await tagRepository.GetAllAsync(t => t.UserId == userId);
        if (!tagsResult.IsSuccess || tagsResult.Data == null)
            return Result.Failure<FileContentResult>(ResultMessages.Task.NoCompletedTasks);

        var tags = tagsResult.Data;
        double globalTotalTime = 0;

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Tasks By Tag");

        worksheet.Cell(1, 1).Value = $"Tags time distribution for user: {user.UserName} from {startDate.ToShortDateString()} to {endDate.ToShortDateString()}";
        ApplyHeaderStyle(worksheet.Cell(1, 1));
        worksheet.Row(1).Height = 50;

        worksheet.Range("A1:C1").Merge();

        SetRow(worksheet.Row(2), "Tag Name", "Task Count", "Total Time (hours)");
        ApplyTableHeaderStyle(worksheet.Row(2));

        int row = 3;

        foreach (var tag in tags)
        {
            var tasksResult = await taskRepository.GetAllAsync(
                t => t.TaskTags.Any(tt => tt.TagId == tag.Id) && t.UserId == userId);

            if (!tasksResult.IsSuccess || tasksResult.Data == null)
                continue;

            var tasks = tasksResult.Data;
            double totalTagTime = 0;

            foreach (var task in tasks)
            {
                var timeIntervalsResult = await timeIntervalRepository.GetAllAsync(
                    ti => ti.TaskId == task.Id &&
                          ti.EndTime.HasValue &&
                          ti.EndTime.Value >= startDate.ToUniversalTime() &&
                          ti.EndTime.Value <= endDate.ToUniversalTime()
                );

                if (!timeIntervalsResult.IsSuccess)
                    return Result.Failure<FileContentResult>(ResultMessages.Timer.ErrorRetrieve + $"{task.Id}.");

                var taskTime = timeIntervalsResult.Data?.Sum(ti => (ti.EndTime.Value - ti.StartTime).TotalHours) ?? 0;
                totalTagTime += taskTime;
            }

            globalTotalTime += totalTagTime;

            SetRow(worksheet.Row(row), tag.Name, tasks.Count().ToString(), totalTagTime.ToString("0.00"));
            row++;
        }

        worksheet.Cell(row, 1).Value = "Total Time:";
        worksheet.Range(row, 1, row, 2).Merge();
        ApplyHeaderStyle(worksheet.Cell(row, 1));
        worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

        worksheet.Cell(row, 3).Value = globalTotalTime.ToString("0.00");
        ApplyHeaderStyle(worksheet.Cell(row, 3));

        ApplyTableBorder(worksheet.Range($"A1:C{row}"), XLBorderStyleValues.Thick);

        worksheet.Columns().AdjustToContents();

        return Result.Success(GenerateExcelReport(workbook, "TasksByTagReport.xlsx"));
    }

    public async Task<Result<FileContentResult>> GenerateDeadlineReport(long userId, DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        var userResult = await userRepository.GetByIdAsync(userId);
        var user = userResult.Data;

        var completedTasksResult = await taskRepository.GetAllAsync(
            t => t.UserId == userId
                 && t.CompletedAt.HasValue
                 && t.Deadline.HasValue
                 && t.CompletedAt >= startDate.ToUniversalTime()
                 && t.CompletedAt <= endDate.ToUniversalTime());

        if (!completedTasksResult.IsSuccess || completedTasksResult.Data == null)
            Result.Failure<FileContentResult>(ResultMessages.Task.NoCompletedTasksWithDeadline);

        var tasks = completedTasksResult.Data;

        var onTimeTasks = tasks.Where(task => task.CompletedAt <= task.Deadline).ToList();
        var lateTasks = tasks.Where(task => task.CompletedAt > task.Deadline).ToList();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Deadline Completion");

        worksheet.Cell(1, 1).Value = $"Deadline Completion Report for user: {user.UserName} from {startDate.ToShortDateString()} to {endDate.ToShortDateString()}";
        ApplyHeaderStyle(worksheet.Cell(1, 1));
        worksheet.Row(1).Height = 30;
        worksheet.Range("A1:D1").Merge();

        int totalTasks = tasks.Count();
        int onTimeCount = onTimeTasks.Count;
        double onTimePercentage = (totalTasks > 0) ? ((double)onTimeCount / totalTasks) * 100 : 0;

        worksheet.Cell(3, 1).Value = "Task count completed before the deadline";
        worksheet.Cell(3, 4).Value = $"{onTimeCount}/{totalTasks} ({onTimePercentage:F2}%)";
        ApplyTableBorder(worksheet.Range("A3:D3"), XLBorderStyleValues.Thin);

        ApplyTableHeaderStyle(worksheet.Row(3));

        worksheet.Range("A3:C3").Merge();

        int row = 5;

        worksheet.Cell(row, 1).Value = "Tasks completed before deadline";
        ApplyHeaderStyle(worksheet.Cell(row, 1));
        worksheet.Range(row, 1, row, 4).Merge();
        row++;

        SetRow(worksheet.Row(row), "Task", "Deadline", "CompletedAt", "Days Left");
        ApplyTableHeaderStyle(worksheet.Row(row));

        row++;

        foreach (var task in onTimeTasks.OrderBy(task => (task.Deadline.Value - task.CompletedAt.Value).Days))
        {
            int daysLeft = (task.Deadline.Value - task.CompletedAt.Value).Days;

            XLColor color = daysLeft switch
            {
                0 => XLColor.White,
                >= 1 and <= 2 => XLColor.LightGreen,
                >= 3 and <= 5 => XLColor.FromArgb(113, 240, 113),
                > 5 => XLColor.FromArgb(5, 252, 5),
                _ => XLColor.White
            };

            SetRow(worksheet.Row(row), task.Title,
                task.Deadline?.ToString("HH:mm yyyy-MM-dd"),
                task.CompletedAt?.ToString("HH:mm yyyy-MM-dd"),
                daysLeft.ToString());

            worksheet.Range(row, 1, row, 4).Style.Fill.SetBackgroundColor(color);
            ApplyCellBorders(worksheet.Range(row, 1, row, 4));
            row++;
        }

        ApplyTableBorder(worksheet.Range($"A5:D{row - 1}"), XLBorderStyleValues.Thin);

        row++;

        worksheet.Cell(row, 1).Value = "Tasks completed after the deadline";
        ApplyHeaderStyle(worksheet.Cell(row, 1));
        worksheet.Range(row, 1, row, 4).Merge();
        row++;

        SetRow(worksheet.Row(row), "Task", "Deadline", "CompletedAt", "Days Overdue");
        ApplyTableHeaderStyle(worksheet.Row(row));

        row++;

        foreach (var task in lateTasks.OrderBy(task => (task.CompletedAt.Value - task.Deadline.Value).Days))
        {
            int daysOverdue = (task.CompletedAt.Value - task.Deadline.Value).Days;

            XLColor color = daysOverdue switch
            {
                0 => XLColor.White,
                >= 1 and <= 2 => XLColor.FromArgb(255, 204, 204),
                >= 3 and <= 5 => XLColor.FromArgb(255, 102, 102),
                > 5 => XLColor.FromArgb(204, 0, 0),
                _ => XLColor.White
            };

            SetRow(worksheet.Row(row),
                 task.Title,
                 task.Deadline?.ToString("HH:mm yyyy-MM-dd"),
                 task.CompletedAt?.ToString("HH:mm yyyy-MM-dd"),
                 daysOverdue.ToString());

            worksheet.Range(row, 1, row, 4).Style.Fill.SetBackgroundColor(color);
            ApplyCellBorders(worksheet.Range(row, 1, row, 4));
            row++;
        }

        ApplyTableBorder(worksheet.Range($"A{8 + onTimeCount}:D{row - 1}"), XLBorderStyleValues.Thin);

        ApplyTableBorder(worksheet.Range($"A1:D{row - 1}"), XLBorderStyleValues.Thick);

        worksheet.Columns().AdjustToContents();
        worksheet.Column(1).Width = 8;

        return Result.Success(GenerateExcelReport(workbook, "DeadlineReport.xlsx"));
    }
}