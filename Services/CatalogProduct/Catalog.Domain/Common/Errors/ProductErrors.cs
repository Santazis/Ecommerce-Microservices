using SharedKernel.Common;

namespace Catalog.Domain.Common.Errors;

public static class ProductErrors
{
    public static readonly Error NotFound = new("Product.NotFound", "Product not found");
}