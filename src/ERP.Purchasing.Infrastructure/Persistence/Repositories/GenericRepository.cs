using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly PurchasingDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ILogger<GenericRepository<T>> _logger;

    public GenericRepository(PurchasingDbContext context, ILogger<GenericRepository<T>> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbSet = _context.Set<T>();
    }

    public virtual async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting entity {EntityType} with Id: {Id}", typeof(T).Name, id);
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all entities of type {EntityType}", typeof(T).Name);
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Finding entities of type {EntityType} with predicate", typeof(T).Name);
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting first entity of type {EntityType} with predicate", typeof(T).Name);
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Adding entity of type {EntityType}", typeof(T).Name);
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Adding {Count} entities of type {EntityType}", entities.Count(), typeof(T).Name);
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public virtual void Update(T entity)
    {
        _logger.LogDebug("Updating entity of type {EntityType}", typeof(T).Name);
        _dbSet.Update(entity);
    }

    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        _logger.LogDebug("Updating {Count} entities of type {EntityType}", entities.Count(), typeof(T).Name);
        _dbSet.UpdateRange(entities);
    }

    public virtual void Remove(T entity)
    {
        _logger.LogDebug("Removing entity of type {EntityType}", typeof(T).Name);
        _dbSet.Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        _logger.LogDebug("Removing {Count} entities of type {EntityType}", entities.Count(), typeof(T).Name);
        _dbSet.RemoveRange(entities);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            return await _dbSet.CountAsync(cancellationToken);

        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual IQueryable<T> Query()
    {
        return _dbSet.AsQueryable();
    }
}
