using Catalog.Application.Interfaces;
using Catalog.Application.Models.Requests.ProductImage;
using Catalog.Database;
using Catalog.Domain.Common.Errors;
using Catalog.Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common;

namespace Catalog.Application.Services;

public class ProductImageService : IProductImageService
{
    private readonly ApplicationDbContext _dbContext;

    public ProductImageService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Dictionary<Guid, string>>> CreateImagesAsync(Guid productId, IEnumerable<ProcessImageRequest> requests,
        CancellationToken cancellation)
    {
        
        var product = await _dbContext.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId, cancellation);
        if (product is null)
        {
            return Result<Dictionary<Guid, string>>.Failure(ProductErrors.NotFound);
        }
        var result = new Dictionary<Guid, string>();
        foreach (var image in requests)
        {
          var id =  product.AddImage(image.TempUrl, image.SortOrder);
          result.Add(id,image.TempUrl);
        }
        
        await _dbContext.SaveChangesAsync(cancellation);
        return Result<Dictionary<Guid, string>>.Success(result);
    }

    public async Task<Result> UpdateImagesAsync(Guid productId, IEnumerable<UpdateImageRequest> requests,
        CancellationToken cancellation)
    {
        var product = await _dbContext.Products
            .Include(p=> p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId, cancellation);
        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound);
        }

        foreach (var updateImageRequest in requests)
        {
            product.UpdateImage(updateImageRequest.Id, updateImageRequest.Url, updateImageRequest.SortOrder);
        }

        await _dbContext.SaveChangesAsync(cancellation);
        return Result.Success;
    }
}