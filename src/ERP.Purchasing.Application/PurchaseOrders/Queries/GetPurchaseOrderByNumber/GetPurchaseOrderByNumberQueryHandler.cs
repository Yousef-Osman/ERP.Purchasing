using ERP.Purchasing.Application.Common.DTOs;
using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Application.Common.Mappers;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Application.PurchaseOrders.Queries.GetPurchaseOrderByNumber;
public class GetPurchaseOrderByNumberQueryHandler
        : IRequestHandler<GetPurchaseOrderByNumberQuery, PurchaseOrderDto>
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly ILogger<GetPurchaseOrderByNumberQueryHandler> _logger;

    public GetPurchaseOrderByNumberQueryHandler(
        IPurchaseOrderRepository repository,
        ILogger<GetPurchaseOrderByNumberQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PurchaseOrderDto> Handle(GetPurchaseOrderByNumberQuery request,CancellationToken cancellationToken)
    {
        try
        {
            var poNumber = new PurchaseOrderNumber(request.Number);
            var purchaseOrder = await _repository.GetByNumberAsync(poNumber);

            if (purchaseOrder == null)
            {
                _logger.LogWarning("Purchase order not found: {Number}", request.Number);
                return null;
            }

            var dto = PurchaseOrderMapper.ToDto(purchaseOrder);

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting purchase order by Number: {Number}", request.Number);
            throw;
        }
    }
}
