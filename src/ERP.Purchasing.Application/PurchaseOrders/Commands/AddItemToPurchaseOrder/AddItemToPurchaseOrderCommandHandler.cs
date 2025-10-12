using ERP.Purchasing.Application.Common.DTOs;
using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Application.Common.Mappers;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Application.PurchaseOrders.Commands.AddItemToPurchaseOrder;
public class AddItemToPurchaseOrderCommandHandler
        : IRequestHandler<AddItemToPurchaseOrderCommand, PurchaseOrderDto>
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddItemToPurchaseOrderCommandHandler> _logger;

    public AddItemToPurchaseOrderCommandHandler(
        IPurchaseOrderRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<AddItemToPurchaseOrderCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PurchaseOrderDto> Handle(
        AddItemToPurchaseOrderCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Adding item to purchase order: {Id}", request.PurchaseOrderId);

            var po = await _repository.GetByIdAsync(request.PurchaseOrderId);

            po.AddItem(
                new GoodCode(request.GoodCode),
                new Money(request.Price, request.Currency));

            await _repository.UpdateAsync(po);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Added item {GoodCode} to PO: {Number}",
                request.GoodCode, po.Number.Value);

            return PurchaseOrderMapper.ToDto(po);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to purchase order: {Id}", request.PurchaseOrderId);
            throw;
        }
    }
}
