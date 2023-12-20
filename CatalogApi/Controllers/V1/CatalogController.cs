using CatalogApi.DTOs;
using CatalogApi.DTOs.RequestFeatures;
using CatalogApi.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CatalogApi.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/{v:apiversion}/catalog")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CatalogController : BaseController
    {
        public CatalogController(
            ICarService carService,
            IValidator<CarCreateDto> createValidator,
            IValidator<CarUpdateDto> updateValidator)
             : base(carService, createValidator, updateValidator)
        {
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCarsAsync(
            [FromQuery] RequestParameters requestParameters)
        {
            var cars = await _carService
                    .GetAllAsync(requestParameters);
            var carDtos = _carService.Create(cars);

            Response.Headers.Add("Pagination", JsonConvert.SerializeObject(carDtos.MetaData));

            return carDtos.Count() > 0
                    ? Ok(carDtos)
                    : NotFound(new { message = "There are no cars" });
        }

        [HttpGet("{Id}", Name = "Get")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCarByIdAsync(
            Guid id)
        {
            var car = await _carService
                .GetAsync(id);
            var carDto = _carService.Create(car);

            return carDto != null
                ? Ok(carDto)
                : NotFound(new { message = "There is no car" });
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateCarAsync(
            CarCreateDto carCreateDto,
            CancellationToken cancellationToken)
        {
            ValidationResult result = await _createValidator
                .ValidateAsync(carCreateDto, cancellationToken);

            if (!result.IsValid)
            {
                return UnprocessableEntity(new { message = "Car is not valid" });
            }

            var isExists = await _carService
                .IsExistsAsync(carCreateDto.VehicleNumber.ToLowerInvariant());

            if (!isExists)
            {
                var car = _carService.Create(carCreateDto);
                var id = await _carService.AddAsync(car, cancellationToken);

                return id != Guid.Empty
                    ? CreatedAtRoute("Get", new { Id = id }, id)
                    : BadRequest(new { message = "Sth went wrong" });
            }

            return BadRequest(new { message = "Car with such Vehicle Number already exists" });
        }

        [HttpPut("{Id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCarAsync(
            Guid id,
            CarUpdateDto carUpdateDto,
            CancellationToken cancellationToken)
        {
            ValidationResult result = await _updateValidator
                .ValidateAsync(carUpdateDto, cancellationToken);

            if (!result.IsValid)
            {
                return UnprocessableEntity(new { message = "Car is not valid" });
            }

            var car = await _carService.GetAsync(id);

            if (car == null)
            {
                return NotFound(new { message = "There is no car" });
            }

            var isExists = await _carService
                .IsExistsAsync(id, carUpdateDto.VehicleNumber.ToLowerInvariant());

            if (!isExists)
            {
                _carService.Create(car, carUpdateDto);
                await _carService.UpdateAsync(car, cancellationToken);

                return NoContent();
            }

            return BadRequest(new { message = "Car with such Vehicle Number already exists" });
        }

        [HttpDelete("{Id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCarAsync(
            Guid Id,
            CancellationToken cancellationToken)
        {
            var car = await _carService.GetAsync(Id);

            if (car != null)
            {
                await _carService.DeleteAsync(car, cancellationToken);

                return NoContent();
            }

            return NotFound(new { message = "There is no car" });
        }
    }
}
