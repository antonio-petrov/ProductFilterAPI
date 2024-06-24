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

        [HttpGet("filter")]
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
