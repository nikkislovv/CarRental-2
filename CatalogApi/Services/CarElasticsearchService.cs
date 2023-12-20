using CatalogApi.Data.Entities;
using CatalogApi.Logger;
using Nest;

namespace CatalogApi.Services
{
    public class CarElasticsearchService : ICarElasticsearchService
    {
        private readonly ILoggerManager _logger;
        private readonly IElasticClient _elasticClient;

        public CarElasticsearchService(
            ILoggerManager logger,
            IElasticClient elasticClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _elasticClient = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
        }

        public async Task AddAsync(
            Car car,
            CancellationToken cancellationToken)
        {

            var response = await _elasticClient.IndexDocumentAsync(car, cancellationToken);

            _logger.LogInformation(response.IsValid
                ? $"Created a new car in Elasticsearch Storage with id:{car.Id}"
                : $"Cant't create a new car in Elasticsearch Storage with id:{car.Id}");
        }

        public async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken)
        {

            var response = await _elasticClient.DeleteAsync<Car>(id, null, cancellationToken);

            _logger.LogInformation(response.IsValid
                ? $"Deleted a car in Elasticsearch Storage with id:{id}"
                : $"Cant't delete a car in Elasticsearch Storage with id:{id}");
        }

        public async Task UpdateAsync(
            Car car,
            CancellationToken cancellationToken)
        {

            var response = await _elasticClient.UpdateAsync(DocumentPath<Car>.Id(car.Id),
                c => c.Index("cars")
                    .DocAsUpsert(true)
                    .Doc(car), cancellationToken);

            _logger.LogInformation(response.IsValid
                ? $"Updated a new car in Elasticsearch Storage with id:{car.Id}"
                : $"Cant't update a car in Elasticsearch Storage with id:{car.Id}");
        }

    }
}
