using ProductFilterService.Models;

namespace ProductFilterService.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync();
    }
}
