using ERP.Purchasing.Application.Common.DTOs;
using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Application.Common.Mappers;
using ERP.SharedKernel.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Application.PurchaseOrders.Commands.ApprovePurchaseOrder;
public class ApprovePurchaseOrderCommandHandler
        : IRequestHandler<ApprovePurchaseOrderCommand, PurchaseOrderDto>
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<ApprovePurchaseOrderCommandHandler> _logger;

    public ApprovePurchaseOrderCommandHandler(
        IPurchaseOrderRepository repository,
        IUnitOfWork unitOfWork,
        IDomainEventDispatcher eventDispatcher,
        ILogger<ApprovePurchaseOrderCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
    }

    public async Task<PurchaseOrderDto> Handle(
        ApprovePurchaseOrderCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Approving purchase order: {Id}", request.PurchaseOrderId);

            var po = await _repository.GetByIdAsync(request.PurchaseOrderId);

            po.Approve();

            await _repository.UpdateAsync(po);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _eventDispatcher.DispatchAsync(po.DomainEvents, cancellationToken);
            po.ClearDomainEvents();

            _logger.LogInformation("Approved purchase order: {Number}", po.Number.Value);

            return PurchaseOrderMapper.ToDto(po);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving purchase order: {Id}", request.PurchaseOrderId);
            throw;
        }
    }
}
