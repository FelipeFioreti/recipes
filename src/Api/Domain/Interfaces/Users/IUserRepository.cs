using Recipes.Api.Domain.Entities.Users;

namespace Recipes.Api.Domain.Interfaces.Users;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAll(int page = 1, int size = 10);
    Task<User?> GetById(int id);
    Task<User?> GetByEmail(string email);
    Task<User?> Create(User user);
    Task<User?> Update(User user);
}