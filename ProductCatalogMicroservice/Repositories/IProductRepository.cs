using ProductCatalogMicroservice.Model;

namespace ProductCatalogMicroservice.Repositories
{
    public interface IProductRepository
    {
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> AddProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
    }
}
