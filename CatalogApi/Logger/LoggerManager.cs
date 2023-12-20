namespace CatalogApi.Logger
{

    public class LoggerManager : ILoggerManager
    {
        private readonly ILogger _logger;

        public LoggerManager(ILogger<LoggerManager> logger)
        {
            _logger = logger;
        }

        public void LogInformation(
            string message,
            params object[] parameters)
        {
            _logger.LogInformation(message, parameters);
        }

        public void LogError(
            Exception exception,
            string message,
            params object[] parameters)
        {
            _logger.LogError(exception, message, parameters);
        }
    }
}
