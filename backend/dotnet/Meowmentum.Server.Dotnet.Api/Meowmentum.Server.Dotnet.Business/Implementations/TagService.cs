using AutoMapper;
using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Infrastructure.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Responses;
using Meowmentum.Server.Dotnet.Shared.Results;

namespace Meowmentum.Server.Dotnet.Business.Implementations;

public class TagService(
    IRepository<Tag> repository, 
    IMapper mapper) : ITagService
{
    public async Task<Result<bool>> CreateAsync(long userId, Tag tag, CancellationToken ct = default)
    {
        var isUnique = await repository.IsUnique(t =>
            t.UserId == userId &&
            t.Name.ToLower() == tag.Name.ToLower(),
            ct
        );

        if (!isUnique.IsSuccess || !isUnique.Data)
        {
            return Result.Failure<bool>(ResultMessages.Tag.TagNameAlreadyExists);
        }

        tag.UserId = userId;
        return await repository.AddAsync(tag, ct);
    }

    public async Task<Result<bool>> DeleteAsync(long userId, long tagId, CancellationToken ct = default)
    {
        var result = await repository.GetFirstOrDefaultAsync(
            tag => tag.Id == tagId && tag.UserId == userId,
            ct
        );

        if (!result.IsSuccess)
        {
            return Result.Failure<bool>(ResultMessages.Tag.TagNotFound);
        }

        return await repository.DeleteAsync(tagId, ct);
    }

    public async Task<Result<IEnumerable<TagResponse>>> GetAllAsync(long userId, CancellationToken ct = default)
    {
        var result = await repository.GetAllAsync(
            filter: tag => tag.UserId == userId,
            ct: ct
        );

        if (!result.IsSuccess)
        {
            return Result.Failure<IEnumerable<TagResponse>>(ResultMessages.Tag.FetchTagsError);
        }

        var tagResponses = mapper.Map<IEnumerable<TagResponse>>(result.Data);

        return Result.Success(tagResponses);
    }

    public async Task<Result<TagResponse>> GetByIdAsync(long userId, long tagId, CancellationToken ct = default)
    {
        var result = await repository.GetFirstOrDefaultAsync(
            tag => tag.Id == tagId && tag.UserId == userId,
            ct
        );

        if (!result.IsSuccess)
        {
            return Result.Failure<TagResponse>(ResultMessages.Tag.TagNotFound);
        }

        var tagResponse = mapper.Map<TagResponse>(result.Data);

        return Result.Success(tagResponse);
    }

    public async Task<Result<bool>> UpdateAsync(long userId, long tagId, Tag tag, CancellationToken ct = default)
    {
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
            return Result.Failure<bool>(ResultMessages.Tag.TagNotFound);
        }

        return await repository.UpdateAsync(result.Data, ct);
    }
}
