using FluentAssertions;
using NSubstitute;
using ProductCatalog.Application.Catalog.CreateCategory;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Entities;
using ProductCatalog.Domain.Catalog.Errors;
using ProductCatalog.Domain.Catalog.Repositories;

namespace ProductCatalog.UnitTests.Application.Catalog;

public class CreateCategoryTests
{
    private readonly CreateCategoryCommandHandler _handler;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IProductCategoryRepository _productCategoryRepositoryMock;

    public CreateCategoryTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _productCategoryRepositoryMock = Substitute.For<IProductCategoryRepository>();

        _handler = new CreateCategoryCommandHandler(_productCategoryRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Name_Is_Invalid()
    {
        // Arrange
        var command = new CreateCategoryCommand("", "Test Description");

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidCategoryName);
        _productCategoryRepositoryMock.DidNotReceive().Add(Arg.Any<ProductCategory>());
        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_And_PersistCategory_When_Command_Is_Valid()
    {
        // Arrange
        var command = new CreateCategoryCommand("Electronics", "Electronic goods");

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _productCategoryRepositoryMock.Received(1).Add(Arg.Any<ProductCategory>());
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
