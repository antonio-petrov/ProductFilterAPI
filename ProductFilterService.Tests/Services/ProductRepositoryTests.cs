using Microsoft.Extensions.Logging;
using Moq;
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

        public ProductRepositoryTests()
        {
            _mockServer = WireMockServer.Start();
            HttpClient httpClient = new HttpClient { BaseAddress = new Uri(_mockServer.Urls[0]) };
            _mockLogger = new Mock<ILogger<ProductRepository>>();
            _productRepository = new ProductRepository(httpClient, _mockLogger.Object);
        }

        //[Fact]
        //public async Task GetAllProductsAsync_ReturnsProducts_WhenApiReturnsValidResponse()
        //{
        //    // Arrange
        //    string mockResponse = "[{\"id\":1,\"name\":\"Product 1\",\"price\":10,\"size\":\"Small\"}]";
        //    _mockServer
        //        .Given(Request.Create().WithPath("/v3/1b1e9d77-2b78-4085-a1a9-4f723f7af8e7").UsingGet())
        //        .RespondWith(Response.Create().WithStatusCode(200).WithBody(mockResponse));

        //    // Act
        //    List<Product> result = await _productRepository.GetAllProductsAsync();

        //    // Assert
        //    Assert.Single(result);
        //    Assert.Equal(1, result[0].Id);
        //    Assert.Equal("Product 1", result[0].Name);
        //}

        [Fact]
        public async Task GetAllProductsAsync_ThrowsException_WhenApiReturnsNonSuccessStatusCode()
        {
            // Arrange
            _mockServer
                .Given(Request.Create().WithPath("/v3/1b1e9d77-2b78-4085-a1a9-4f723f7af8e7").UsingGet())
                .RespondWith(Response.Create().WithStatusCode(500));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _productRepository.GetAllProductsAsync());
        }

        [Fact]
        public async Task GetAllProductsAsync_LogsResponse_WhenApiReturnsValidResponse()
        {
            // Arrange
            string mockResponse = "[{\"id\":1,\"name\":\"Product 1\",\"price\":10,\"size\":\"Small\"}]";
            _mockServer
                .Given(Request.Create().WithPath("/v3/1b1e9d77-2b78-4085-a1a9-4f723f7af8e7").UsingGet())
                .RespondWith(Response.Create().WithStatusCode(200).WithBody(mockResponse));

            // Act
            await _productRepository.GetAllProductsAsync();

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Mocky.io response: 200")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
                Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains($"Mocky.io response content: {mockResponse}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
                Times.Once);
        }

        public void Dispose()
        {
            _mockServer.Stop();
        }
    }
}
