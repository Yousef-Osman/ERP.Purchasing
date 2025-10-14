using System.Linq;
using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Application.Common.Models;
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

    public async Task<QueryResult<PurchaseOrder>> GetAllAsync(PurchaseOrderQueryParams queryParams)
    {
        IQueryable<PurchaseOrderEntity> query = _dbSet.AsNoTracking();

        // Apply filters
        if (queryParams.State.HasValue)
            query = query.Where(po => po.State == (int)queryParams.State.Value);

        if (queryParams.IsActive.HasValue)
            query = query.Where(po => po.IsActive == queryParams.IsActive.Value);

        if (queryParams.FromDate.HasValue)
            query = query.Where(po => po.IssueDate >= queryParams.FromDate.Value);

        if (queryParams.ToDate.HasValue)
            query = query.Where(po => po.IssueDate <= queryParams.ToDate.Value);

        if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
        {
            var searchTerm = queryParams.SearchTerm.ToLower();
            query = query.Where(po =>
                po.Number.ToLower().Contains(searchTerm) ||
                po.Items.Any(i => i.GoodCode.ToLower().Contains(searchTerm)));
        }

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = queryParams.SortBy.ToLower() switch
        {
            "number" => queryParams.SortDescending
                ? query.OrderByDescending(po => po.Number)
                : query.OrderBy(po => po.Number),
            "totalprice" => queryParams.SortDescending
                ? query.OrderByDescending(po => po.TotalPrice)
                : query.OrderBy(po => po.TotalPrice),
            "state" => queryParams.SortDescending
                ? query.OrderByDescending(po => po.State)
                : query.OrderBy(po => po.State),
            _ => queryParams.SortDescending
                ? query.OrderByDescending(po => po.IssueDate)
                : query.OrderBy(po => po.IssueDate)
        };

        // Apply pagination
        var entities = await query
            .Skip(queryParams.Skip)
            .Take(queryParams.Take)
            .Include(po => po.Items)
            .ToListAsync();

        var items = entities.Select(PurchaseOrderMapper.ToDomain);

        return new QueryResult<PurchaseOrder>(items, totalCount);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetRecentAsync(int count = 7)
    {
        var entities = await _dbSet
            .Include(po => po.Items)
            .AsNoTracking()
            .OrderByDescending(po => po.IssueDate)
            .Take(count)
            .ToListAsync();
        return entities.Select(PurchaseOrderMapper.ToDomain);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetActiveAsync()
    {
        var entities = await _dbSet
            .Include(po => po.Items)
            .AsNoTracking()
            .Where(po => po.IsActive)
            .OrderByDescending(po => po.IssueDate)
            .ToListAsync();
        return entities.Select(PurchaseOrderMapper.ToDomain);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetByStateAsync(PurchaseOrderState state)
    {
        var entities = await _dbSet
            .Include(po => po.Items)
            .AsNoTracking()
            .Where(po => po.State == (int)state)
            .OrderByDescending(po => po.IssueDate)
            .ToListAsync();
        return entities.Select(PurchaseOrderMapper.ToDomain);
    }

    public async Task AddAsync(PurchaseOrder purchaseOrder)
    {
        var entity = PurchaseOrderMapper.ToEntity(purchaseOrder);
        await base.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<PurchaseOrder> purchaseOrders)
    {
        var entities = purchaseOrders.Select(PurchaseOrderMapper.ToEntity);
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
            base.Remove(entity);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
