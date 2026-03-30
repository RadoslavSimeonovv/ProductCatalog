using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Api.Common;
using ProductCatalog.Application.Catalog.CreateCategory;
using ProductCatalog.Application.Catalog.GetAllCategories;
using ProductCatalog.Application.Catalog.GetCategoryById;
using ProductCatalog.Application.Catalog.Responses;
using ProductCatalog.Application.Catalog.UpdateCategoryCommand;

namespace ProductCatalog.Api.Endpoints.Categories;

public static class CategoriesEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/categories")
            .WithTags("Categories");

        MapGetAllCategories(group);
        MapCreateCategory(group);
        MapGetCategoryById(group);
        MapUpdateCategory(group);

        return app;
    }
    private static void MapCreateCategory(RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            CreateCategoryRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateCategoryCommand(request.Name, request.Description);
            var result = await sender.Send(command, ct);
            if (result.IsSuccess)
                return Results.Created($"/products/categories/{result.Value}", null);
            return result.ToHttpResult();
        })
            .WithName("CreateProductCategory")
            .WithSummary("Creates a new product category")
            .Produces(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policies.AdminOnly);
    }

    private static void MapUpdateCategory(RouteGroupBuilder group)
    {
        group.MapPut("/{categoryId:guid}", async (
            Guid categoryId,
            UpdateCategoryRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateCategoryCommand(categoryId, request.Name, request.Description);
            var result = await sender.Send(command, ct);
            if (result.IsSuccess)
                return Results.NoContent();
            return result.ToHttpResult();
        })
            .WithName("UpdateProductCategory")
            .WithSummary("Updates a product category")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policies.AdminOnly);
    }

    private static void MapGetAllCategories(RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetAllCategoriesQuery(), ct);
            return result.ToHttpResult();
        })
            .WithName("GetAllCategories")
            .WithSummary("Gets all categories")
            .Produces<IReadOnlyList<ProductCategoryResponse>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static void MapGetCategoryById(RouteGroupBuilder group)
    {
        group.MapGet("/{categoryId:guid}", async (
            Guid categoryId,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetCategoryByIdQuery(categoryId), ct);
            return result.ToHttpResult();
        })
            .WithName("GetCategoryById")
            .WithSummary("Gets a category by id")
            .Produces<ProductCategoryResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }
}
