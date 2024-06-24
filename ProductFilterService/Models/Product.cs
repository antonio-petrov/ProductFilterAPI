namespace ProductFilterService.Models
{
    public class Product
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
        public List<string> Sizes { get; set; }
        public string Description { get; set; }
    }
}
