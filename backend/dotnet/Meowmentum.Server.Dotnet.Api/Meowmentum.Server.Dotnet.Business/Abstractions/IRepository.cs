using Meowmentum.Server.Dotnet.Shared.Results;
using System.Linq.Expressions;

namespace Meowmentum.Server.Dotnet.Infrastructure.Abstractions;

public interface IRepository<TEntity> where TEntity : class
{
    Task<Result<bool>> AddAsync(TEntity entity);

    Task<Result<TEntity>> GetByIdAsync(long id);

    Task<Result<IEnumerable<TEntity>>> GetAllAsync(
         Expression<Func<TEntity, bool>>? filter = null,
          Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> orderBy = null,
          int? pageNum = null, int? count = null);

    Task<Result<bool>> DeleteAsync(long id);

    Task<Result<bool>> UpdateAsync(TEntity entity);
}
