using ERP.Purchasing.Application.Common.DTOs;
using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Application.Common.Mappers;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Factories;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Services;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.ValueObjects;
using ERP.SharedKernel.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Application.PurchaseOrders.Commands.CreatePurchaseOrder;
public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, PurchaseOrderDto>
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPurchaseOrderNumberGeneratorFactory _numberFactory;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<CreatePurchaseOrderCommandHandler> _logger;

    public CreatePurchaseOrderCommandHandler(
        IPurchaseOrderRepository repository,
        IUnitOfWork unitOfWork,
        IPurchaseOrderNumberGeneratorFactory numberFactory,
        IDomainEventDispatcher eventDispatcher,
        ILogger<CreatePurchaseOrderCommandHandler> logger)
    {
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

            var domainService = new PurchaseOrderDomainService(_numberFactory);

            // Create purchase order
            var po = domainService.CreatePurchaseOrder(
                request.IssueDate,
                request.NumberGenerationStrategy);

            // Add items
            foreach (var item in request.Items)
            {
                po.AddItem(
                    new GoodCode(item.GoodCode),
                    new Money(item.Price, item.Currency));
            }

            // Save
            await _repository.AddAsync(po);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Dispatch events
            await _eventDispatcher.DispatchAsync(po.DomainEvents, cancellationToken);
            po.ClearDomainEvents();

            _logger.LogInformation("Created purchase order: {Number}", po.Number.Value);

            return PurchaseOrderMapper.ToDto(po);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating purchase order");
            throw;
        }
    }
}

