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
        private readonly IAppConfig _appConfig;

        public ProductRepository(HttpClient httpClient, ILogger<ProductRepository> logger, IAppConfig appConfig)
        {
            _httpClient = httpClient;
            _logger = logger;
            _appConfig = appConfig;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_appConfig.DatabaseUrl);

                _logger.LogInformation("Mocky.io response: {StatusCode}", response.StatusCode);

                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Mocky.io response content: {Content}", content);

                return JsonSerializer.Deserialize<List<Product>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error occurred while fetching products");
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error occurred while deserializing products");
                throw;
            }
        }
    }
}
