using CatalogApi.Logger;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using Polly.Retry;
using Polly.Wrap;

namespace CatalogApi.Extensions
{
    public class RetryingMiddleware
    {
        private readonly RequestDelegate _next;
        private const int MaxRetries = 3;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly ILoggerManager _logger;

        public RetryingMiddleware(
            RequestDelegate next,
            ILoggerManager logger)
        {
            _next = next;
            _logger = logger;
            _retryPolicy = Policy.Handle<SqlException>().WaitAndRetryAsync(
                retryCount: MaxRetries,
                sleepDurationProvider: (attemptCount) => TimeSpan.FromSeconds(attemptCount * 2),
                onRetry: (exception, sleepDuration, attemptNumber, context) =>
                {
                    _logger.LogError(exception, $"Transient error. Retrying in {sleepDuration}. {attemptNumber} / {MaxRetries}");
                });
        }

        public async Task InvokeAsync(
            HttpContext context)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _next(context);
            });
        }
    }
}
