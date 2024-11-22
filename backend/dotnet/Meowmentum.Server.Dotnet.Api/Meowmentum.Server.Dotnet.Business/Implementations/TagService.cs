using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Infrastructure.Abstractions;
using Meowmentum.Server.Dotnet.Shared.Requests.Tag;
using Meowmentum.Server.Dotnet.Shared.Responses;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Meowmentum.Server.Dotnet.Business.Implementations;

public class TagService(IRepository<Tag> repository) : ITagService
{
    public async Task<Result<bool>> CreateAsync(CreateTagRequest request)
    {
        var tag = new Tag
        {
            Name = request.Name,
            CreatedDate = DateTime.UtcNow
        };

        return await repository.AddAsync(tag);
    }

    public async Task<Result<bool>> DeleteAsync(long id)
    {
        return await repository.DeleteAsync(id);
    }

    public async Task<Result<IEnumerable<TagResponse>>> GetAllAsync()
    {
        var result = await repository.GetAllAsync();
        if (!result.IsSuccess)
        {
            return Result.Failure<IEnumerable<TagResponse>>(ResultMessages.Tag.FetchTagsError);
        }

        var tagResponses = result.Data.Select(t => new TagResponse
        {
            Id = t.Id,
            Name = t.Name,
            CreatedDate = t.CreatedDate,
            UpdatedDate = t.UpdatedDate
        }).ToList();

        return Result.Success<IEnumerable<TagResponse>>(tagResponses);
    }

    public async Task<Result<TagResponse>> GetByIdAsync(long id)
    {
        var result = await repository.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            return Result.Failure<TagResponse>(ResultMessages.Tag.TagNotFound);
        }

        var tagResponse = new TagResponse
        {
            Id = result.Data.Id,
            Name = result.Data.Name,
            CreatedDate = result.Data.CreatedDate,
            UpdatedDate = result.Data.UpdatedDate
        };

        return Result.Success(tagResponse);
    }

    public async Task<Result<bool>> UpdateAsync(long id, UpdateTagRequest request)
    {
        var result = await repository.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            return Result.Failure<bool>(ResultMessages.Tag.TagNotFound);
        }

        var tag = result.Data;
        tag.Name = request.Name;
        tag.UpdatedDate = DateTime.UtcNow;

        return await repository.UpdateAsync(tag);
    }
}
