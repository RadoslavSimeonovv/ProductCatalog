using ProductCatalog.Api.Endpoints.Categories;
using ProductCatalog.Api.Endpoints.Orders;
using ProductCatalog.Api.Endpoints.Payments;
using ProductCatalog.Api.Endpoints.Products;
using ProductCatalog.Api.Extensions;
using ProductCatalog.Application;
using ProductCatalog.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();

app.UseCustomExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapOrderEndpoints();
app.MapPaymentEndpoints();
app.MapProductEndpoints();
app.MapCategoryEndpoints();

app.Run();