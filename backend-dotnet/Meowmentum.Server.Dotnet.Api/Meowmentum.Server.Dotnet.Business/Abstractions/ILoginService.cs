using Meowmentum.Server.Dotnet.Shared.Requests;
using Meowmentum.Server.Dotnet.Shared.Results;
using System.Threading;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Business.Abstractions
{
    public interface ILoginService
    {
        Task<Result<string>> LoginAsync(LoginRequest request, CancellationToken token = default);
    }
}
