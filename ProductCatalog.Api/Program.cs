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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
}

app.UseRequestContextLogging();

app.UseCustomExceptionHandler();

app.UseSerilogRequestLogging();

app.UseAuthentication();

app.UseUserContextLogging();

app.UseAuthorization();

app.MapOrderEndpoints();
app.MapPaymentEndpoints();
app.MapProductEndpoints();
app.MapCategoryEndpoints();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();