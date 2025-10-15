using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Purchasing.Infrastructure.Logging;
public static class ElasticsearchExtensions
{
    public static IServiceCollection AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(sp =>
        {
            var config = configuration.GetSection("Elasticsearch").Get<ElasticSettings>();

            var settings = new ElasticsearchClientSettings(new Uri(config.Url))
                .Authentication(new BasicAuthentication(config.Username, config.Password))
                .EnableDebugMode(); //remove this line in production

            return new ElasticsearchClient(settings);
        });

        return services;
    }
}
