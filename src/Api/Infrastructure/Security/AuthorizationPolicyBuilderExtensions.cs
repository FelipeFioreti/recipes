using Microsoft.AspNetCore.Authorization;
using Recipes.Api.Domain.Entities.Enums;

namespace Recipes.Api.Infrastructure.Security;

public static class AuthorizationPolicyBuilderExtensions
{
    extension(AuthorizationPolicyBuilder policy)
    {
        public AuthorizationPolicyBuilder RequireApplicationUser()
        {
            policy.RequireAuthenticatedUser();
            policy.RequireAssertion(context =>
                context.User.HasUserIdentifier() &&
                context.User.HasUserEmail());

            return policy;
        }

        public AuthorizationPolicyBuilder RequireAdminUser()
        {
            policy.RequireApplicationUser();
            policy.RequireAssertion(context => context.User.HasApplicationRole(nameof(Roles.ADMIN)));

            return policy;
        }
    }
}