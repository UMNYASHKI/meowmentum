using Meowmentum.Server.Dotnet.Shared.Requests.Tag;
using Meowmentum.Server.Dotnet.Shared.Responses;
using Meowmentum.Server.Dotnet.Shared.Results;

namespace Meowmentum.Server.Dotnet.Business.Abstractions;

public interface ITagService
{
    Task<Result<TagResponse>> GetByIdAsync(long id);
    Task<Result<IEnumerable<TagResponse>>> GetAllAsync();
    Task<Result<bool>> CreateAsync(CreateTagRequest request);
    Task<Result<bool>> UpdateAsync(long id, UpdateTagRequest request);
    Task<Result<bool>> DeleteAsync(long id);
}
