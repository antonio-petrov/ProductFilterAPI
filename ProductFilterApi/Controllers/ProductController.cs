using Microsoft.AspNetCore.Mvc;
using ProductFilterService.Interfaces;
using ProductFilterService.Models;

namespace ProductFilterApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Filters and retrieves products based on specified criteria.
        /// </summary>
        /// <param name="filterParams">The filter parameters to apply.</param>
        /// <returns>A filtered list of products and associated filter metadata.</returns>
        /// <response code="200">Returns the filtered products and filter object.</response>
        /// <response code="400">If the filter parameters are invalid.</response>
        /// <response code="500">If there was an internal server error during processing.</response>
        [HttpGet("filter")]
        [ProducesResponseType(typeof(FilterResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FilterProducts([FromQuery] FilterParameters filterParams)
        {
            try
            {
                FilterResult result = await _productService.FilterProductsAsync(filterParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while filtering products");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
