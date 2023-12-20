using CatalogApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogApi.Data.Configurations
{
    public class CarConfigurations : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> builder)
        {
            builder.HasData(
            new Car
            {
                Id = new Guid("80abbca8-664d-4b20-b5de-024705497d4a"),
                VehicleNumber = "1234AB-3".ToLowerInvariant(),
                Title = "BMW".ToLowerInvariant(),
                Model = "G30".ToLowerInvariant(),
                ReleaseYear = new DateTime(2019, 1, 1),
                Color = "Blue".ToLowerInvariant(),
                RentPrice = 300,
                IsAvailable = true,
                CreatedAt = DateTime.Today,
                Image = "https://paultan.org/image/2017/04/P90256342_highRes-e1493375941446-750x423.jpg"
            },
            new Car
            {
                Id = new Guid("81abbca8-664d-4b20-b5de-024705497d4a"),
                VehicleNumber = "4321BC-7".ToLowerInvariant(),
                Title = "Merсedes".ToLowerInvariant(),
                Model = "C43".ToLowerInvariant(),
                ReleaseYear = new DateTime(2020, 1, 1),
                Color = "Black".ToLowerInvariant(),
                RentPrice = 600,
                IsAvailable = true,
                CreatedAt = DateTime.Today,
                Image = "https://cdn.motor1.com/images/mgl/Rqqn89/s3/2023-mercedes-amg-c43-front-3-4.webp"
            },
            new Car
            {
                Id = new Guid("82abbca8-664d-4b20-b5de-024705497d4a"),
                VehicleNumber = "5674RY-7".ToLowerInvariant(),
                Title = "Audi".ToLowerInvariant(),
                Model = "RS4".ToLowerInvariant(),
                ReleaseYear = new DateTime(2021, 1, 1),
                Color = "Black".ToLowerInvariant(),
                RentPrice = 550,
                IsAvailable = true,
                CreatedAt = DateTime.Today,
                Image = "https://s.auto.drom.ru/i24251/c/photos/generations/500x_audi_rs4_g12456.jpg?981264"
            }
            );
        }
    }
}
