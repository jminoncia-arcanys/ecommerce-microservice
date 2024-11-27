using Microsoft.EntityFrameworkCore;
using UserAuthMicroservice.Model;

namespace UserAuthMicroservice.Data
{
    public class UserAuthDbContext : DbContext
    {
        public UserAuthDbContext(DbContextOptions<UserAuthDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }


    }
}
