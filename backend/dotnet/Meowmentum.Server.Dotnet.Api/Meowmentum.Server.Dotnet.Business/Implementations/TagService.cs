using Meowmentum.Server.Dotnet.Business.Abstractions;
using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Persistence;
using Meowmentum.Server.Dotnet.Shared.Requests.Tag;
using Meowmentum.Server.Dotnet.Shared.Responses;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Business.Implementations;

public class TagService(ApplicationDbContext context) : ITagService
{
    public async Task<Result<bool>> CreateAsync(CreateTagRequest request)
    {
        try
        {
            var tag = new Tag
            {
                Name = request.Name,
                CreatedDate = DateTime.UtcNow
            };

            await context.Tags.AddAsync(tag);
            await context.SaveChangesAsync();
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"{ResultMessages.Tag.UnexpectedError} {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteAsync(long id)
    {
        try
        {
            var tag = await context.Tags.FindAsync(id);
            if (tag == null)
            {
                return Result.Failure<bool>(ResultMessages.Tag.TagNotFound);
            }

            context.Tags.Remove(tag);
            await context.SaveChangesAsync();
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"{ResultMessages.Tag.UnexpectedError} {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<TagResponse>>> GetAllAsync()
    {
        var tags = await context.Tags.ToListAsync();
        var tagResponses = tags.Select(t => new TagResponse
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
        var tag = await context.Tags.FindAsync(id);
        if (tag == null)
        {
            return Result.Failure<TagResponse>(ResultMessages.Tag.TagNotFound);
        }

        var tagResponse = new TagResponse
        {
            Id = tag.Id,
            Name = tag.Name,
            CreatedDate = tag.CreatedDate,
            UpdatedDate = tag.UpdatedDate
        };

        return Result.Success(tagResponse);
    }

    public async Task<Result<bool>> UpdateAsync(long id, UpdateTagRequest request)
    {
        try
        {
            var tag = await context.Tags.FindAsync(id);
            if (tag == null)
            {
                return Result.Failure<bool>(ResultMessages.Tag.TagNotFound);
            }

            tag.Name = request.Name;
            tag.UpdatedDate = DateTime.UtcNow;

            context.Tags.Update(tag);
            await context.SaveChangesAsync();
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"{ResultMessages.Tag.UnexpectedError} {ex.Message}");
        }
    }
}
