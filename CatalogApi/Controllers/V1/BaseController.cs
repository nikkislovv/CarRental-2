using CatalogApi.DTOs;
using CatalogApi.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CatalogApi.Controllers.V1
{

    public class BaseController : ControllerBase
    {
        protected readonly ICarService _carService;
        protected readonly IValidator<CarCreateDto> _createValidator;
        protected readonly IValidator<CarUpdateDto> _updateValidator;
        public BaseController(
            ICarService carService,
            IValidator<CarCreateDto> createValidator,
            IValidator<CarUpdateDto> updateValidator)
        {
            _carService = carService ?? throw new ArgumentNullException(nameof(carService));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }
    }
}
