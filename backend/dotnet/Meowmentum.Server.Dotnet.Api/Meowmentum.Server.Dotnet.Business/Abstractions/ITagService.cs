using Meowmentum.Server.Dotnet.Core.Entities;
using Meowmentum.Server.Dotnet.Shared.Responses;
using Meowmentum.Server.Dotnet.Shared.Results;

namespace Meowmentum.Server.Dotnet.Business.Abstractions;

public interface ITagService
{
    Task<Result<TagResponse>> GetByIdAsync(long userId, long tagId, CancellationToken ct = default);
    Task<Result<IEnumerable<TagResponse>>> GetAllAsync(long userId, CancellationToken ct = default);
    Task<Result<bool>> CreateAsync(long userId, Tag tag, CancellationToken ct = default);
    Task<Result<bool>> UpdateAsync(long userId, long tagId, Tag tag, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(long userId, long tagId, CancellationToken ct = default);
}
