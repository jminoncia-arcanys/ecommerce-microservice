using Microsoft.EntityFrameworkCore;
using UserAuthMicroservice.Data;
using UserAuthMicroservice.Model;
using UserAuthMicroservice.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserAuthDbContext _context;

    public UserRepository(UserAuthDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserByIdAsync(int id) => await _context.Users.FindAsync(id);

    public async Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User> FindByUsernameAsync(string username) =>
        await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
}
