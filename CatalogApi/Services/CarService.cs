using CatalogApi.Data.Entities;
using CatalogApi.DTOs;
using CatalogApi.DTOs.RequestFeatures;
using CatalogApi.Extensions;
using CatalogApi.Logger;
using CatalogApi.Repository;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.Services
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;
        private readonly ILoggerManager _logger;
        private readonly ICarElasticsearchService _carElasticsearchService;
        public CarService(
            ICarRepository carRepository,
            ILoggerManager logger,
            ICarElasticsearchService carElasticsearchService)
        {
            _carRepository = carRepository ?? throw new ArgumentNullException(nameof(carRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _carElasticsearchService = carElasticsearchService ?? throw new ArgumentNullException(nameof(carElasticsearchService));
        }

        public async Task<bool> IsExistsAsync(
            string vehicleNumbern)
        {
            var car = await _carRepository
                .GetAsync(vehicleNumbern);

            return car != null;
        }

        public async Task<bool> IsExistsAsync(
            Guid id,
            string vehicleNumber)
        {
            var car = await _carRepository
                .GetAsync(vehicleNumber);

            return car != null && car.Id != id;
        }

        public async Task<PagedList<Car>> GetAllAsync(
            RequestParameters requestParameters)
        {
            var cars = (await _carRepository.GetAsync((requestParameters.PageNumber - 1) * requestParameters.PageSize, requestParameters.PageSize)).ToList();

            _logger.LogInformation(cars.Count() > 0
               ? $"retrived car(s) "
               : $"can't retrive cars");

            var totalCount = await _carRepository.CountAsync();

            return new PagedList<Car>(cars, totalCount, requestParameters.PageNumber, requestParameters.PageSize);
        }

        public PagedList<CarShowDto> Create(
            PagedList<Car> pagedCars)
        {
            List<CarShowDto> carDtos = new List<CarShowDto>();

            foreach (var item in pagedCars)
            {
                carDtos.Add(new CarShowDto()
                {
                    Id = item.Id,
                    VehicleNumber = item.VehicleNumber.CapitalazeFirstLetter(),
                    Title = item.Title.CapitalazeFirstLetter(),
                    Model = item.Model.CapitalazeFirstLetter(),
                    ReleaseYear = item.ReleaseYear,
                    Color = item.Color.CapitalazeFirstLetter(),
                    RentPrice = item.RentPrice,
                    IsAvailable = item.IsAvailable,
                    Image = item.Image
                });
            }

            return new PagedList<CarShowDto>(carDtos, pagedCars.MetaData.TotalCount, pagedCars.MetaData.CurrentPage, pagedCars.MetaData.PageSize);
        }

        public async Task<Car?> GetAsync(
            Guid id)
        {
            var car = await _carRepository.GetAsync(id);

            _logger.LogInformation(car != null
               ? $"retrived a car by id: {id}"
               : $"can't retrive a car by id: {id}");

            return car;
        }

        public CarShowDto Create(
            Car car)
        {
            return new CarShowDto()
            {
                Id = car.Id,
                VehicleNumber = car.VehicleNumber.CapitalazeFirstLetter(),
                Title = car.Title.CapitalazeFirstLetter(),
                Model = car.Model.CapitalazeFirstLetter(),
                ReleaseYear = car.ReleaseYear,
                Color = car.Color.CapitalazeFirstLetter(),
                RentPrice = car.RentPrice,
                IsAvailable = car.IsAvailable,
                Image = car.Image
            };
        }

        public Car Create(
            CarCreateDto carCreateDto)
        {
            return new Car()
            {
                VehicleNumber = carCreateDto.VehicleNumber.ToLowerInvariant(),
                Title = carCreateDto.Title.ToLowerInvariant(),
                Model = carCreateDto.Model.ToLowerInvariant(),
                ReleaseYear = carCreateDto.ReleaseYear,
                Color = carCreateDto.Color.ToLowerInvariant(),
                RentPrice = carCreateDto.RentPrice,
                IsAvailable = carCreateDto.IsAvailable,
                Image = carCreateDto.Image,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public async Task<Guid> AddAsync(
            Car car,
            CancellationToken cancellationToken)
        {
            car.Id = Guid.NewGuid();
            await _carRepository.CreateAsync(car);

            await _carElasticsearchService.AddAsync(car, cancellationToken);

            _logger.LogInformation($"Created a new car with id:{car.Id}");

            return car.Id;
        }

        public void Create(
            Car car,
            CarUpdateDto carUpdateDto)
        {
            car.VehicleNumber = carUpdateDto.VehicleNumber.ToLowerInvariant();
            car.Title = carUpdateDto.Title.ToLowerInvariant();
            car.Model = carUpdateDto.Model.ToLowerInvariant();
            car.ReleaseYear = carUpdateDto.ReleaseYear;
            car.Color = carUpdateDto.Color.ToLowerInvariant();
            car.RentPrice = carUpdateDto.RentPrice;
            car.IsAvailable = carUpdateDto.IsAvailable;
            car.Image = carUpdateDto.Image;
            car.UpdatedAt = DateTime.UtcNow;
        }

        public async Task UpdateAsync(
            Car car,
            CancellationToken cancellationToken)
        {
            await _carRepository.UpdateAsync(car);

            await _carElasticsearchService.UpdateAsync(car, cancellationToken);

            _logger.LogInformation($"Updated a car with id:{car.Id}");
        }

        public async Task DeleteAsync(
            Car car,
            CancellationToken cancellationToken)
        {
            await _carRepository.DeleteAsync(car);

            await _carElasticsearchService.DeleteAsync(car.Id, cancellationToken);

            _logger.LogInformation($"Deleted a car with id:{car.Id}");
        }
    }
}
