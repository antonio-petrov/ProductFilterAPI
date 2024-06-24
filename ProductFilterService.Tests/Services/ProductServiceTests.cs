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
            new Product { Id = 1, Name = "Product 1", Price = 10, Size = "Small" },
            new Product { Id = 2, Name = "Product 2", Price = 20, Size = "Medium" }
        };
            _mockRepository.Setup(r => r.GetAllProductsAsync()).ReturnsAsync(mockProducts);

            FilterParameters filterParams = new FilterParameters();

            // Act
            FilterResult result = await _productService.FilterProductsAsync(filterParams);

            // Assert
            Assert.Equal(mockProducts.Count, result.Products.Count);
        }

        [Fact]
        public async Task FilterProductsAsync_ReturnsFilteredProducts_WhenPriceFilterApplied()
        {
            // Arrange
            List<Product> mockProducts = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", Price = 10, Size = "Small" },
            new Product { Id = 2, Name = "Product 2", Price = 20, Size = "Medium" },
            new Product { Id = 3, Name = "Product 3", Price = 30, Size = "Large" }
        };
            _mockRepository.Setup(r => r.GetAllProductsAsync()).ReturnsAsync(mockProducts);

            FilterParameters filterParams = new FilterParameters { MinPrice = 15, MaxPrice = 25 };

            // Act
            FilterResult result = await _productService.FilterProductsAsync(filterParams);

            // Assert
            Assert.Single(result.Products);
            Assert.Equal(2, result.Products[0].Id);
        }

        [Fact]
        public async Task FilterProductsAsync_ReturnsFilteredProducts_WhenSizeFilterApplied()
        {
            // Arrange
            List<Product> mockProducts = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", Price = 10, Size = "Small" },
            new Product { Id = 2, Name = "Product 2", Price = 20, Size = "Medium" },
            new Product { Id = 3, Name = "Product 3", Price = 30, Size = "Large" }
        };
            _mockRepository.Setup(r => r.GetAllProductsAsync()).ReturnsAsync(mockProducts);

            FilterParameters filterParams = new FilterParameters { Size = "Medium" };

            // Act
            FilterResult result = await _productService.FilterProductsAsync(filterParams);

            // Assert
            Assert.Single(result.Products);
            Assert.Equal(2, result.Products[0].Id);
        }

        [Fact]
        public async Task FilterProductsAsync_HighlightsWords_WhenHighlightParameterProvided()
        {
            // Arrange
            List<Product> mockProducts = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", Price = 10, Size = "Small", Description = "This is a blue product" },
            new Product { Id = 2, Name = "Product 2", Price = 20, Size = "Medium", Description = "This is a green product" }
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
                new Product { Id = 1, Name = "Product 1", Price = 10, Size = "Small", Description = "This is a blue product" },
                new Product { Id = 2, Name = "Product 2", Price = 20, Size = "Medium", Description = "This is a green product" },
                new Product { Id = 3, Name = "Product 3", Price = 30, Size = "Large", Description = "This is a red product" }
            };
            _mockRepository.Setup(r => r.GetAllProductsAsync()).ReturnsAsync(mockProducts);

            FilterParameters filterParams = new FilterParameters();

            // Act
            FilterResult result = await _productService.FilterProductsAsync(filterParams);

            // Assert
            Assert.Equal(10, result.Filter.MinPrice);
            Assert.Equal(30, result.Filter.MaxPrice);
            Assert.Equal(new[] { "Small", "Medium", "Large" }, result.Filter.Sizes);
            Assert.Equal(new[] { "product", "is", "a", "blue", "green", "red" }, result.Filter.CommonWords);
        }
    }
}
