using ERP.Purchasing.Application.Common.DTOs;
using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Application.Common.Mappers;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Application.PurchaseOrders.Queries.GetAllPurchaseOrders;
public class GetAllPurchaseOrdersQueryHandler
        : IRequestHandler<GetAllPurchaseOrdersQuery, List<PurchaseOrderDto>>
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly ILogger<GetAllPurchaseOrdersQueryHandler> _logger;

    public GetAllPurchaseOrdersQueryHandler(
        IPurchaseOrderRepository repository,
        ILogger<GetAllPurchaseOrdersQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<PurchaseOrderDto>> Handle(
        GetAllPurchaseOrdersQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting all purchase orders with filters");

            // Calculate skip/take from page number/size
            int? skip = null;
            int? take = null;

            if (request.PageNumber.HasValue && request.PageSize.HasValue)
            {
                skip = (request.PageNumber.Value - 1) * request.PageSize.Value;
                take = request.PageSize.Value;
            }

            // Direct call to repository with parameters
            var pos = await _repository.GetAllAsync(
                state: request.State,
                isActive: request.IsActive,
                fromDate: request.FromDate,
                toDate: request.ToDate,
                skip: skip,
                take: take);

            return pos.Select(PurchaseOrderMapper.ToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all purchase orders");
            throw;
        }
    }
}