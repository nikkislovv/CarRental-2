using CatalogApi.Data;
using CatalogApi.Data.Entities;
using CatalogApi.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Nest;
using System.Net;
using System.Text;

namespace CatalogApi.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigureApi(
           this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            return services;
        }

        public static IServiceCollection ConfigureVersioning(
            this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
            });

            return services;
        }

        public static IServiceCollection ConfigureMsSqlHealthChecker(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHealthChecks().AddCheck("sql", () =>
            {
                string sqlHealthCheckDescription = "Tests that we can connect and select from the MsSql database.";
                string connectionString = configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("SELECT TOP(1) id from dbo.Cars", connection);
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        return HealthCheckResult.Unhealthy(sqlHealthCheckDescription);
                    }
                }

                return HealthCheckResult.Healthy(sqlHealthCheckDescription);
            });

            return services;
        }

        public static IServiceCollection ConfigureElasticsearchStorage(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var settings = new ConnectionSettings(new Uri(configuration["ElasticConfiguration:Uri"]))
                .DefaultIndex("cars-index");

            var client = new ElasticClient(settings);
            services.AddSingleton<IElasticClient>(client);

            var indexExists = client.Indices.Exists("cars-index");
            if (!indexExists.Exists)
            {
                var response = client.Indices.Create("cars-index",
                   index => index.Map<Car>(
                       x => x.AutoMap()
                   ));
            }

            return services;
        }

        public static IServiceCollection ConfigureJWT(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero,

                        ValidAudience = configuration["JWT:ValidAudience"],
                        ValidIssuer = configuration["JWT:ValidIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
                    };
                });

            return services;
        }

        public static IServiceCollection ConfigureCors(
            this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .WithExposedHeaders("Pagination"));
            });

            return services;
        }
    }
}
