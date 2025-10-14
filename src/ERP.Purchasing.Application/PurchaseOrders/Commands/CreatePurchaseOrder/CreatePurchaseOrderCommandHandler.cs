using ERP.Purchasing.Application.Common.DTOs;
using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Application.Common.Mappers;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Services;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.ValueObjects;
using ERP.SharedKernel.Factories;
using ERP.SharedKernel.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Application.PurchaseOrders.Commands.CreatePurchaseOrder;
public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, PurchaseOrderDto>
{
    private readonly PurchaseOrderDomainService _domainService;
    private readonly IPurchaseOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDocumentNumberGeneratorFactory _numberFactory;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<CreatePurchaseOrderCommandHandler> _logger;

    public CreatePurchaseOrderCommandHandler(
        PurchaseOrderDomainService domainService,
        IPurchaseOrderRepository repository,
        IUnitOfWork unitOfWork,
        IDocumentNumberGeneratorFactory numberFactory,
        IDomainEventDispatcher eventDispatcher,
        ILogger<CreatePurchaseOrderCommandHandler> logger)
    {
        _domainService = domainService;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _numberFactory = numberFactory;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
    }

    public async Task<PurchaseOrderDto> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating new purchase order");

            // Create purchase order
            var purchaseOrder = _domainService.CreatePurchaseOrder(request.IssueDate, request.NumberGenerationMethod);

            // Add items
            foreach (var item in request.Items)
            {
                purchaseOrder.AddItem(new GoodCode(item.GoodCode), new Money(item.Price, item.Currency));
            }

            // Save
            await _repository.AddAsync(purchaseOrder);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Dispatch events
            await _eventDispatcher.DispatchAsync(purchaseOrder.DomainEvents, cancellationToken);
            purchaseOrder.ClearDomainEvents();

            _logger.LogInformation("Created purchase order: {Number}", purchaseOrder.Number.Value);

            return PurchaseOrderMapper.ToDto(purchaseOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating purchase order");
            throw;
        }
    }
}

