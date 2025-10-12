using ERP.Purchasing.Domain.PurchaseOrderAggregate;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Enums;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.ValueObjects;

namespace ERP.Purchasing.Application.Common.Interfaces;
public interface IPurchaseOrderRepository: IGenericRepository<PurchaseOrder>
{
    Task<PurchaseOrder> GetByIdAsync(Guid id);
    Task<PurchaseOrder> GetByNumberAsync(PurchaseOrderNumber number);
    Task<IEnumerable<PurchaseOrder>> GetAllAsync(
        PurchaseOrderState? state = null,
        bool? isActive = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int? skip = null,
        int? take = null);

    Task<IEnumerable<PurchaseOrder>> GetRecentAsync(int count = 7);
    Task<IEnumerable<PurchaseOrder>> GetActiveAsync();
    Task<IEnumerable<PurchaseOrder>> GetByStateAsync(PurchaseOrderState state);

    Task AddAsync(PurchaseOrder purchaseOrder);
    Task AddRangeAsync(IEnumerable<PurchaseOrder> purchaseOrders);
    Task UpdateAsync(PurchaseOrder purchaseOrder);
    Task DeleteAsync(PurchaseOrder purchaseOrder);
    Task<int> SaveChangesAsync();
}