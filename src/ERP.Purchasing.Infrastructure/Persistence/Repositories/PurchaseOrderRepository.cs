using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Domain.PurchaseOrderAggregate;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Enums;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.ValueObjects;
using ERP.Purchasing.Infrastructure.Mappers;
using ERP.Purchasing.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Infrastructure.Persistence.Repositories;
public class PurchaseOrderRepository : GenericRepository<PurchaseOrderEntity>, IPurchaseOrderRepository
{
    public PurchaseOrderRepository(PurchasingDbContext context, ILogger<GenericRepository<PurchaseOrderEntity>> logger)
        : base(context, logger)
    {
    }

    public async Task<PurchaseOrder> GetByIdAsync(Guid id)
    {
        var entity = await _dbSet
            .Include(po => po.Items)
            .FirstOrDefaultAsync(po => po.Id == id);

        return PurchaseOrderMapper.ToDomain(entity);
    }

    public async Task<PurchaseOrder> GetByNumberAsync(PurchaseOrderNumber number)
    {
        var entity = await _dbSet
            .Include(po => po.Items)
            .FirstOrDefaultAsync(po => po.Number == number.Value);

        return PurchaseOrderMapper.ToDomain(entity);
    }

    // SIMPLIFIED: Direct query methods instead of specifications
    public async Task<IEnumerable<PurchaseOrder>> GetAllAsync(
        PurchaseOrderState? state = null,
        bool? isActive = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int? skip = null,
        int? take = null)
    {
        IQueryable<PurchaseOrderEntity> query = _dbSet
            .Include(po => po.Items)
            .AsNoTracking();

        // Apply filters
        if (state.HasValue)
            query = query.Where(po => po.State == (int)state.Value);

        if (isActive.HasValue)
            query = query.Where(po => po.IsActive == isActive.Value);

        if (fromDate.HasValue)
            query = query.Where(po => po.IssueDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(po => po.IssueDate <= toDate.Value);

        // Order by date descending
        query = query.OrderByDescending(po => po.IssueDate);

        // Apply pagination
        if (skip.HasValue)
            query = query.Skip(skip.Value);

        if (take.HasValue)
            query = query.Take(take.Value);

        var entities = await query.ToListAsync();
        return entities.Select(PurchaseOrderMapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<PurchaseOrder>> GetRecentAsync(int count = 7)
    {
        var entities = await _dbSet
            .Include(po => po.Items)
            .AsNoTracking()
            .OrderByDescending(po => po.IssueDate)
            .Take(count)
            .ToListAsync();

        return entities.Select(PurchaseOrderMapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<PurchaseOrder>> GetActiveAsync()
    {
        var entities = await _dbSet
            .Include(po => po.Items)
            .AsNoTracking()
            .Where(po => po.IsActive)
            .OrderByDescending(po => po.IssueDate)
            .ToListAsync();

        return entities.Select(PurchaseOrderMapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<PurchaseOrder>> GetByStateAsync(PurchaseOrderState state)
    {
        var entities = await _dbSet
            .Include(po => po.Items)
            .AsNoTracking()
            .Where(po => po.State == (int)state)
            .OrderByDescending(po => po.IssueDate)
            .ToListAsync();

        return entities.Select(PurchaseOrderMapper.ToDomain).ToList();
    }

    public async Task AddAsync(PurchaseOrder purchaseOrder)
    {
        var entity = PurchaseOrderMapper.ToEntity(purchaseOrder);
        await base.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<PurchaseOrder> purchaseOrders)
    {
        var entities = purchaseOrders.Select(PurchaseOrderMapper.ToEntity).ToList();
        await base.AddRangeAsync(entities);
    }

    public async Task UpdateAsync(PurchaseOrder purchaseOrder)
    {
        var entity = await _dbSet
            .Include(po => po.Items)
            .FirstOrDefaultAsync(po => po.Id == purchaseOrder.Id);

        if (entity == null)
            throw new InvalidOperationException($"PurchaseOrder {purchaseOrder.Id} not found");

        PurchaseOrderMapper.UpdateEntity(entity, purchaseOrder);
    }

    public async Task DeleteAsync(PurchaseOrder purchaseOrder)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(po => po.Id == purchaseOrder.Id);
        if (entity != null)
        {
            base.Remove(entity);
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
