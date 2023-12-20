using CatalogApi.Data.Entities;

namespace CatalogApi.Services
{
    public interface ICarElasticsearchService
    {
        Task AddAsync(
            Car car,
            CancellationToken cancellationToken);

        Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task UpdateAsync(
            Car car,
            CancellationToken cancellationToken);
    }
}
