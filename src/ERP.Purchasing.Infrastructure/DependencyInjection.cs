using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Domain.PurchaseOrderAggregate.Factories;
using ERP.Purchasing.Infrastructure.Persistence;
using ERP.Purchasing.Infrastructure.Persistence.Repositories;
using ERP.Purchasing.Infrastructure.Services;
using ERP.SharedKernel.Interfaces;
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

        // Domain Services
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddSingleton<IPurchaseOrderNumberGeneratorFactory, PurchaseOrderNumberGeneratorFactory>();

        return services;
    }
}
