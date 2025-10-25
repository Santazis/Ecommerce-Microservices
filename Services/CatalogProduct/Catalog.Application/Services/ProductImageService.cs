using Catalog.Application.Interfaces;
using Catalog.Application.Models.Requests.ProductImage;
using Catalog.Database;
using Catalog.Domain.Common.Errors;
using Catalog.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Catalog.Application.Services;

public class ProductImageService : IProductImageService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ProductImageService> _logger;

    public ProductImageService(ApplicationDbContext dbContext, ILogger<ProductImageService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Dictionary<Guid, string>>> CreateImagesAsync(Guid productId,
        IEnumerable<ProcessImageRequest> requests,
        CancellationToken cancellation)
    {
        _logger.LogInformation("Creating {imageCount} images for product {productId}", requests.Count(), productId);
        var product = await _dbContext.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId, cancellation);
        if (product is null)
        {
            _logger.LogError("Product {productId} not found", productId);
            return Result<Dictionary<Guid, string>>.Failure(ProductErrors.NotFound);
        }

        var result = new Dictionary<Guid, string>();
        foreach (var image in requests)
        {
            var id = product.AddImage(image.TempUrl, image.SortOrder);
            result.Add(id, image.TempUrl);
        }

        await _dbContext.SaveChangesAsync(cancellation);
        _logger.LogInformation("Images created for product {productId}", productId);
        return Result<Dictionary<Guid, string>>.Success(result);
    }

    public async Task<Result> UpdateImagesAsync(Guid productId, IEnumerable<UpdateImageRequest> requests,
        CancellationToken cancellation)
    {
        _logger.LogInformation("Updating {imageCount} images for product {productId}", requests.Count(), productId);
        var product = await _dbContext.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId, cancellation);
        if (product is null)
        {
            _logger.LogError("Product {productId} not found", productId);
            return Result.Failure(ProductErrors.NotFound);
        }

        foreach (var updateImageRequest in requests)
        {
            product.UpdateImage(updateImageRequest.Id, updateImageRequest.Url, updateImageRequest.SortOrder);
        }

        await _dbContext.SaveChangesAsync(cancellation);
        _logger.LogInformation("Images updated for product {productId}", productId);
        return Result.Success;
    }
}