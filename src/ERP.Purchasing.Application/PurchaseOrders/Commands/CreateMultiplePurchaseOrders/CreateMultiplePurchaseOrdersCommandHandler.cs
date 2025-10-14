using ERP.Purchasing.Application.Common.DTOs;
using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Application.Common.Mappers;
using ERP.Purchasing.Domain.PurchaseOrderAggregate;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Services;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.ValueObjects;
using ERP.SharedKernel.Exceptions;
using ERP.SharedKernel.Factories;
using ERP.SharedKernel.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Purchasing.Application.PurchaseOrders.Commands.CreateMultiplePurchaseOrders;
public class CreateMultiplePurchaseOrdersCommandHandler
        : IRequestHandler<CreateMultiplePurchaseOrdersCommand, List<PurchaseOrderDto>>
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDocumentNumberGeneratorFactory _numberFactory;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<CreateMultiplePurchaseOrdersCommandHandler> _logger;

    public CreateMultiplePurchaseOrdersCommandHandler(
        IPurchaseOrderRepository repository,
        IUnitOfWork unitOfWork,
        IDocumentNumberGeneratorFactory numberFactory,
        IDomainEventDispatcher eventDispatcher,
        ILogger<CreateMultiplePurchaseOrdersCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _numberFactory = numberFactory;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
    }

    public async Task<List<PurchaseOrderDto>> Handle(
        CreateMultiplePurchaseOrdersCommand request,
        CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        var validPOs = new List<PurchaseOrder>();
        var domainService = new PurchaseOrderDomainService(_numberFactory);

        _logger.LogInformation("Creating {Count} purchase orders", request.PurchaseOrders.Count);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Single loop validation across all layers
            foreach (var (poRequest, index) in request.PurchaseOrders.Select((po, i) => (po, i)))
            {
                try
                {
                    // Create PO
                    var purchaseOrder = domainService.CreatePurchaseOrder(poRequest.IssueDate);

                    foreach (var item in poRequest.Items)
                    {
                        purchaseOrder.AddItem(new GoodCode(item.GoodCode), new Money(item.Price, item.Currency));
                    }

                    validPOs.Add(purchaseOrder);
                }
                catch (Exception ex)
                {
                    errors.Add($"PO #{index + 1}: {ex.Message}");
                    _logger.LogWarning(ex, "Validation failed for PO #{Index}", index + 1);
                }
            }

            if (errors.Any())
            {
                throw new DomainException($"Validation errors: {string.Join("; ", errors)}");
            }

            // Atomic save
            await _repository.AddRangeAsync(validPOs);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Dispatch all events
            foreach (var po in validPOs)
            {
                await _eventDispatcher.DispatchAsync(po.DomainEvents, cancellationToken);
                po.ClearDomainEvents();
            }

            _logger.LogInformation("Successfully created {Count} purchase orders", validPOs.Count);

            return validPOs.Select(PurchaseOrderMapper.ToDto).ToList();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error creating multiple purchase orders");
            throw;
        }
    }
}
