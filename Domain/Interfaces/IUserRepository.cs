using IdeonBack.Domain.Entities;

namespace IdeonBack.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<IEnumerable<User>> GetAllAsync();
}
