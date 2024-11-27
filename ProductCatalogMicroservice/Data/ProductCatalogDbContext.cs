using Microsoft.EntityFrameworkCore;
using ProductCatalogMicroservice.Model;

namespace ProductCatalogMicroservice.Data
{
    public class ProductCatalogDbContext : DbContext
    {
        public ProductCatalogDbContext(DbContextOptions<ProductCatalogDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }


    }
}
