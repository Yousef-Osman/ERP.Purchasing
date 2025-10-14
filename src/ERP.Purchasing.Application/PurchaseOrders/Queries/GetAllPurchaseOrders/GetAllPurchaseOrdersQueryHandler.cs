using ERP.Purchasing.Application.Common.DTOs;
using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Application.Common.Mappers;
using ERP.Purchasing.Application.Common.Models;
using ERP.SharedKernel.Enums;
using ERP.SharedKernel.Pagination;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Application.PurchaseOrders.Queries.GetAllPurchaseOrders;
public class GetAllPurchaseOrdersQueryHandler
        : IRequestHandler<GetAllPurchaseOrdersQuery, PagedResult<PurchaseOrderDto>>
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

    public async Task<PagedResult<PurchaseOrderDto>> Handle(
        GetAllPurchaseOrdersQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var req = request.Request;

            _logger.LogInformation("Getting purchase orders - Page: {PageNumber}, Size: {PageSize}", req.PageNumber, req.PageSize);

            // Map to domain query params
            var queryParams = new PurchaseOrderQueryParams
            {
                State = req.State,
                IsActive = req.IsActive,
                FromDate = req.FromDate,
                ToDate = req.ToDate,
                SearchTerm = req.SearchTerm,
                SortBy = req.SortBy,
                SortDescending = req.SortDirection == SortDirection.Descending,
                Skip = (req.PageNumber - 1) * req.PageSize,
                Take = req.PageSize
            };

            var result = await _repository.GetAllAsync(queryParams);
            var dtos = result.Items.Select(PurchaseOrderMapper.ToDto).ToList();

            return new PagedResult<PurchaseOrderDto>(dtos, result.TotalCount, req.PageNumber, req.PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting purchase orders");
            throw;
        }
    }
}
