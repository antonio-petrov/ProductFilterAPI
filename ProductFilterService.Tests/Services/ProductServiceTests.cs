using Moq;
using ProductFilterService.Interfaces;
using ProductFilterService.Models;
using ProductFilterService.Services;

namespace ProductFilterService.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _productService = new ProductService(_mockRepository.Object);
        }

        [Fact]
        public async Task FilterProductsAsync_ReturnsAllProducts_WhenNoFiltersApplied()
        {
            // Arrange
            List<Product> mockProducts = new List<Product>
            {
                new Product { Title = "Product 1", Price = 10, Sizes = new List<string> { "Small" }, Description = "Description 1" },
                new Product { Title = "Product 2", Price = 20, Sizes = new List<string> { "Medium" }, Description = "Description 2" }
            };
            _mockRepository.Setup(r => r.GetAllProductsAsync()).ReturnsAsync(mockProducts);

            FilterParameters filterParams = new FilterParameters();

            // Act
            FilterResult result = await _productService.FilterProductsAsync(filterParams);

            // Assert
            Assert.Equal(mockProducts.Count, result.Products.Count);
            //    Assert.Single(result.Products);
            //    Assert.Equal(2, result.Products[0].Id);
        }

        [Fact]
        public async Task FilterProductsAsync_ReturnsFilteredProducts_WhenPriceFilterApplied()
        {
            // Arrange
            List<Product> mockProducts = new List<Product>
            {
                new Product { Title = "Product 1", Price = 10, Sizes = new List<string> { "Small" }, Description = "Description 1" },
                new Product { Title = "Product 2", Price = 20, Sizes = new List<string> { "Medium" }, Description = "Description 2" },
                new Product { Title = "Product 3", Price = 30, Sizes = new List<string> { "Large" }, Description = "Description 3" }
            };
            _mockRepository.Setup(r => r.GetAllProductsAsync()).ReturnsAsync(mockProducts);

            FilterParameters filterParams = new FilterParameters { MinPrice = 15, MaxPrice = 25 };

            // Act
            FilterResult result = await _productService.FilterProductsAsync(filterParams);

            // Assert
            Assert.Single(result.Products);
            Assert.Equal("Product 2", result.Products[0].Title);
        }
        [Fact]
        public async Task FilterProductsAsync_ReturnsFilteredProducts_WhenSizeFilterApplied()
        {
            // Arrange
            List<Product> mockProducts = new List<Product>
            {
                new Product { Title = "Product 1", Price = 10, Sizes = new List<string> { "Small" }, Description = "Description 1" },
                new Product { Title = "Product 2", Price = 20, Sizes = new List<string> { "Medium" }, Description = "Description 2" },
                new Product { Title = "Product 3", Price = 30, Sizes = new List<string> { "Large" }, Description = "Description 3" }
            };
            _mockRepository.Setup(r => r.GetAllProductsAsync()).ReturnsAsync(mockProducts);

            FilterParameters filterParams = new FilterParameters { Size = "Medium" };

            // Act
            FilterResult result = await _productService.FilterProductsAsync(filterParams);

            // Assert
            Assert.Single(result.Products);
            Assert.Equal("Product 2", result.Products[0].Title);
        }

        [Fact]
        public async Task FilterProductsAsync_HighlightsWords_WhenHighlightParameterProvided()
        {
            // Arrange
            List<Product> mockProducts = new List<Product>
            {
                new Product { Title = "Product 1", Price = 10, Sizes = new List<string> { "Small" }, Description = "This is a blue product" },
                new Product { Title = "Product 2", Price = 20, Sizes = new List<string> { "Medium" }, Description = "This is a green product" }
            };
            _mockRepository.Setup(r => r.GetAllProductsAsync()).ReturnsAsync(mockProducts);

            FilterParameters filterParams = new FilterParameters { Highlight = "blue,green" };

            // Act
            FilterResult result = await _productService.FilterProductsAsync(filterParams);

            // Assert
            Assert.Equal("This is a <em>blue</em> product", result.Products[0].Description);
            Assert.Equal("This is a <em>green</em> product", result.Products[1].Description);
        }
        [Fact]
        public async Task FilterProductsAsync_ReturnsCorrectFilterObject()
        {
            // Arrange
            List<Product> mockProducts = new List<Product>
            {
                new Product { Title = "Product 1", Price = 10, Sizes = new List<string> { "Small" }, Description = "This is a blue product" },
                new Product { Title = "Product 2", Price = 20, Sizes = new List<string> { "Medium" }, Description = "This is a green product" },
                new Product { Title = "Product 3", Price = 30, Sizes = new List<string> { "Large" }, Description = "This is a red product" }
            };
            _mockRepository.Setup(r => r.GetAllProductsAsync()).ReturnsAsync(mockProducts);

            FilterParameters filterParams = new FilterParameters();

            // Act
            FilterResult result = await _productService.FilterProductsAsync(filterParams);

            // Assert
            Assert.Equal(10, result.Filter.MinPrice);
            Assert.Equal(30, result.Filter.MaxPrice);
            Assert.Equal(new[] { "Small", "Medium", "Large" }, result.Filter.Sizes);

            Assert.Single(result.Filter.CommonWords);
            Assert.Contains("red", result.Filter.CommonWords);
        }
    }
}
