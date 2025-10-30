using SharedKernel.Common;

namespace Catalog.Domain.Common.Errors;

public static class ProductErrors
{
    public static readonly Error NotFound = new("Product.NotFound", "Product not found");
    public static readonly Error NotAvailable = new("Product.NotAvailable", "Product not available");
    public static readonly Error NotEnoughStock = new("Product.NotEnoughStock", "Product not enough stock");
    public static readonly Error NoPermission = new("Product.NoPermission", "No permission");
}