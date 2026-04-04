using Recipes.Domain.Entities.Users;
using Recipes.Domain.Interfaces.Users;

namespace Recipes.Application.Services.Users;

public class UserService(IUserRepository userRepository, ILogger<UserService> logger) : IUserService
{
    public async Task<IEnumerable<User>> GetAll()
    {
        logger.LogDebug("GetAll()");

        return await userRepository.GetAll();
    }

    public async Task<User?> GetById(int id)
    {
        logger.LogDebug("GetById()");

        return await userRepository.GetById(id);
    }

    public async Task<User?> GetByEmail(string email)
    {
        logger.LogDebug("GetByEmail()");

        return await userRepository.GetByEmail(email);
    }

    public async Task<User?> Create(User user)
    {
        logger.LogDebug("Create()");

        user.Password = HashPassword(user.Password);

        return await userRepository.Create(user);
    }

    public async Task<User?> Update(User user)
    {
        logger.LogDebug("Update()");

        if (!string.IsNullOrWhiteSpace(user.Password))
            user.Password = HashPassword(user.Password);

        return await userRepository.Update(user);
    }

    public async Task Disable(int id)
    {
        logger.LogDebug("Disable()");

        var user = await GetById(id);

        if (user == null)
            return;

        user.DeletedAt = DateTime.Now;
        await userRepository.Update(user);
    }

    public async Task Delete(int id)
    {
        logger.LogDebug("Delete()");

        var user = await GetById(id);

        if (user == null)
            return;

        await userRepository.Delete(user);
    }

    private static string HashPassword(string password)
    {
        return password.StartsWith("$2") ? password : BCrypt.Net.BCrypt.HashPassword(password);
    }
}