using ERP.Purchasing.Application.Common.Constants;
using ERP.Purchasing.Application.Common.DTOs;
using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Application.Common.Mappers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace ERP.Purchasing.Application.PurchaseOrders.Queries.GetRecentPurchaseOrders;
public class GetRecentPurchaseOrdersQueryHandler
    : IRequestHandler<GetRecentPurchaseOrdersQuery, List<PurchaseOrderDto>>
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetRecentPurchaseOrdersQueryHandler> _logger;
    private readonly IConfiguration _configuration;
    private readonly IFeatureManagerSnapshot _featureManager;

    public GetRecentPurchaseOrdersQueryHandler(
        IPurchaseOrderRepository repository,
        ICacheService cacheService,
        ILogger<GetRecentPurchaseOrdersQueryHandler> logger,
        IConfiguration configuration,
        IFeatureManagerSnapshot featureManager)
    {
        _repository = repository;
        _cacheService = cacheService;
        _logger = logger;
        _configuration = configuration;
        _featureManager = featureManager;
    }

    public async Task<List<PurchaseOrderDto>> Handle(GetRecentPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            bool isCachingEnabled = _configuration.GetValue<bool>("FeatureFlags:UseMessageBroker");

            if (isCachingEnabled && request.Count == CacheKeys.PurchaseOrderCacheCount)
            {
                var cached = await _cacheService.GetAsync<List<PurchaseOrderDto>>(CacheKeys.RecentPurchaseOrders, cancellationToken);

                if (cached is not null)
                    return cached;
            }

            var purchaseOrders = await _repository.GetRecentAsync(request.Count);
            var dtos = purchaseOrders.Select(PurchaseOrderMapper.ToDto).ToList();

            if (isCachingEnabled && request.Count == CacheKeys.PurchaseOrderCacheCount)
            {
                await _cacheService.SetAsync(CacheKeys.RecentPurchaseOrders, dtos, TimeSpan.FromMinutes(5), cancellationToken);
            }

            return dtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent purchase orders");
            throw;
        }
    }
}
