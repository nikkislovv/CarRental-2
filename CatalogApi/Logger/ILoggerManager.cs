namespace CatalogApi.Logger
{
    public interface ILoggerManager
    {
        void LogInformation(
            string message,
            params object[] parameters);
        void LogError(
            Exception exception,
            string message,
            params object[] parameters);
    }
}
