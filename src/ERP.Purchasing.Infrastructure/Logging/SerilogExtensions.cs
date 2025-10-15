using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace ERP.Purchasing.Infrastructure.Logging;
public static class SerilogExtensions
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        var elasticConfig = builder.Configuration.GetSection("Elasticsearch").Get<ElasticSettings>();

        var applicationName = builder.Environment.ApplicationName;
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        //var indexFormat = $"{applicationName.ToLower().Replace(".", "-")}-logs-{DateTime.UtcNow:yyyy-MM-dd}";

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithExceptionDetails()
            .Enrich.WithProperty("Environment", environment)
            .Enrich.WithProperty("ApplicationName", applicationName)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticConfig.Url))
            {
                AutoRegisterTemplate = true,
                IndexFormat = elasticConfig.IndexFormat,
                NumberOfReplicas = 1,
                NumberOfShards = 2,
            })
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Services.AddSingleton(Log.Logger);
        builder.Host.UseSerilog();

        return builder;
    }
}
