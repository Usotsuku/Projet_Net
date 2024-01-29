namespace ElectroShop.Models.DTOs
{
    public class ProductDisplayModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public string Sterm { get; set; } = "";
        public int CategoryId { get; set; } = 0;
    }
}
