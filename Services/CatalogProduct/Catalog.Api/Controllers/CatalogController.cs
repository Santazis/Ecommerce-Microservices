using Catalog.Application.Interfaces;
using Catalog.Application.Models.Catalogs;
using Catalog.Application.Models.Requests.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/catalog")]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogService _catalogService;

        public CatalogController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateCatalogRequest request,
            CancellationToken cancellation)
        {
            var result = await _catalogService.CreateAsync(request,cancellation);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Created();
        }
        [HttpGet("{id:guid}")]
        [ProducesResponseType<CatalogDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(Guid id,CancellationToken cancellation)
        {
            var result = await _catalogService.GetByIdAsync(id,cancellation);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }
            return Ok(result.Value);
        }

        [HttpGet("menu")]
        public async Task<IActionResult> GetCatalogsMenuAsync(CancellationToken cancellation)
        {
            var result = await _catalogService.GetCatalogsMenuAsync(cancellation);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }
            return Ok(result.Value);  
        }
    }
}