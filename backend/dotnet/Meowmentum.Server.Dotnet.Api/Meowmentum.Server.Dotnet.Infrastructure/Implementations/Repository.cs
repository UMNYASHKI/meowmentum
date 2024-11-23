using Meowmentum.Server.Dotnet.Infrastructure.Abstractions;
using Meowmentum.Server.Dotnet.Persistence;
using Meowmentum.Server.Dotnet.Shared.Results;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Infrastructure.Implementations;

public class Repository<TEntity>(ApplicationDbContext _context) 
    : IRepository<TEntity> where TEntity : class
{
    public async Task<Result<bool>> AddAsync(TEntity entity, CancellationToken ct = default)
    {
        try
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Error adding {typeof(TEntity).Name}: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteAsync(long id, CancellationToken ct = default)
    {
        try
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);

            if (entity == null)
            {
                return Result.Failure<bool>($"{typeof(TEntity).Name} not found");
            }

            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Error deleting {typeof(TEntity).Name}: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<TEntity>>> GetAllAsync(
        Expression<Func<TEntity, bool>>? filter = null, 
        Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> orderBy = null, 
        int? pageNum = null, int? count = null,
        CancellationToken ct = default)
    {
        try
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (pageNum != null && count != null)
            {
                query = query.Skip((int)((pageNum - 1) * count)).Take((int)count);
            }
            else if (count != null)
            {
                query = query.Take((int)count);
            }

            if (orderBy != null)
            {
                return Result.Success<IEnumerable<TEntity>>(await orderBy.Compile()(query).ToListAsync());
            }
            else
            {
                return Result.Success<IEnumerable<TEntity>>(await query.ToListAsync());
            }
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<TEntity>>($"Error fetching {ex.Message}s");
        }
    }

    public async Task<Result<TEntity>> GetByIdAsync(long id, CancellationToken ct = default)
    {
        try
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);

            if (entity == null)
            {
                return Result.Failure<TEntity>($"{typeof(TEntity).Name} not found");
            }

            return Result.Success(entity);
        }
        catch (Exception ex)
        {
            return Result.Failure<TEntity>($"Error fetching {typeof(TEntity).Name}: {ex.Message}");
        }
    }

    public async Task<Result<TEntity>> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct = default)
    {
        try
        {
            var entity = await _context.Set<TEntity>().FirstOrDefaultAsync(filter, ct);

            if (entity == null)
            {
                return Result.Failure<TEntity>($"{typeof(TEntity).Name} not found");
            }

            return Result.Success(entity);
        }
        catch (Exception ex)
        {
            return Result.Failure<TEntity>($"Error fetching {typeof(TEntity).Name}: {ex.Message}");
        }
    }

    public async Task<Result<bool>> IsUnique(Expression<Func<TEntity, bool>> filter)
    {
        try
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().AsQueryable();
            var exists = await query.AnyAsync(filter);
            Console.WriteLine($"Checking uniqueness: {filter}, Exists: {exists}");
            return Result.Success(!exists);
            //return Result.Success(!query.Any(filter));
        }
        catch
        {
            return Result.Failure<bool>($"Cannot check uniqueness of {typeof(TEntity).Name}");
        }
    }

    public async Task<Result<bool>> UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        try
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Error updating {typeof(TEntity).Name}: {ex.Message}");
        }
    }
}
