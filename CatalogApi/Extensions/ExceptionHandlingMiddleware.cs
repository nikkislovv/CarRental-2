using System.Text.Json;
using CatalogApi.Logger;
using SendGrid.Helpers.Errors.Model;

namespace CatalogApi.Extensions
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerManager _logger;
        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILoggerManager logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(
            HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, DateTime.UtcNow);
                await HandleExceptionAsync(context, e);
            }
        }
        private async Task HandleExceptionAsync(
            HttpContext httpContext,
            Exception exception)
        {
            var statusCode = GetStatusCode(exception);
            var response = new
            {
                status = statusCode,
                message = exception.Message,
            };
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        private int GetStatusCode(Exception exception) =>
            exception switch
            {
                ArgumentNullException => StatusCodes.Status400BadRequest,
                BadRequestException => StatusCodes.Status400BadRequest,
                FluentValidation.ValidationException => StatusCodes.Status422UnprocessableEntity,
                OperationCanceledException => 499,
                _ => StatusCodes.Status500InternalServerError
            };
    }
}
