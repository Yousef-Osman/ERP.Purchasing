using ERP.Purchasing.Application.Common.DTOs;
using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Application.Common.Mappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Application.PurchaseOrders.Queries.GetRecentPurchaseOrders;
public class GetRecentPurchaseOrdersQueryHandler
        : IRequestHandler<GetRecentPurchaseOrdersQuery, List<PurchaseOrderDto>>
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly ILogger<GetRecentPurchaseOrdersQueryHandler> _logger;

    public GetRecentPurchaseOrdersQueryHandler(
        IPurchaseOrderRepository repository,
        ILogger<GetRecentPurchaseOrdersQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<PurchaseOrderDto>> Handle(
        GetRecentPurchaseOrdersQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting recent {Count} purchase orders", request.Count);

            var pos = await _repository.GetRecentAsync(request.Count);
            var dtos = pos.Select(PurchaseOrderMapper.ToDto).ToList();

            return dtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent purchase orders");
            throw;
        }
    }
}
