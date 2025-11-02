using SharedKernel.Common;

namespace Catalog.Domain.Common.Errors;

public static class ProductErrors
{
    public static readonly Error NotFound = new("Product.NotFound", "Product not found");
    public static readonly Error NotAvailable = new("Product.NotAvailable", "Product not available");
    public static readonly Error OutOfStock = new("Product.NotEnoughStock", "Out of stock");
    public static readonly Error InsufficientStock = new ("Product.InsufficientStock", "Insufficient stock quantity available");
    public static readonly Error NoPermission = new("Product.NoPermission", "No permission");
}