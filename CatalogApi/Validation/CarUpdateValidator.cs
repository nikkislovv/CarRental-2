using CatalogApi.DTOs;
using FluentValidation;
using System.Text.RegularExpressions;

namespace CatalogApi.Validation
{
    public class CarUpdateValidator : AbstractValidator<CarUpdateDto>
    {
        public CarUpdateValidator()
        {
            var regexVehicleNumber = new Regex(@"^\d{4}[A-Z]{2}-\d$");

            var regexLettersAndDashes = new Regex(@"^[a-zA-Z\-]+$");

            var regexLettersNumbersAndSymbols = new Regex(@"^[a-zA-Z0-9\-\,]+$");

            RuleFor(c => c.VehicleNumber).NotEmpty()
                .Length(8)
                .Matches(regexVehicleNumber)
                .WithMessage("Vehicle Number does not matches the pattern or less or more than 8 characters");

            RuleFor(c => c.Title).NotEmpty()
                .MaximumLength(20)
                .Matches(regexLettersAndDashes)
                .WithMessage("The Title field does not matches the pattern or more then 20 characters");

            RuleFor(c => c.Model).NotEmpty()
                .MaximumLength(30)
                .Matches(regexLettersNumbersAndSymbols)
                .WithMessage("The Model field does not matches the pattern or more then 30 characters");

            RuleFor(c => c.ReleaseYear).NotEmpty()
                .InclusiveBetween(new DateTime(1900, 1, 1), DateTime.UtcNow)
                .WithMessage("The ReleaseYear field cant be empty or not valid date of creating");

            RuleFor(c => c.Color).NotEmpty()
                .Must(IsExists)
                .WithMessage("The Color field cant be empty or not valid color");

            RuleFor(c => c.RentPrice).NotEmpty()
                .InclusiveBetween(100, 3000)
                .WithMessage("The RentPrice field cant be empty or not valid Price");

            RuleFor(c => c.IsAvailable).NotEmpty()
                .WithMessage("The IsAvailable field cant be empty");

            RuleFor(c => c.Image).NotEmpty()
               .WithMessage("The Image field cant be empty");
        }

        private bool IsExists(string color)
        {
            var colorList = new List<string>()
            {
                "red","black","white","light gray","dark gray","yellow","orange","brown","green","cyan","blue","pink"
            };
            return colorList.Contains(color.ToLowerInvariant());
        }
    }
}
