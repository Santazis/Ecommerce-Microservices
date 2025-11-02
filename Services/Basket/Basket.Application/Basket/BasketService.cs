using Basket.Application.Interfaces;
using Basket.Database;
using Basket.Domain;
using Basket.Domain.Baskets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Basket.Application.Basket;

public class BasketService : IBasketService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<BasketService> _logger;
    private readonly IProductServiceClient _productServiceClient;

    public BasketService(ApplicationDbContext dbContext, ILogger<BasketService> logger,
        IProductServiceClient productServiceClient)
    {
        _dbContext = dbContext;
        _logger = logger;
        _productServiceClient = productServiceClient;
    }
    

    public async Task<Result> DeleteProduct(Guid basketId, DeleteBasketItemRequest request, CancellationToken cancellation)
    {
        var basket = await _dbContext.Baskets.Include(b=> b.Items).FirstOrDefaultAsync(b => b.Id == basketId, cancellation);
        if (basket == null)
        {
            _logger.LogInformation("Basket {basketId} not found", basketId);
            return Result.Failure(BasketErrors.NotFound);
        }
        basket.RemoveItem(request.ProductId);
        await _dbContext.SaveChangesAsync(cancellation);
        _logger.LogInformation("Product {productId} was deleted from basket {basketId}", request.ProductId, basketId);
        return Result.Success;
    }

    public async Task<Result> CreateBasketAsync(Guid userId, CancellationToken cancellation)
    {
        var isExists = await _dbContext.Baskets.AnyAsync(b => b.UserId == userId, cancellation);
        if (isExists)
        {
            _logger.LogError("Basket already exists for user {userId}", userId);
            return Result.Failure(BasketErrors.AlreadyExists);
        }

        var basket = Domain.Baskets.Basket.Create(userId);
        await _dbContext.Baskets.AddAsync(basket, cancellation);
        await _dbContext.SaveChangesAsync(cancellation);
        _logger.LogInformation("Basket created for user {userId}", userId);
        return Result.Success;
    }

    public async Task<Result> DeleteProductFromBasketsAsync(Guid productId, CancellationToken cancellation)
    {
        var result = await _dbContext.BasketItems.Where(i => i.ProductId == productId)
            .ExecuteDeleteAsync(cancellation);
        if (result == 0)
        {
            _logger.LogWarning("Product {productId} not found in basket", productId);
        }

        _logger.LogInformation("Product was deleted from {result} baskets", result);
        return Result.Success;
    }

    public async Task<Result> AddProductToBasketAsync(Guid basketId, AddProductRequest request,
        CancellationToken cancellation)
    {
        var basket = await _dbContext.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.Id == basketId, cancellation);
        if (basket == null)
        {
            _logger.LogInformation("Basket {basketId} not found", basketId);
            return Result.Failure(BasketErrors.NotFound);
        }

        var isAvailable =
            await _productServiceClient.IsProductAvailableAsync(request.ProductId, request.Quantity, cancellation);
        if (isAvailable.IsFailure)
        {
            _logger.LogInformation("Product {productId} not available", request.ProductId);
            return Result.Failure(isAvailable.Error);
        }

        basket.AddItem(request.ProductId, request.Quantity);
        await _dbContext.SaveChangesAsync(cancellation);
        _logger.LogInformation("{Quantity} product {ProductId} successfully added to basket {BasketId}",
            request.Quantity, request.ProductId, basketId);
        return Result.Success;
    }

    public async Task<Result<BasketDto>> GetBasketAsync(Guid basketId, CancellationToken cancellation)
    {
        _logger.LogInformation("Getting basket {basketId}", basketId);
        var basket = await _dbContext.Baskets.AsNoTracking()
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.Id == basketId, cancellation);
        if (basket is null)
        {
            return Result<BasketDto>.Failure(BasketErrors.NotFound);
        }

        var productsRequest = basket.Items.ToDictionary(i => i.ProductId, i => i.Quantity);
        _logger.LogInformation("Getting {productsCount} products for basket {basketId}", productsRequest.Count,
            basketId);
        var productsResponse = await _productServiceClient.GetProductsByIdsAsync(productsRequest, cancellation);

        var basketItems = productsResponse.ToList();
        var totalAmount = basketItems.Sum(i => i.Price * i.Quantity);
        var totalItems = basketItems.Sum(i => i.Quantity);
        return Result<BasketDto>.Success(new BasketDto(basketId, basketItems, basket.CreatedAt, basket.UpdatedAt,
            totalAmount, basketItems.Count, totalItems));
    }
}