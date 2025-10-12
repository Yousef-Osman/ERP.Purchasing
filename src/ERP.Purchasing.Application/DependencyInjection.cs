using System.Reflection;
using ERP.Purchasing.Application.PurchaseOrders.EventHandlers.Domain;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Events;
using ERP.SharedKernel.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Purchasing.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Domain Event Handlers
        services.AddScoped<IDomainEventHandler<PurchaseOrderCreatedEvent>, PurchaseOrderCreatedEventHandler>();
        services.AddScoped<IDomainEventHandler<PurchaseOrderApprovedEvent>, PurchaseOrderApprovedEventHandler>();
        services.AddScoped<IDomainEventHandler<PurchaseOrderBeingShippedEvent>, PurchaseOrderBeingShippedEventHandler>();
        services.AddScoped<IDomainEventHandler<PurchaseOrderClosedEvent>, PurchaseOrderClosedEventHandler>();

        return services;
    }
}
