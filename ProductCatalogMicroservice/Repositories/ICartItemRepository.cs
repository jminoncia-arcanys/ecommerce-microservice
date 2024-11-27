using ProductCatalogMicroservice.Model;

namespace ProductCatalogMicroservice.Repositories
{
    public interface ICartItemRepository
    {
        Task<CartItem> AddOrUpdateCartItemAsync(CartItem cartItem);
        Task<CartItem> GetCartItemAsync(int userId, int productId);
    }
}
