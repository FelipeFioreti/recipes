using Recipes.Domain.DTOs.Users;

namespace Recipes.Domain.Interfaces.Users;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAll();
    Task<UserResponse?> GetById(int id);
    Task<UserResponse?> Create(CreateUserRequest request);
    Task<UserResponse?> Update(int userId, UpdateUserRequest request);
    Task<bool> Disable(int id);
    Task<bool> Delete(int id);
}