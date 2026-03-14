using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Api.Common;
using ProductCatalog.Api.Endpoints.Categories;
using ProductCatalog.Application.Catalog.AddProductFeature;
using ProductCatalog.Application.Catalog.ChangeProductCategory;
using ProductCatalog.Application.Catalog.ChangeProductPrice;
using ProductCatalog.Application.Catalog.CreateCategory;
using ProductCatalog.Application.Catalog.CreateProduct;
using ProductCatalog.Application.Catalog.DeactivateProduct;
using ProductCatalog.Application.Catalog.DiscontinueProduct;
using ProductCatalog.Application.Catalog.GetAllCategories;
using ProductCatalog.Application.Catalog.GetCategoryById;
using ProductCatalog.Application.Catalog.GetProductById;
using ProductCatalog.Application.Catalog.GetProductFeatures;
using ProductCatalog.Application.Catalog.GetProducts;
using ProductCatalog.Application.Catalog.PaginatedResponse;
using ProductCatalog.Application.Catalog.PublishProduct;
using ProductCatalog.Application.Catalog.RemoveProductFeature;
using ProductCatalog.Application.Catalog.Responses;
using ProductCatalog.Application.Catalog.UpdateCategoryCommand;
using ProductCatalog.Application.Catalog.UpdateProductFeature;

namespace ProductCatalog.Api.Endpoints.Products;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/products")
            .WithTags("Products");

        MapGetProducts(group);
        MapGetProductById(group);
        MapGetProductFeatures(group);
        MapCreateProduct(group);
        MapPublishProduct(group);
        MapDiscontinueProduct(group);
        MapDeactivateProduct(group);
        MapUpdateProductFeature(group);
        MapRemoveProductFeature(group);
        MapAddProductFeature(group);
        MapChangeProductCategory(group);
        MapChangeProductPrice(group);

        return app;
    }

    private static void MapGetProductById(RouteGroupBuilder group)
    {
        group.MapGet("/{productId:guid}", async (
            Guid productId,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetProductByIdQuery(productId), ct);

            return result.ToHttpResult();
        })
            .WithName("GetProductById")
            .WithSummary("Gets a product by id")
            .Produces<ProductResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static void MapGetProducts(RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            [AsParameters] GetProductsRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new GetProductsQuery(
                    request.PageNumber,
                    request.PageSize,
                    request.ProductStatus,
                    request.CategoryId,
                    request.SortBy,
                    request.SearchTerm),
                ct);
            return result.ToHttpResult();
        })
            .WithName("GetProducts")
            .WithSummary("Gets all products")
            .Produces<PagedResult<ProductResponse>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static void MapGetProductFeatures(RouteGroupBuilder group)
    {
        group.MapGet("/{productId:guid}/features", async (
            Guid productId,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetProductFeaturesQuery(productId), ct);
            return result.ToHttpResult();
        })
            .WithName("GetProductFeatures")
            .WithSummary("Gets features of a product")
            .Produces<GetProductFeaturesQueryResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static void MapCreateProduct(RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            CreateProductRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateProductCommand(request.Name, 
                request.Description,
                request.Sku, 
                request.CategoryId,
                request.Price.Amount,
                request.Price.Currency);

            var result = await sender.Send(command, ct);

            if (result.IsSuccess)
                return Results.Created($"/products/{result.Value}", result.Value);
            return result.ToHttpResult();
        })
            .WithName("CreateProduct")
            .WithSummary("Creates a new product")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static void MapDeactivateProduct(RouteGroupBuilder group)
    {
        group.MapPut("/{productId:guid}/deactivate", async (
            Guid productId,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new DeactivateProductCommand(productId);
            var result = await sender.Send(command, ct);

            if (result.IsSuccess)
                return Results.NoContent();
            return result.ToHttpResult();
        })
            .WithName("DeactivateProduct")
            .WithSummary("Deactivates a product")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static void MapDiscontinueProduct(RouteGroupBuilder group)
    {
        group.MapPut("/{productId:guid}/discontinue", async (
            Guid productId,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new DiscontinueProductCommand(productId);
            var result = await sender.Send(command, ct);
            if (result.IsSuccess)
                return Results.NoContent();
            return result.ToHttpResult();
        })
            .WithName("DiscontinueProduct")
            .WithSummary("Discontinues a product")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static void MapPublishProduct(RouteGroupBuilder group)
    {
        group.MapPut("/{productId:guid}/publish", async (
            Guid productId,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new PublishProductCommand(productId);
            var result = await sender.Send(command, ct);
            if (result.IsSuccess)
                return Results.NoContent();
            return result.ToHttpResult();
        })
            .WithName("PublishProduct")
            .WithSummary("Publishes a product")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static void MapAddProductFeature(RouteGroupBuilder group)
    {
        group.MapPost("/{productId:guid}/features", async (
            Guid productId,
            AddProductFeatureRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new AddProductFeatureCommand(productId, request.Name, request.Value, request.DisplayOrder);
            var result = await sender.Send(command, ct);
            if (result.IsSuccess)
                return Results.Created($"/products/{productId}/features/{result.Value}", null);
            return result.ToHttpResult();
        })
            .WithName("AddProductFeature")
            .WithSummary("Adds a feature to a product")
            .Produces(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static void MapChangeProductPrice(RouteGroupBuilder group)
    {
        group.MapPut("/{productId:guid}/price", async (
            Guid productId,
            ChangeProductPriceRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new ChangeProductPriceCommand(productId, request.NewPrice.Amount, request.NewPrice.Currency);
            var result = await sender.Send(command, ct);
            if (result.IsSuccess)
                return Results.NoContent();
            return result.ToHttpResult();
        })
            .WithName("ChangeProductPrice")
            .WithSummary("Changes the price of a product")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static void MapChangeProductCategory(RouteGroupBuilder group)
    {
        group.MapPut("/{productId:guid}/category", async (
            Guid productId,
            ChangeProductCategoryRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new ChangeProductCategoryCommand(productId, request.NewCategoryId);
            var result = await sender.Send(command, ct);
            if (result.IsSuccess)
                return Results.NoContent();
            return result.ToHttpResult();
        })
            .WithName("ChangeProductCategory")
            .WithSummary("Changes the category of a product")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static void MapUpdateProductFeature(RouteGroupBuilder group)
    {
        group.MapPut("/{productId:guid}/features/{featureId:guid}", async (
            Guid productId,
            Guid featureId,
            UpdateProductFeatureRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateProductFeatureValueCommand(productId, featureId, request.NewValue);
            var result = await sender.Send(command, ct);
            if (result.IsSuccess)
                return Results.NoContent();
            return result.ToHttpResult();
        })
            .WithName("UpdateProductFeature")
            .WithSummary("Updates a feature of a product")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static void MapRemoveProductFeature(RouteGroupBuilder group)
    {
        group.MapDelete("/{productId:guid}/features/{featureId:guid}", async (
            Guid productId,
            Guid featureId,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new RemoveProductFeatureCommand(productId, featureId);
            var result = await sender.Send(command, ct);
            if (result.IsSuccess)
                return Results.NoContent();
            return result.ToHttpResult();
        })
            .WithName("RemoveProductFeature")
            .WithSummary("Removes a feature from a product")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }
}