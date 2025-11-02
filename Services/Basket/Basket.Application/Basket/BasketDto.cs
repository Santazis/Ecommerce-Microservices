namespace Basket.Application.Basket;

public record BasketDto(
    Guid Id,
    List<BasketItemDto> Items,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    decimal TotalAmount,
    int TotaPositions,int TotalItems);