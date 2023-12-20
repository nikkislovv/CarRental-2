using Microsoft.AspNetCore.Mvc;
using CatalogApi.Extensions;
using CatalogApi.Repository;
using FluentValidation;
using CatalogApi.DTOs;
using CatalogApi.Validation;
using CatalogApi.Services;
using Serilog;
using CatalogApi.Logger;
using CatalogApi.Data;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                        false, true)
                    .Build();

LoggerConfigurator.ConfigureLogging(configuration);
builder.Host.UseSerilog();

// Add services to the container.
services
    .ConfigureCors()
    .AddSingleton<CatalogContext>()
    .ConfigureJWT(builder.Configuration)
    .AddScoped<ICarElasticsearchService,CarElasticsearchService>()
    .ConfigureElasticsearchStorage(configuration)
    .AddSingleton<ILoggerManager,LoggerManager>()
    .ConfigureMsSqlHealthChecker(builder.Configuration)
    .ConfigureVersioning()
    .AddScoped<ICarService, CarService>()
    .AddScoped<IValidator<CarCreateDto>, CarCreateValidator>()
    .AddScoped<IValidator<CarUpdateDto>, CarUpdateValidator>()
    .AddScoped<ICarRepository, CarRepository>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddControllers();



var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseCors("CorsPolicy");

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseMiddleware<RetryingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
