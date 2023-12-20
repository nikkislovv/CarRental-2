using CatalogApi.Data.Entities;
using CatalogApi.DTOs;
using CatalogApi.DTOs.RequestFeatures;

namespace CatalogApi.Services
{
    public interface ICarService
    {
        Task<bool> IsExistsAsync(
            string vehicleNumber);

        Task<bool> IsExistsAsync(
            Guid id,
            string vehicleNumber);

        Task<PagedList<Car>> GetAllAsync(
            RequestParameters requestParameters);

        PagedList<CarShowDto> Create(
            PagedList<Car> pagedCars);

        CarShowDto Create(
            Car car);

        Car Create(
           CarCreateDto carCreateDto);

        void Create(
         Car car,
         CarUpdateDto carUpdateDto);

        Task<Car?> GetAsync(
            Guid id);

        Task<Guid> AddAsync(
            Car car,
            CancellationToken cancellationToken);

        Task UpdateAsync(
            Car car,
            CancellationToken cancellationToken);

        Task DeleteAsync(
            Car car,
            CancellationToken cancellationToken);
    }
}
