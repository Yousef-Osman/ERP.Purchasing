using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Infrastructure.Behaviors;
using ERP.Purchasing.Infrastructure.Caching;
using ERP.Purchasing.Infrastructure.Persistence;
using ERP.Purchasing.Infrastructure.Persistence.Repositories;
using ERP.Purchasing.Infrastructure.Services;
using ERP.SharedKernel.Factories;
using ERP.SharedKernel.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Purchasing.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PurchasingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddSingleton<IDocumentNumberGeneratorFactory, DocumentNumberGeneratorFactory>();

        // Add Validation Behavior
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddRedisCaching(configuration);

        return services;
    }
}
