using Microsoft.EntityFrameworkCore;
using ProductCatalogMicroservice.Data;
using ProductCatalogMicroservice.Model;

namespace ProductCatalogMicroservice.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly ProductCatalogDbContext _context;

        public CartItemRepository(ProductCatalogDbContext context)
        {
            _context = context;
        }

        public async Task<CartItem> AddOrUpdateCartItemAsync(CartItem cartItem)
        {
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == cartItem.UserId && c.ProductId == cartItem.ProductId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += cartItem.Quantity;
                _context.CartItems.Update(existingCartItem);
            }
            else
            {
                await _context.CartItems.AddAsync(cartItem);
            }

            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task<CartItem> GetCartItemAsync(int userId, int productId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
        }
    }
}
