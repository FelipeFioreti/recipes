using Recipes.Domain.DTOs.Users;

namespace Recipes.Domain.Interfaces.Users;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAll();
    Task<UserResponse?> GetCurrent();
    Task<UserResponse?> GetById(int id);
    Task<UserResponse?> Create(CreateUserRequest request);
    Task<UserResponse?> Update(int id, UpdateUserRequest request);
    Task<bool> Disable(int id);
}
