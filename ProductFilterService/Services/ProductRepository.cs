using Microsoft.Extensions.Logging;
using ProductFilterService.Constants;
using ProductFilterService.Interfaces;
using ProductFilterService.Models;
using System.Text.Json;

namespace ProductFilterService.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(HttpClient httpClient, ILogger<ProductRepository> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(DatabaseConstants.DATABASE_URL);

            _logger.LogInformation("Mocky.io response: {StatusCode}", response.StatusCode);

            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Mocky.io response content: {Content}", content);

            return JsonSerializer.Deserialize<List<Product>>(content);
        }
    }
}
