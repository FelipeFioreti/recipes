using Recipes.Domain.Entities.Users;

namespace Recipes.Domain.Interfaces.Users;

public interface IUserService
{
    Task<IEnumerable<User>> GetAll();
    Task<User?> GetById(int id);
    Task<User?> GetByEmail(string email);
    Task<User?> Create(User user);
    Task<User?> Update(User user);
    Task Disable(int id);
    Task Delete(int id);
}