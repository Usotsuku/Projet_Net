using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ElectroShop.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string? ImageUrl { get; set; }
        [DataType(DataType.Currency)]
        public float Price { get; set; }
        public string? Description { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public List<CartItem> CartItems { get; set; }

        [NotMapped]
        public string CategoryName { get; set; }
    }
}
