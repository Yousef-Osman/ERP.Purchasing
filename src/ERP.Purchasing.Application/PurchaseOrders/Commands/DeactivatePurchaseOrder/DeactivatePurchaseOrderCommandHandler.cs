using ERP.Purchasing.Application.Common.DTOs;
using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Application.Common.Mappers;
using ERP.Purchasing.Domain.PurchaseOrderAggregate;
using ERP.SharedKernel.Exceptions;
using ERP.SharedKernel.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Application.PurchaseOrders.Commands.DeactivatePurchaseOrder;
public class DeactivatePurchaseOrderCommandHandler
        : IRequestHandler<DeactivatePurchaseOrderCommand, PurchaseOrderDto>
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<DeactivatePurchaseOrderCommandHandler> _logger;

    public DeactivatePurchaseOrderCommandHandler(
        IPurchaseOrderRepository repository,
        IUnitOfWork unitOfWork,
        IDomainEventDispatcher eventDispatcher,
        ILogger<DeactivatePurchaseOrderCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
    }

    public async Task<PurchaseOrderDto> Handle(
        DeactivatePurchaseOrderCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deactivating purchase order: {Id}", request.PurchaseOrderId);

            var purchaseOrder = await _repository.GetByIdAsync(request.PurchaseOrderId);

            if (purchaseOrder == null)
                throw new EntityNotFoundException(nameof(PurchaseOrder), request.PurchaseOrderId);

            purchaseOrder.Deactivate();

            await _repository.UpdateAsync(purchaseOrder);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _eventDispatcher.DispatchAsync(purchaseOrder.DomainEvents, cancellationToken);
            purchaseOrder.ClearDomainEvents();

            _logger.LogInformation("Deactivated purchase order: {Number}", purchaseOrder.Number.Value);

            return PurchaseOrderMapper.ToDto(purchaseOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating purchase order: {Id}", request.PurchaseOrderId);
            throw;
        }
    }
}
