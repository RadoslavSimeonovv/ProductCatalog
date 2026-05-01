using System.Text.Json.Serialization;
using Asp.Versioning;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using ProductCatalog.Api.Endpoints.Categories;
using ProductCatalog.Api.Endpoints.Orders;
using ProductCatalog.Api.Endpoints.Payments;
using ProductCatalog.Api.Endpoints.Products;
using ProductCatalog.Api.Extensions;
using ProductCatalog.Application;
using ProductCatalog.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "ProductCatalog API", Version = "v1" });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });

    app.ApplyMigrations();
}

app.UseRequestContextLogging();

app.UseCustomExceptionHandler();

app.UseSerilogRequestLogging();

app.UseAuthentication();

app.UseUserContextLogging();

app.UseAuthorization();

var v1 = app.NewVersionedApi();
var v1Group = v1.MapGroup("/api/v{version:apiVersion}").HasApiVersion(1);

v1Group.MapProductEndpoints();
v1Group.MapCategoryEndpoints();
v1Group.MapOrderEndpoints();
v1Group.MapPaymentEndpoints();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();