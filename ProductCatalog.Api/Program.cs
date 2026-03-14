using Bookify.Infrastructure;
using ProductCatalog.Api.Endpoints.Categories;
using ProductCatalog.Api.Endpoints.Orders;
using ProductCatalog.Api.Endpoints.Payments;
using ProductCatalog.Api.Endpoints.Products;
using ProductCatalog.Application;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapOrderEndpoints();
app.MapPaymentEndpoints();
app.MapProductEndpoints();
app.MapCategoryEndpoints();

app.Run();