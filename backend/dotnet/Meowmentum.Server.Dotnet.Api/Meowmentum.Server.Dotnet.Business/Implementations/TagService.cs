using AutoMapper;
using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Infrastructure.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Responses;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.Extensions.Logging;

namespace Meowmentum.Server.Dotnet.Business.Implementations;

public class TagService(
    IRepository<Tag> repository, 
    IMapper mapper,
    ILogger<TagService> logger) : ITagService
{
    public async Task<Result<bool>> CreateAsync(long userId, Tag tag, CancellationToken ct = default)
    {
        logger.LogInformation("Creating tag for user {UserId} with name {TagName}", userId, tag.Name);

        var isUnique = await repository.IsUnique(t =>
            t.UserId == userId &&
            t.Name.ToLower() == tag.Name.ToLower(),
            ct
        );

        if (!isUnique.IsSuccess || !isUnique.Data)
        {
            logger.LogError("Attempted to create a tag with name {TagName}, but a tag with that name already exists", tag.Name);
            return Result.Failure<bool>(ResultMessages.Tag.TagNameAlreadyExists);
        }

        tag.UserId = userId;
        var result = await repository.AddAsync(tag, ct);
        if (result.IsSuccess)
        {
            logger.LogInformation("Tag with name {TagName} successfully created for user {UserId}", tag.Name, userId);
        }
        else
        {
            logger.LogError("Error creating tag for user {UserId} with name {TagName}: {ErrorMessage}", userId, tag.Name, result.ErrorMessage);
        }
        return result;
    }

    public async Task<Result<bool>> DeleteAsync(long userId, long tagId, CancellationToken ct = default)
    {
        logger.LogInformation("Deleting tag with ID {TagId} for user {UserId}", tagId, userId);

        var result = await repository.GetFirstOrDefaultAsync(
            tag => tag.Id == tagId && tag.UserId == userId,
            ct
        );

        if (!result.IsSuccess)
        {
            logger.LogError("Tag with ID {TagId} not found for user {UserId}", tagId, userId);
            return Result.Failure<bool>(ResultMessages.Tag.TagNotFound);
        }

        var deleteResult = await repository.DeleteAsync(tagId, ct);
        if (deleteResult.IsSuccess)
        {
            logger.LogInformation("Tag with ID {TagId} successfully deleted for user {UserId}", tagId, userId);
        }
        else
        {
            logger.LogError("Error deleting tag with ID {TagId} for user {UserId}: {ErrorMessage}", tagId, userId, deleteResult.ErrorMessage);
        }

        return deleteResult;
    }

    public async Task<Result<IEnumerable<TagResponse>>> GetAllAsync(long userId, CancellationToken ct = default)
    {
        logger.LogInformation("Fetching all tags for user {UserId}", userId);

        var result = await repository.GetAllAsync(
            filter: tag => tag.UserId == userId,
            ct: ct
        );

        if (!result.IsSuccess)
        {
            logger.LogError("Error fetching tags for user {UserId}: {ErrorMessage}", userId, result.ErrorMessage);
            return Result.Failure<IEnumerable<TagResponse>>(ResultMessages.Tag.FetchTagsError);
        }

        var tagResponses = mapper.Map<IEnumerable<TagResponse>>(result.Data);
        logger.LogInformation("Successfully fetched {TagCount} tags for user {UserId}", tagResponses.Count(), userId);

        return Result.Success(tagResponses);
    }

    public async Task<Result<TagResponse>> GetByIdAsync(long userId, long tagId, CancellationToken ct = default)
    {
        logger.LogInformation("Fetching tag with ID {TagId} for user {UserId}", tagId, userId);

        var result = await repository.GetFirstOrDefaultAsync(
            tag => tag.Id == tagId && tag.UserId == userId,
            ct
        );

        if (!result.IsSuccess)
        {
            logger.LogError("Tag with ID {TagId} not found for user {UserId}", tagId, userId);
            return Result.Failure<TagResponse>(ResultMessages.Tag.TagNotFound);
        }

        var tagResponse = mapper.Map<TagResponse>(result.Data);
        logger.LogInformation("Successfully fetched tag with ID {TagId} for user {UserId}", tagId, userId);

        return Result.Success(tagResponse);
    }

    public async Task<Result<bool>> UpdateAsync(long userId, long tagId, Tag tag, CancellationToken ct = default)
    {
        logger.LogInformation("Updating tag with ID {TagId} for user {UserId}", tagId, userId);

        var isUnique = await repository.IsUnique(t => 
            t.UserId == userId &&
            t.Name.ToLower() == tag.Name.ToLower() && 
            t.Id != tagId,
            ct
        );

        if (!isUnique.IsSuccess || !isUnique.Data)
        {
            return Result.Failure<bool>(ResultMessages.Tag.TagNameAlreadyExists);
        }

        var result = await repository.GetFirstOrDefaultAsync(
            tag => tag.Id == tagId && tag.UserId == userId,
            ct
        );

        if (!result.IsSuccess)
        {
            logger.LogWarning("Attempted to update tag with name {TagName}, but a tag with that name already exists for user {UserId}", tag.Name, userId);
            return Result.Failure<bool>(ResultMessages.Tag.TagNotFound);
        }

        result.Data.Name = tag.Name;
        var updateResult = await repository.UpdateAsync(result.Data, ct);
        if (updateResult.IsSuccess)
        {
            logger.LogInformation("Tag with ID {TagId} successfully updated for user {UserId}", tagId, userId);
        }
        else
        {
            logger.LogError("Error updating tag with ID {TagId} for user {UserId}: {ErrorMessage}", tagId, userId, updateResult.ErrorMessage);
        }

        return updateResult;
    }
}
