using CatalogApi.Data;
using CatalogApi.Data.Entities;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CatalogApi.Repository
{
    public class CarRepository : ICarRepository
    {
        private readonly CatalogContext _context;

        public CarRepository(
            CatalogContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Car?> GetAsync(
            Guid id)
        {
            var query = @"SELECT * FROM Cars
                          WHERE id = @id";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Car>(query, new { id });
            }
        }

        public async Task<Car?> GetAsync(
            string vehicleNumber)
        {
            var query = @"SELECT * FROM Cars
                          WHERE vehicleNumber = @vehicleNumber";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Car>(query, new { vehicleNumber });
            }
        }

        public async Task<IEnumerable<Car>> GetAsync(
            int Offset,
            int Fetch)
        {
            var query = @"SELECT * FROM Cars
                          ORDER BY Title
                          OFFSET @Offset ROWS
                          FETCH NEXT @Fetch ROWS ONLY";

            var parameters = new DynamicParameters();
            parameters.Add("Offset", Offset, DbType.Int32);
            parameters.Add("Fetch", Fetch, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Car>(query, parameters);
            }
        }

        public async Task<int> CountAsync()
        {
            var query = @"SELECT COUNT(*) FROM Cars";

            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>(query);
            }
        }

        public async Task CreateAsync(
            Car car)
        {
            var query = @"INSERT INTO Cars (Id, VehicleNumber, Title, Model, ReleaseYear, Color, RentPrice, IsAvailable, Image, CreatedAt, UpdatedAt)
                          VALUES (@Id ,@VehicleNumber, @Title, @Model, @ReleaseYear, @Color, @RentPrice, @IsAvailable, @Image, @CreatedAt, @UpdatedAt)";

            var parameters = new DynamicParameters();
            parameters.Add("Id", car.Id, DbType.Guid);
            parameters.Add("VehicleNumber", car.VehicleNumber, DbType.String);
            parameters.Add("Title", car.Title, DbType.String);
            parameters.Add("Model", car.Model, DbType.String);
            parameters.Add("ReleaseYear", car.ReleaseYear, DbType.DateTime);
            parameters.Add("Color", car.Color, DbType.String);
            parameters.Add("RentPrice", car.RentPrice, DbType.Decimal);
            parameters.Add("IsAvailable", car.IsAvailable, DbType.Boolean);
            parameters.Add("Image", car.Image, DbType.String);
            parameters.Add("CreatedAt", car.CreatedAt, DbType.DateTime);
            parameters.Add("UpdatedAt", car.UpdatedAt, DbType.DateTime);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task DeleteAsync(
            Car car)
        {
            var query = "DELETE FROM Cars WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { car.Id });
            }
        }

        public async Task UpdateAsync(
            Car car)
        {
            var query = @"UPDATE Cars SET VehicleNumber = @VehicleNumber,
                                          Title = @Title,
                                          Model = @Model,
                                          ReleaseYear = @ReleaseYear,
                                          Color = @Color,
                                          RentPrice = @RentPrice,
                                          IsAvailable = @IsAvailable,
                                          Image = @Image
                        WHERE Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", car.Id, DbType.Guid);
            parameters.Add("VehicleNumber", car.VehicleNumber, DbType.String);
            parameters.Add("Title", car.Title, DbType.String);
            parameters.Add("Model", car.Model, DbType.String);
            parameters.Add("ReleaseYear", car.ReleaseYear, DbType.DateTime);
            parameters.Add("Color", car.Color, DbType.String);
            parameters.Add("RentPrice", car.RentPrice, DbType.Decimal);
            parameters.Add("IsAvailable", car.IsAvailable, DbType.Boolean);
            parameters.Add("Image", car.Image, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }
    }
}
