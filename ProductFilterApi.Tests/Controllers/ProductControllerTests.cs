using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProductFilterApi.Controllers;
using ProductFilterService.Interfaces;
using ProductFilterService.Models;

namespace ProductFilterApi.Tests.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly Mock<ILogger<ProductController>> _mockLogger;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _mockLogger = new Mock<ILogger<ProductController>>();
            _controller = new ProductController(_mockProductService.Object, _mockLogger.Object);
        }

        //[Fact]
        //public async Task FilterProducts_ReturnsOkResult_WhenServiceReturnsData()
        //{
        //    // Arrange
        //    FilterParameters filterParams = new FilterParameters();
        //    FilterResult expectedResult = new FilterResult
        //    {
        //        Products = new List<Product> { new Product { Id = 1, Name = "Test Product" } },
        //        Filter = new FilterObject()
        //    };
        //    _mockProductService.Setup(s => s.FilterProductsAsync(It.IsAny<FilterParameters>()))
        //        .ReturnsAsync(expectedResult);

        //    // Act
        //    IActionResult result = await _controller.FilterProducts(filterParams);

        //    // Assert
        //    OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        //    Assert.Equal(expectedResult, okResult.Value);
        //}

        [Fact]
        public async Task FilterProducts_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            FilterParameters filterParams = new FilterParameters();
            _mockProductService.Setup(s => s.FilterProductsAsync(It.IsAny<FilterParameters>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            IActionResult result = await _controller.FilterProducts(filterParams);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("An error occurred while processing your request.", objectResult.Value);
        }

        [Fact]
        public async Task FilterProducts_LogsError_WhenExceptionOccurs()
        {
            // Arrange
            FilterParameters filterParams = new FilterParameters();
            _mockProductService.Setup(s => s.FilterProductsAsync(It.IsAny<FilterParameters>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            await _controller.FilterProducts(filterParams);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals("Error occurred while filtering products", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
                Times.Once);
        }
    }


}
