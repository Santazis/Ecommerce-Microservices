namespace Basket.Application.Basket;

public record BasketItemDto(
    Guid ProductId,
    string Name,
    int Quantity,
    int AvailableQty,
    decimal Price,
    string Currency,
    string ImageUrl,
    bool IsAvailable);