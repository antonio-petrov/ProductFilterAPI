using ProductFilterService.Models;

namespace ProductFilterService.Interfaces
{
    public interface IProductService
    {
        Task<FilterResult> FilterProductsAsync(FilterParameters filterParams);
    }
}
