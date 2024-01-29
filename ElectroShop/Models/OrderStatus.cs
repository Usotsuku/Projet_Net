using System.ComponentModel.DataAnnotations;

namespace ElectroShop.Models
{
    public class OrderStatus
    {
        public int Id { get; set; }
        [Required]
        public int StatusId { get; set; }
        [Required]
        public string? Name { get; set; }

    }
}
