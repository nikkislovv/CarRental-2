using CatalogApi.Data.Entities;

namespace CatalogApi.Repository
{
    public interface ICarRepository
    {
        Task<Car?> GetAsync(
            Guid id);

        Task<Car?> GetAsync(
            string vehicleNumber);

        Task<IEnumerable<Car>> GetAsync(
            int Offset,
            int Fetch);

        Task<int> CountAsync();

        Task CreateAsync(
            Car car);

        Task DeleteAsync(
            Car car);

        Task UpdateAsync(
            Car car);
    }
}
