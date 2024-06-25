using ProductFilterService.Interfaces;
using ProductFilterService.Models;
using System.Text.RegularExpressions;

namespace ProductFilterService.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<FilterResult> FilterProductsAsync(FilterParameters filterParams)
        {
            List<Product> allProducts = await _productRepository.GetAllProductsAsync();

            // Apply filters
            IEnumerable<Product> filteredProducts = allProducts;

            if (filterParams.MinPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price >= filterParams.MinPrice.Value);
            }

            if (filterParams.MaxPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price <= filterParams.MaxPrice.Value);
            }

            if (!string.IsNullOrWhiteSpace(filterParams.Size))
            {
                filteredProducts = filteredProducts.Where(p => p.Sizes.Any(s => string.Equals(s, filterParams.Size, StringComparison.OrdinalIgnoreCase))).ToList();
            }

            // Highlight words in description
            List<string> highlightWords = filterParams.Highlight?.Split(',').Select(w => w.Trim()).ToList() ?? new List<string>();
            foreach (Product product in filteredProducts)
            {
                product.Description = HighlightWords(product.Description, highlightWords);
            }

            // Create filter object
            FilterObject filterObject = CreateFilterObject(allProducts);

            return new FilterResult
            {
                Products = filteredProducts.ToList(),
                Filter = filterObject
            };
        }

        private string HighlightWords(string text, List<string> wordsToHighlight)
        {
            if (string.IsNullOrWhiteSpace(text) || !wordsToHighlight.Any())
            {
                return text;
            }

            foreach (string word in wordsToHighlight)
            {
                text = Regex.Replace(text, $@"\b{Regex.Escape(word)}\b", $"<em>{word}</em>", RegexOptions.IgnoreCase);
            }

            return text;
        }

        private FilterObject CreateFilterObject(List<Product> products)
        {
            if (!products.Any())
            {
                return new FilterObject
                {
                    MinPrice = 0,
                    MaxPrice = 0,
                    Sizes = new List<string>(),
                    CommonWords = new List<string>()
                };
            }

            decimal minPrice = products.Min(p => p.Price);
            decimal maxPrice = products.Max(p => p.Price);
            List<string> sizes = products.SelectMany(p => p.Sizes).Distinct().ToList();

            List<string> commonWords = products
                .SelectMany(p => p.Description.Split(new[] { ' ', ',', '.', '!', '?', ';', ':', '-', '(', ')', '[', ']', '{', '}' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(word => new string(word.Where(c => !char.IsPunctuation(c)).ToArray()).ToLower())
                .Where(word => !string.IsNullOrWhiteSpace(word) && word.Length > 1)
                .GroupBy(word => word)
                .OrderByDescending(g => g.Count())
                .Skip(5)  // Skip the 5 most common words
                .Take(10) // Take the next 10 most common words
                .Select(g => g.Key)
                .ToList();

            return new FilterObject
            {
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Sizes = sizes,
                CommonWords = commonWords
            };
        }
    }
}
