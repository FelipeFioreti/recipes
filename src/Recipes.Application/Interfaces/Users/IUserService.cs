using Recipes.Application.DTOs.Users;

namespace Recipes.Application.Interfaces.Users;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAll();
    Task<UserResponse?> GetCurrent();
    Task<UserResponse?> GetById(int id);
    Task<UserResponse?> Create(CreateUserRequest request);
    Task<UserResponse?> Update(UpdateUserRequest request);
    Task<bool> Disable(int id);
}
