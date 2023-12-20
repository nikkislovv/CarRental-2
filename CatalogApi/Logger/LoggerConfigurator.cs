using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;

namespace CatalogApi.Logger
{
    public static class LoggerConfigurator
    {
        public static void ConfigureLogging(
            IConfigurationRoot configuration)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            Log.Logger = new LoggerConfiguration()
                         .Enrich.FromLogContext()
                         .Enrich.WithExceptionDetails()
                         .WriteTo.Debug()
                         .WriteTo.Console()
                         .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
                         .CreateLogger();
        }

        private static ElasticsearchSinkOptions ConfigureElasticSink(
            IConfiguration configuration,
            string? environment)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
            };
        }
    }
}
