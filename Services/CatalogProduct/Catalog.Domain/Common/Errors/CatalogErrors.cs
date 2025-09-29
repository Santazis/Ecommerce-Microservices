using SharedKernel.Common;

namespace Catalog.Domain.Common.Errors;

public static class CatalogErrors
{
    public static readonly Error NotFound = new("Catalog.NotFound", "Catalog not found");
}