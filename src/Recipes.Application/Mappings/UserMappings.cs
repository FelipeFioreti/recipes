using Recipes.Application.DTOs.Users;
using Recipes.Domain.Entities.Users;

namespace Recipes.Application.Mappings;

public static class UserMappings
{
    public static UserResponse ToResponse(this User user)
    {
        return new UserResponse(user);
    }
}
