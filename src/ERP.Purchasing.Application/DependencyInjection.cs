using System.Reflection;
using ERP.Purchasing.Application.PurchaseOrders.EventHandlers.Domain;
using ERP.Purchasing.Application.PurchaseOrders.EventHandlers.Integration;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Events;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Services;
using ERP.SharedKernel.Interfaces;
using ERP.SharedKernel.Messaging.Configuration;
using ERP.SharedKernel.Messaging.Publishers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Purchasing.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Add Message Broker with feature flag
        services.AddMessageBroker(configuration, bus =>
        {
            // Register consumers for this service
            bus.AddConsumer<ShippingOrderCreatedConsumer>();
            bus.AddConsumer<ShippingOrderClosedConsumer>();
        });

        // Domain Event Handlers
        services.AddScoped<IDomainEventHandler<PurchaseOrderCreatedEvent>, PurchaseOrderCreatedEventHandler>();
        services.AddScoped<IDomainEventHandler<PurchaseOrderApprovedEvent>, PurchaseOrderApprovedEventHandler>();
        services.AddScoped<IDomainEventHandler<PurchaseOrderBeingShippedEvent>, PurchaseOrderBeingShippedEventHandler>();
        services.AddScoped<IDomainEventHandler<PurchaseOrderClosedEvent>, PurchaseOrderClosedEventHandler>();

        // Integration event publisher
        services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();

        // Domain Services
        services.AddScoped<PurchaseOrderDomainService>();

        return services;
    }
}
