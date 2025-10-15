using ERP.Purchasing.Application.Common.Interfaces;
using ERP.Purchasing.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Purchasing.Infrastructure.Caching;
public static class CachingExtensions
{
    public static IServiceCollection AddRedisCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnection");
            options.InstanceName = "Purchasing_";
        });

        services.AddScoped<ICacheService, CacheService>();

        return services;
    }
}
