using UserAuthMicroservice.Model;

namespace UserAuthMicroservice.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(int id);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task<User> FindByUsernameAsync(string username);
    }
}
