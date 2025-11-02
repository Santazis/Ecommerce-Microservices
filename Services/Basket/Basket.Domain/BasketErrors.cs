
using SharedKernel.Common;

namespace Basket.Domain;

public sealed class BasketErrors
{
    public static readonly Error AlreadyExists = new("Basket.Exists","Basket already exists");
    public static readonly Error NotFound = new("Basket.NotFound","Basket not found");
}