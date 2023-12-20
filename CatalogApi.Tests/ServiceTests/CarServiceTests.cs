using AutoFixture;
using CatalogApi.Data.Entities;
using CatalogApi.DTOs;
using CatalogApi.DTOs.RequestFeatures;
using CatalogApi.Extensions;
using CatalogApi.Logger;
using CatalogApi.Repository;
using CatalogApi.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CatalogApi.Tests.ServiceTests
{
    public class CarServiceTests
    {
        private readonly CarService _carService;
        private readonly Mock<ICarRepository> _carRepository;
        private readonly IFixture _fixture;
        private readonly CancellationToken _token;
        private readonly Mock<ILoggerManager> _logger;
        private readonly Mock<ICarElasticsearchService> _elasticsearchService;
        public CarServiceTests()
        {
            _carRepository = new Mock<ICarRepository>();
            _logger = new Mock<ILoggerManager>();
            _elasticsearchService = new Mock<ICarElasticsearchService>();
            _carService = new CarService(_carRepository.Object, _logger.Object, _elasticsearchService.Object);/////
            _fixture = new Fixture();
            _token = new CancellationToken();
        }

        [Fact]
        public async Task IsExistsAsync_CarExists_ReturnsTrue()
        {
            //Arrange
            var car = _fixture.Create<Car>();

            _carRepository.Setup(c => c.GetAsync(car.VehicleNumber))
                .ReturnsAsync(car);
            //Act
            var result = await _carService.IsExistsAsync(car.VehicleNumber);
            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsExistsAsync_CarDoesNotExists_ReturnsFalse()
        {
            //Arrange
            var car = _fixture.Create<Car>();

            _carRepository.Setup(c => c.GetAsync(car.VehicleNumber))
                .ReturnsAsync(default(Car));
            //Act
            var result = await _carService.IsExistsAsync(car.VehicleNumber);
            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsExistsAsync_CarWithOtherIdExists_ReturnsTrue()
        {
            //Arrange
            var car = _fixture.Create<Car>();

            var anyCar = _fixture.Create<Car>();

            _carRepository.Setup(c => c.GetAsync(car.VehicleNumber))
                .ReturnsAsync(anyCar);

            //Act
            var result = await _carService.IsExistsAsync(car.Id, car.VehicleNumber);
            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsExistsAsync_CarWithOtherIdDoesNotExists_ReturnsTrue()
        {
            //Arrange
            var car = _fixture.Create<Car>();

            var anyCar = _fixture.Build<Car>()
                .With(c => c.Id, car.Id)
                .Create();

            _carRepository.Setup(c => c.GetAsync(car.VehicleNumber))
                .ReturnsAsync(anyCar);
            //Act
            var result = await _carService.IsExistsAsync(car.Id, car.VehicleNumber);
            //Assert
            Assert.False(result);
        }

        [Fact]
        public void Create_MappingCarCollectionToCarDtoCollection_ReturnsDtoCollection()
        {
            //Arrange

            var pagedCars = _fixture.Create<PagedList<Car>>();

            var carDtos = new List<CarShowDto>();
            foreach (var item in pagedCars)
            {
                carDtos.Add(_fixture.Build<CarShowDto>()
                    .With(c => c.Id, item.Id)
                    .With(c => c.VehicleNumber, item.VehicleNumber
                        .CapitalazeFirstLetter())
                    .With(c => c.Title, item.Title
                        .CapitalazeFirstLetter())
                    .With(c => c.Model, item.Model
                        .CapitalazeFirstLetter())
                    .With(c => c.ReleaseYear, item.ReleaseYear)
                    .With(c => c.Color, item.Color
                        .CapitalazeFirstLetter())
                    .With(c => c.RentPrice, item.RentPrice)
                    .With(c => c.IsAvailable, item.IsAvailable)
                    .With(c => c.Image, item.Image)
                    .Create());
            }

            var pagedCarDtos = new PagedList<CarShowDto>(carDtos,
                pagedCars.MetaData.TotalCount, pagedCars.MetaData.CurrentPage, pagedCars.MetaData.PageSize);
            //Act
            var result = _carService.Create(pagedCars);
            //Assert
            Assert.Equal(JsonConvert.SerializeObject(pagedCarDtos), JsonConvert.SerializeObject(result));
        }

        [Fact]
        public async Task GetAsync_CarExists_ReturnsCar()
        {
            //Arrange
            var car = _fixture.Create<Car>();

            _carRepository.Setup(c => c.GetAsync(car.Id))
                .ReturnsAsync(car);

            //Act
            var result = await _carService.GetAsync(car.Id);
            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAsync_CarDoesNotExists_ReturnsNull()
        {
            //Arrange
            var id = Guid.NewGuid();

            _carRepository.Setup(c => c.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(default(Car));

            //Act
            var result = await _carService.GetAsync(id);
            //Assert
            Assert.Null(result);
        }

        [Fact]
        public void Create_MappingCarToCarShowDto_ReturnsCarDto()
        {
            //Arrange
            var car = _fixture.Create<Car>();

            var carDto = _fixture.Build<CarShowDto>()
                .With(c => c.Id, car.Id)
                .With(c => c.VehicleNumber, car.VehicleNumber
                        .CapitalazeFirstLetter())
                .With(c => c.Title, car.Title
                        .CapitalazeFirstLetter())
                .With(c => c.Model, car.Model
                        .CapitalazeFirstLetter())
                .With(c => c.ReleaseYear, car.ReleaseYear)
                .With(c => c.Color, car.Color
                        .CapitalazeFirstLetter())
                .With(c => c.RentPrice, car.RentPrice)
                .With(c => c.IsAvailable, car.IsAvailable)
                .With(c => c.Image, car.Image)
                .Create();
            //Act
            var result = _carService.Create(car);
            //Assert
            Assert.Equal(JsonConvert.SerializeObject(carDto), JsonConvert.SerializeObject(result));
        }

        [Fact]
        public void Create_MappingCarShowDtoToCar_ReturnsCar()
        {
            //Arrange
            var carDto = _fixture.Create<CarCreateDto>();

            var car = _fixture.Build<Car>()
                .With(c => c.Id, Guid.Empty)
                .With(c => c.VehicleNumber, carDto.VehicleNumber
                        .ToLowerInvariant())
                .With(c => c.Title, carDto.Title
                        .ToLowerInvariant())
                .With(c => c.Model, carDto.Model
                        .ToLowerInvariant())
                .With(c => c.ReleaseYear, carDto.ReleaseYear)
                .With(c => c.Color, carDto.Color
                        .ToLowerInvariant())
                .With(c => c.RentPrice, carDto.RentPrice)
                .With(c => c.IsAvailable, carDto.IsAvailable)
                .With(c => c.Image, carDto.Image)
                .With(c => c.CreatedAt, DateTime.UtcNow)
                .With(c => c.UpdatedAt, DateTime.UtcNow)
                .Create();
            //Act
            var result = _carService.Create(carDto);

            result.CreatedAt = car.CreatedAt;
            result.UpdatedAt = car.UpdatedAt;
            //Assert
            Assert.Equal(JsonConvert.SerializeObject(car), JsonConvert.SerializeObject(result));
        }

        [Fact]
        public async Task AddAsync_AddingCar_VerifyCreateAndSafeMethods()
        {
            //Arrange
            var car = _fixture.Create<Car>();

            _carRepository.Setup(c => c.CreateAsync(car));


            _elasticsearchService.Setup(c => c.AddAsync(car, _token));
            //Act
            await _carService.AddAsync(car, _token);
            //Assert
            _carRepository.Verify(c => c.CreateAsync(car), Times.Once);
            _elasticsearchService.Verify(c => c.AddAsync(car, _token), Times.Once);
        }

        [Fact]
        public void Create_MappingCarUpdateDtoToCar_ReturnsCar()
        {
            //Arrange
            var car = _fixture.Create<Car>();

            var carDto = _fixture.Create<CarUpdateDto>();

            var carAfterMapping = _fixture.Build<Car>()
                .With(c => c.Id, car.Id)
                .With(c => c.VehicleNumber, carDto.VehicleNumber
                        .ToLowerInvariant())
                .With(c => c.Title, carDto.Title
                        .ToLowerInvariant())
                .With(c => c.Model, carDto.Model
                        .ToLowerInvariant())
                .With(c => c.ReleaseYear, carDto.ReleaseYear)
                .With(c => c.Color, carDto.Color
                        .ToLowerInvariant())
                .With(c => c.RentPrice, carDto.RentPrice)
                .With(c => c.IsAvailable, carDto.IsAvailable)
                .With(c => c.Image, carDto.Image)
                .With(c => c.CreatedAt, car.CreatedAt)
                .With(c => c.UpdatedAt, DateTime.UtcNow)
                .Create();
            //Act
            _carService.Create(car, carDto);
            carAfterMapping.UpdatedAt = car.UpdatedAt;
            //Assert
            Assert.Equal(JsonConvert.SerializeObject(car), JsonConvert.SerializeObject(carAfterMapping));
        }

        [Fact]
        public async Task UpdateAsync_UpdatingCar_VerifyUpdateAndSafeMethods()
        {
            //Arrange
            var car = _fixture.Create<Car>();

            _carRepository.Setup(c => c.UpdateAsync(car));


            _elasticsearchService.Setup(c => c.UpdateAsync(car, _token));
            //Act
            await _carService.UpdateAsync(car, _token);
            //Assert
            _carRepository.Verify(c => c.UpdateAsync(car), Times.Once);
            _elasticsearchService.Verify(c => c.UpdateAsync(car, _token), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeletingCar_VerifyDeleteAndSafeMethods()
        {
            //Arrange
            var car = _fixture.Create<Car>();

            _carRepository.Setup(c => c.DeleteAsync(car));

            _elasticsearchService.Setup(c => c.DeleteAsync(car.Id, _token));
            //Act
            await _carService.DeleteAsync(car, _token);
            //Assert
            _carRepository.Verify(c => c.DeleteAsync(car), Times.Once);
            _elasticsearchService.Verify(c => c.DeleteAsync(car.Id, _token), Times.Once);
        }

        [Fact]
        public async Task AddAsync_AddingCar_VerifyLoggerMessage()
        {
            //Arrange
            var car = _fixture.Create<Car>();

            _carRepository.Setup(c => c.CreateAsync(car));

            _elasticsearchService.Setup(c => c.AddAsync(car, _token));
            //Act
            await _carService.AddAsync(car, _token);
            //Assert
            _logger.Verify(l => l.LogInformation($"Created a new car with id:{car.Id}"), Times.Once);
        }

        [Fact]
        public async Task GetAsync_CarExists_VerifyLoggerMessage()
        {
            //Arrange
            var car = _fixture.Create<Car>();

            _carRepository.Setup(c => c.GetAsync(car.Id))
                .ReturnsAsync(car);
            //Act
            var result = await _carService.GetAsync(car.Id);
            //Assert
            _logger.Verify(l => l.LogInformation($"retrived a car by id: {car.Id}"), Times.Once);
        }

        [Fact]
        public async Task GetAsync_CarDoesNotExists_VerifyLoggerMessage()
        {
            //Arrange
            var car = _fixture.Create<Car>();

            _carRepository.Setup(c => c.GetAsync(car.Id))
                .ReturnsAsync(default(Car));
            //Act
            var result = await _carService.GetAsync(car.Id);
            //Assert
            _logger.Verify(l => l.LogInformation($"can't retrive a car by id: {car.Id}"), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_UpdatingCar_VerifyLoggerMessage()
        {
            //Arrange
            var car = _fixture.Create<Car>();

            _carRepository.Setup(c => c.UpdateAsync(car));

            _elasticsearchService.Setup(c => c.UpdateAsync(car, _token));
            //Act
            await _carService.UpdateAsync(car, _token);
            //Assert
            _logger.Verify(l => l.LogInformation($"Updated a car with id:{car.Id}"));
        }

        [Fact]
        public async Task DeleteAsync_DeletingCar_VerifyLoggerMessage()
        {
            //Arrange
            var car = _fixture.Create<Car>();

            _carRepository.Setup(c => c.DeleteAsync(car));

            _elasticsearchService.Setup(c => c.DeleteAsync(car.Id, _token));
            //Act
            await _carService.DeleteAsync(car, _token);
            //Assert
            _logger.Verify(l => l.LogInformation($"Deleted a car with id:{car.Id}"));
        }
    }
}
