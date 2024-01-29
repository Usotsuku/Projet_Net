using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ElectroShop.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartRepository(ApplicationDbContext context,UserManager<IdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _context = context;
            _httpContextAccessor = httpContextAccessor;

        }
        public async Task<int> AddItem(int productId, int qty)
        {
            string userId = GetUserId();
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("user is not logged-in");
                var cart = await GetCart(userId);
                if (cart is null)
                {
                    cart = new Cart
                    {
                        UserId = userId
                    };
                    _context.Carts.Add(cart);
                }
                _context.SaveChanges();
                // cart detail section
                var cartItem = _context.CartItems
                                  .FirstOrDefault(a => a.CartId == cart.Id && a.ProductId == productId);
                if (cartItem is not null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    var product = _context.Products.Find(productId);
                    cartItem = new CartItem
                    {
                        ProductId = productId,
                        CartId = cart.Id,
                        Quantity = qty,
                        UnitPrice = product.Price  // it is a new line after update
                    };
                    _context.CartItems.Add(cartItem);
                }
                _context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        public async Task<int> RemoveItem(int productId)
        {
            string userId = GetUserId();
            //using var transaction = _context.Database.BeginTransaction();
            try
            {
                
                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception("user not logged in");
                }
                var cart = await GetCart(userId);
                if (cart is null)
                {
                    throw new Exception("Invalid cart");
                }
                var cartItem = _context.CartItems.FirstOrDefault(a => a.CartId == cart.Id && a.ProductId == productId);
                if( cartItem is null)
                {
                    throw new Exception("No items in the cart");
                }
                else if(cartItem.Quantity == 1)
                {
                    _context.CartItems.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity = cartItem.Quantity - 1;
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }
        public async Task<Cart> GetUserCart()
        {
            var userId = GetUserId();
            if(userId == null)
            {
                throw new Exception("Invalid useid");
            }
            var cart = await _context.Carts
                        .Include(a => a.CartItems)
                        .ThenInclude(a => a.Product)
                        .ThenInclude(a => a.Category)
                        .Where(a => a.UserId == userId).FirstOrDefaultAsync();
            return cart;
        }
        public async Task<Cart> GetCart(string userId) 
        {
            var cart =await _context.Carts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;

        }
        public async Task<int> GetCartItemCount(string userId = "")
        {
            if(!string.IsNullOrEmpty(userId))
            {
                userId =GetUserId();
            }
            var data = await (from cart in _context.Carts
                              join CartItem in _context.CartItems
                              on cart.Id equals CartItem.CartId
                              where cart.UserId == userId
                              select new { CartItem.Id}
                              ).ToListAsync();
            return data.Count;
        }
        public async Task<bool> DoCheckout()
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // logic
                // move data from cartDetail to order and order detail then we will remove cart detail
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User is not logged-in");
                var cart = await GetCart(userId);
                if (cart is null)
                    throw new Exception("Invalid cart");
                var cartItem = _context.CartItems
                                    .Where(a => a.CartId == cart.Id).ToList();
                if (cartItem.Count == 0)
                    throw new Exception("Cart is empty");
                var order = new Order
                {
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow,
                    OrderStatusId = 1//pending
                };
                _context.Orders.Add(order);
                _context.SaveChanges();
                foreach (var item in cartItem)
                {
                    var orderDetail = new OrderDetail
                    {
                        ProductId = item.ProductId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        UnitPrice = (int)item.UnitPrice
                    };
                    _context.OrderDetails.Add(orderDetail);
                }
                _context.SaveChanges();

                // removing the CartItems
                _context.CartItems.RemoveRange(cartItem);
                _context.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            var userId = _userManager.GetUserId(principal);
            return userId;
        }
    }
}
