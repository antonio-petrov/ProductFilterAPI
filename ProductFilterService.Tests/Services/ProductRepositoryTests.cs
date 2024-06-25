using Microsoft.Extensions.Logging;
using Moq;
using ProductFilterService.Constants;
using ProductFilterService.Interfaces;
using ProductFilterService.Models;
using ProductFilterService.Services;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ProductFilterService.Tests.Services
{
    public class ProductRepositoryTests : IDisposable
    {
        private readonly WireMockServer _mockServer;
        private readonly ProductRepository _productRepository;
        private readonly Mock<ILogger<ProductRepository>> _mockLogger;
        private readonly Mock<IAppConfig> _mockConfig;
        private readonly string _apiPath;

        public ProductRepositoryTests()
        {
            _mockServer = WireMockServer.Start();

            HttpClient httpClient = new HttpClient { BaseAddress = new Uri(_mockServer.Urls[0]) };
            _mockLogger = new Mock<ILogger<ProductRepository>>();
            _mockConfig = new Mock<IAppConfig>();
            _mockConfig.Setup(config => config.DatabaseUrl).Returns(_mockServer.Urls[0] + "/api/products");

            _productRepository = new ProductRepository(httpClient, _mockLogger.Object, _mockConfig.Object);
            _apiPath = new Uri(DatabaseConstants.DATABASE_URL).PathAndQuery;
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsProducts_WhenApiReturnsValidResponse()
        {
            // Arrange
            string mockResponse = "[{\"title\":\"Product 1\",\"price\":10,\"sizes\":[\"Small\"],\"description\":\"Description 1\"}]";
            _mockServer
                .Given(Request.Create().WithPath("/api/products").UsingGet())
                .RespondWith(Response.Create().WithStatusCode(200).WithBody(mockResponse));

            // Act
            List<Product> result = await _productRepository.GetAllProductsAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Product 1", result[0].Title);
            Assert.Equal(10, result[0].Price);
            Assert.Single(result[0].Sizes);
            Assert.Equal("Small", result[0].Sizes[0]);
            Assert.Equal("Description 1", result[0].Description);
        }


        [Fact]
        public async Task GetAllProductsAsync_ThrowsException_WhenApiReturnsNonSuccessStatusCode()
        {
            // Arrange
            _mockServer
                .Given(Request.Create().WithPath("/api/products").UsingGet())
                .RespondWith(Response.Create().WithStatusCode(500));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _productRepository.GetAllProductsAsync());
        }

        [Fact]
        public async Task GetAllProductsAsync_LogsError_WhenApiReturnsNonSuccessStatusCode()
        {
            // Arrange
            _mockServer
                .Given(Request.Create().WithPath("/api/products").UsingGet())
                .RespondWith(Response.Create().WithStatusCode(500));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _productRepository.GetAllProductsAsync());

            // Verify logging
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Error occurred while fetching products")),
                    It.IsAny<HttpRequestException>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
                Times.Once);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsEmptyList_WhenApiReturnsEmptyArray()
        {
            // Arrange
            string mockResponse = "[]";
            _mockServer
                .Given(Request.Create().WithPath("/api/products").UsingGet())
                .RespondWith(Response.Create().WithStatusCode(200).WithBody(mockResponse));

            // Act
            List<Product> result = await _productRepository.GetAllProductsAsync();

            // Assert
            Assert.Empty(result);
        }

        public void Dispose()
        {
            _mockServer.Stop();
        }
    }
}
