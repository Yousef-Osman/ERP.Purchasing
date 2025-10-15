using ERP.Purchasing.Application.Common.DTOs;
using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Application.Common.Mappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Application.PurchaseOrders.Queries.GetPurchaseOrderById;
public class GetPurchaseOrderByIdQueryHandler
        : IRequestHandler<GetPurchaseOrderByIdQuery, PurchaseOrderDto>
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly ILogger<GetPurchaseOrderByIdQueryHandler> _logger;

    public GetPurchaseOrderByIdQueryHandler(
        IPurchaseOrderRepository repository,
        ILogger<GetPurchaseOrderByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PurchaseOrderDto> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var purchaseOrder = await _repository.GetByIdAsync(request.Id);
            if (purchaseOrder == null)
            {
                _logger.LogWarning("Purchase order not found: {Id}", request.Id);
                return null;
            }

            var dto = PurchaseOrderMapper.ToDto(purchaseOrder);

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting purchase order by Id: {Id}", request.Id);
            throw;
        }
    }
}
