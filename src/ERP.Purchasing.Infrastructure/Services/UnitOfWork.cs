using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Infrastructure.Services;
public class UnitOfWork : IUnitOfWork
{
    private readonly PurchasingDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private bool _disposed;

    public UnitOfWork(PurchasingDbContext context, ILogger<UnitOfWork> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Saving changes to database");
            var result = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully saved {Count} changes", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes to database");
            throw;
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Beginning transaction");
        await _context.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Committing transaction");
            await _context.CommitTransactionAsync(cancellationToken);
            _logger.LogInformation("Transaction committed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error committing transaction");
            throw;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Rolling back transaction");
        await _context.RollbackTransactionAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
        }
        _disposed = true;
    }
}
