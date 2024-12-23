﻿using AutoMapper;
using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Infrastructure.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Requests.Task;
using Meowmentum.Server.Dotnet.Shared.Responses.Task;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.Extensions.Logging;
using Task = Meowmentum.Server.Dotnet.Core.Entities.Task;

namespace Meowmentum.Server.Dotnet.Business.Implementations;

public class TaskService(
    IRepository<Task> taskRepository,
    IRepository<Tag> tagRepository,
    IMapper mapper,
    ILogger<TaskService> logger) : ITaskService
{
    public async Task<Result<bool>> CreateTaskAsync(long userId, Task task, CancellationToken ct = default)
    {
        task.UserId = userId;

        logger.LogInformation("Attempting to create task for user {UserId}", userId);

        var tagValidationResult = await ValidateTaskTagsAsync(userId, task, ct);
        if (!tagValidationResult.IsSuccess)
        {
            return tagValidationResult;
        }

        var addResult = await taskRepository.AddAsync(task, ct);

        if (addResult.IsSuccess)
        {
            logger.LogInformation("Successfully created task with ID {TaskId} for user {UserId}", task.Id, userId);
        }
        else
        {
            logger.LogError("Failed to create task for user {UserId}: {ErrorMessage}", userId, addResult.ErrorMessage);
        }

        return addResult;
    }

    public async Task<Result<bool>> UpdateTaskAsync(long userId, Task task, CancellationToken ct = default)
    {
        logger.LogInformation("Attempting to update task with ID {TaskId} for user {UserId}", task.Id, userId);

        var result = await taskRepository.GetFirstOrDefaultAsync(
            t => t.Id == task.Id && t.UserId == userId, ct);

        if (!result.IsSuccess)
        {
            logger.LogError("Task with ID {TaskId} not found for user {UserId}", task.Id, userId);
            return Result.Failure<bool>(ResultMessages.Task.TaskNotFound);
        }

        var tagValidationResult = await ValidateTaskTagsAsync(userId, task, ct);
        if (!tagValidationResult.IsSuccess)
        {
            return tagValidationResult;
        }

        mapper.Map(task, result.Data);

        var updateResult = await taskRepository.UpdateAsync(result.Data, ct);

        if (updateResult.IsSuccess)
        {
            logger.LogInformation("Successfully updated task with ID {TaskId} for user {UserId}", task.Id, userId);
        }
        else
        {
            logger.LogError("Failed to update task with ID {TaskId} for user {UserId}: {ErrorMessage}", task.Id, userId, updateResult.ErrorMessage);
        }

        return updateResult;
    }

    private async Task<Result<bool>> ValidateTaskTagsAsync(long userId, Task task, CancellationToken ct = default)
    {
        if (task.TaskTags?.Any() ?? false)
        {
            var tagIds = task.TaskTags.Select(tt => tt.TagId).ToList();

            var tagsResult = await tagRepository.GetAllAsync(
                t => tagIds.Contains(t.Id) && t.UserId == userId,
                ct: ct
            );

            if (!tagsResult.IsSuccess || tagsResult.Data.Count() != task.TaskTags.Count)
            {
                return Result.Failure<bool>(ResultMessages.Task.InvalidTag);
            }
        }

        return Result.Success(true);
    }

    public async Task<Result<bool>> UpsertTaskAsync(long userId, Task task, long? taskId, CancellationToken ct = default)
    {
        logger.LogInformation("Attempting to upsert task for user {UserId}", userId);

        if (taskId.HasValue)
        {
            task.Id = taskId.Value;
            return await UpdateTaskAsync(userId, task, ct);
        }
        else
        {
            return await CreateTaskAsync(userId, task, ct);
        }
    }

    public async Task<Result<bool>> DeleteTaskAsync(long userId, long taskId, CancellationToken ct = default)
    {
        logger.LogInformation("Attempting to delete task with ID {TaskId} for user {UserId}", taskId, userId);

        var taskResult = await taskRepository.GetFirstOrDefaultAsync(
            t => t.Id == taskId && t.UserId == userId, ct);

        if (!taskResult.IsSuccess)
        {
            logger.LogError("Task with ID {TaskId} not found for user {UserId}", taskId, userId);
            return Result.Failure<bool>(ResultMessages.Task.TaskNotFound);
        }

        var deleteResult = await taskRepository.DeleteAsync(taskId, ct);

        if (deleteResult.IsSuccess)
        {
            logger.LogInformation("Successfully deleted task with ID {TaskId} for user {UserId}", taskId, userId);
        }
        else
        {
            logger.LogError("Failed to delete task with ID {TaskId} for user {UserId}: {ErrorMessage}", taskId, userId, deleteResult.ErrorMessage);
        }

        return deleteResult;
    }

    public async Task<Result<IEnumerable<TaskResponse>>> GetTasksAsync(long userId, TaskFilterRequest filterRequest, CancellationToken ct = default)
    {
        if (filterRequest.TaskId.HasValue)
        {
            var taskResult = await taskRepository.GetByIdAsync(filterRequest.TaskId.Value, ct);

            if (!taskResult.IsSuccess)
            {
                logger.LogError("Task with ID {TaskId} not found for user {UserId}", filterRequest.TaskId.Value, userId);
                return Result.Failure<IEnumerable<TaskResponse>>(ResultMessages.Task.TaskNotFound);
            }

            var task = taskResult.Data;

            if (task.UserId != userId)
            {
                logger.LogWarning("Unauthorized access attempt by user {UserId} to task {TaskId}", userId, filterRequest.TaskId.Value);
                return Result.Failure<IEnumerable<TaskResponse>>(ResultMessages.Task.UnauthorizedAccess);
            }

            logger.LogInformation("Successfully fetched task with ID {TaskId} for user {UserId}", filterRequest.TaskId, userId);
            return Result.Success(new List<TaskResponse> { mapper.Map<TaskResponse>(task) }.AsEnumerable());
        }

        var queryResult = await taskRepository.GetAllAsync(
            t => t.UserId == userId,
            orderBy: t => t.OrderBy(task => task.CreatedAt),
            ct: ct);

        if (!queryResult.IsSuccess || queryResult.Data == null || !queryResult.Data.Any())
        {
            logger.LogInformation("No tasks found for user {UserId} with given filters", userId);
            return Result.Failure<IEnumerable<TaskResponse>>(ResultMessages.Task.NoFilteredTasks);
        }

        var tasks = queryResult.Data.AsQueryable();

        if (filterRequest.Status.Any())
        {
            tasks = tasks.Where(t => filterRequest.Status.Contains(t.Status));
        }
        if (filterRequest.Priorities.Any())
        {
            tasks = tasks.Where(t => filterRequest.Priorities.Contains(t.Priority));
        }
        if (filterRequest.TagIds.Any())
        {
            tasks = tasks.Where(t => t.TaskTags != null && t.TaskTags.Any(tt => filterRequest.TagIds.Contains(tt.TagId)));
        }

        var taskResponses = mapper.Map<IEnumerable<TaskResponse>>(tasks);

        logger.LogInformation("Successfully retrieved {TaskCount} tasks for user {UserId}", tasks.Count(), userId);

        return Result.Success(taskResponses);
    }
}
