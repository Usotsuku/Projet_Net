using ElectroShop.Data;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace ElectroShop.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _context;
        public HomeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> Categories()
        {
            return await _context.Categories.ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetProducts(string sterm="",int categoryId = 0)
        {
            sterm = sterm.ToLower();
            IEnumerable<Product> products = await (from product in _context.Products
                            join category in _context.Categories
                            on product.CategoryId equals category.Id
            where string.IsNullOrWhiteSpace(sterm) || (product != null && product.Name.ToLower().StartsWith(sterm))
            select new Product
            {
                                Id = product.Id,
                                ImageUrl = product.ImageUrl,
                                Name = product.Name,
                                Price = product.Price,
                                CategoryId = product.CategoryId,
                                Description = product.Description,
                                CategoryName = category.Name,
                            }
                            ).ToListAsync();

            if (categoryId > 0)
            {
                products = products.Where(a => a.CategoryId == categoryId).ToList();
            }


            return products;
        }
    }
}
