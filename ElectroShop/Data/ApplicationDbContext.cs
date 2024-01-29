using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ElectroShop.Models;
namespace ElectroShop.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<ElectroShop.Models.Product> Products { get; set; } = default!;
        public DbSet<ElectroShop.Models.Category> Categories { get; set; } = default!;
        public DbSet<ElectroShop.Models.Cart> Carts { get; set; } = default!;
        public DbSet<ElectroShop.Models.CartItem> CartItems { get; set; } = default!;
        public DbSet<ElectroShop.Models.Order> Orders { get; set; } = default!;
        public DbSet<ElectroShop.Models.OrderDetail> OrderDetails { get; set; } = default!;
        public DbSet<ElectroShop.Models.OrderStatus> OrderStatuses { get; set; } = default!;

    }
}
