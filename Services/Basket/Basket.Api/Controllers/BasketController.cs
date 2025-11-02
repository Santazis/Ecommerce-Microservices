using Basket.Application.Basket;
using Microsoft.AspNetCore.Mvc;

namespace Basket.Api.Controllers;
[ApiController]
[Route("api/basket")]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;

    public BasketController(IBasketService basketService)
    {
        _basketService = basketService;
    }

    [HttpPost("{basketId:guid}")]
    public async Task<IActionResult> AddProductAsync([FromBody] AddProductRequest request,[FromRoute] Guid basketId,
        CancellationToken cancellation)
    {
        var result = await _basketService.AddProductToBasketAsync(basketId,request,cancellation);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        return Ok();
    }
    
    [HttpGet("{basketId:guid}")]
    public async Task<IActionResult> GetBasketAsync([FromRoute] Guid basketId,CancellationToken cancellation)
    {
        var result = await _basketService.GetBasketAsync(basketId,cancellation);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }

    [HttpDelete("{basketId:guid}")]
    public async Task<IActionResult> DeleteProductAsync([FromRoute] Guid basketId, [FromBody] DeleteBasketItemRequest request,
        CancellationToken cancellation)
    {
       var result = await _basketService.DeleteProduct(basketId,request,cancellation);
       if (result.IsFailure)
       {
           return BadRequest(result.Error);
       }
       return NoContent();
    }
}