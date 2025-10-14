using ERP.Purchasing.Application.Common.Models;
using ERP.Purchasing.Domain.PurchaseOrderAggregate;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Enums;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.ValueObjects;

namespace ERP.Purchasing.Application.Common.Interfaces;
public interface IPurchaseOrderRepository
{
    Task<PurchaseOrder> GetByIdAsync(Guid id);
    Task<PurchaseOrder> GetByNumberAsync(PurchaseOrderNumber number);
    Task<QueryResult<PurchaseOrder>> GetAllAsync(PurchaseOrderQueryParams queryParams);
    Task<IEnumerable<PurchaseOrder>> GetRecentAsync(int count = 7);
    Task<IEnumerable<PurchaseOrder>> GetActiveAsync();
    Task<IEnumerable<PurchaseOrder>> GetByStateAsync(PurchaseOrderState state);

    Task AddAsync(PurchaseOrder purchaseOrder);
    Task AddRangeAsync(IEnumerable<PurchaseOrder> purchaseOrders);
    Task UpdateAsync(PurchaseOrder purchaseOrder);
    Task DeleteAsync(PurchaseOrder purchaseOrder);
    Task<int> SaveChangesAsync();
}
