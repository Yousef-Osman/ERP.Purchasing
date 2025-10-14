using ERP.Purchasing.Application.Common.DTOs;
using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Application.Common.Mappers;
using ERP.Purchasing.Domain.PurchaseOrderAggregate;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.ValueObjects;
using ERP.SharedKernel.Exceptions;
using ERP.SharedKernel.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Application.PurchaseOrders.Commands.UpdateItemPrice;
public class UpdateItemPriceCommandHandler
        : IRequestHandler<UpdateItemPriceCommand, PurchaseOrderDto>
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateItemPriceCommandHandler> _logger;

    public UpdateItemPriceCommandHandler(
        IPurchaseOrderRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateItemPriceCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PurchaseOrderDto> Handle(UpdateItemPriceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating item price in purchase order: {Id}", request.PurchaseOrderId);

            var purchaseOrder = await _repository.GetByIdAsync(request.PurchaseOrderId);

            if (purchaseOrder == null)
                throw new EntityNotFoundException(nameof(PurchaseOrder), request.PurchaseOrderId);

            purchaseOrder.UpdateItemPrice(
                new GoodCode(request.GoodCode),
                new Money(request.NewPrice, request.Currency));

            await _repository.UpdateAsync(purchaseOrder);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated item {GoodCode} price in PO: {Number}",
                request.GoodCode, purchaseOrder.Number.Value);

            return PurchaseOrderMapper.ToDto(purchaseOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating item price in purchase order: {Id}", request.PurchaseOrderId);
            throw;
        }
    }
}
