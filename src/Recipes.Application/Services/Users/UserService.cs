using Recipes.Application.DTOs.Users;
using Recipes.Application.Interfaces.Auth;
using Recipes.Application.Interfaces.Users;
using Recipes.Application.Mappings;
using Recipes.Domain.Entities.Users;
using Recipes.Domain.Interfaces.Users;

namespace Recipes.Application.Services.Users;

public class UserService(
    IUserRepository userRepository,
    IUserContext userContext,
    IPasswordService passwordService,
    ILogger<UserService> logger) : IUserService
{
    public async Task<IEnumerable<UserResponse>> GetAll()
    {
        logger.LogDebug("GetAll()");

        var users = await userRepository.GetAll();

        return users.Select(user => user.ToResponse());
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

        return user == null ? null : user.ToResponse();
    }

    public async Task<UserResponse?> Update(UpdateUserRequest request)
    {
        logger.LogDebug("Update()");

        if (await userRepository.GetById(request.Id) is not { } existingUser)
            return null;

        existingUser.Update(request.Name);

        var updatedUser = await userRepository.Update(existingUser);

        return updatedUser == null ? null : updatedUser.ToResponse();
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

        var user = await userRepository.Create(
            new User(request.Name, request.Email.ToLower(), passwordService.HashPassword(request.Password)));

        return user == null ? null : user.ToResponse();
    }
}
