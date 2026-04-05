using Recipes.Domain.DTOs.Users;
using Recipes.Domain.Entities.Users;
using Recipes.Domain.Interfaces.Auth;
using Recipes.Domain.Interfaces.Users;

namespace Recipes.Application.Services.Users;

public class UserService(
    IUserRepository userRepository,
    IUserContext userContext,
    ILogger<UserService> logger) : IUserService
{
    public async Task<IEnumerable<UserResponse>> GetAll()
    {
        logger.LogDebug("GetAll()");

        var users = await userRepository.GetAll();

        return users.Select(ToResponse);
    }

    public async Task<UserResponse?> GetCurrent()
    {
        logger.LogDebug("GetCurrent()");

        return await GetById(userContext.GetUserId());
    }

    public async Task<UserResponse?> GetById(int id)
    {
        logger.LogDebug("GetById()");

        var user = await userRepository.GetById(id);

        return user == null ? null : ToResponse(user);
    }

    public async Task<UserResponse?> UpdateCurrent(UpdateUserRequest request)
    {
        logger.LogDebug("UpdateCurrent()");

        return await Update(userContext.GetUserId(), request);
    }

    public async Task<UserResponse?> Update(int id, UpdateUserRequest request)
    {
        logger.LogDebug("Update()");

        var existingUser = await userRepository.GetById(id);

        if (existingUser == null)
            return null;

        existingUser.Update(request);

        var updatedUser = await userRepository.Update(existingUser);

        return updatedUser == null ? null : ToResponse(updatedUser);
    }

    public async Task<bool> DisableCurrent()
    {
        logger.LogDebug("DisableCurrent()");

        return await Disable(userContext.GetUserId());
    }

    public async Task<bool> Disable(int id)
    {
        logger.LogDebug("Disable()");

        var user = await userRepository.GetById(id);

        if (user == null)
            return false;

        user.Disable();
        await userRepository.Update(user);

        return true;
    }
    
    public async Task<UserResponse?> Create(CreateUserRequest request)
    {
        logger.LogDebug("Create()");

        var user = await userRepository.Create(new User(request.Name, request.Email.ToLower(), request.Password));

        return user == null ? null : ToResponse(user);
    }

    private static UserResponse ToResponse(User user)
    {
        return new UserResponse(user);
    }
}
