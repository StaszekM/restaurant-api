using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using RestaurantApi.Entities;

namespace RestaurantApi.Authorization;

public class ResourceOperationRequirementHandler : AuthorizationHandler<ResourceOperationRequirement, Restaurant>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOperationRequirement requirement, Restaurant restaurant)
    {
        ResourceOperation operation = requirement.ResourceOperation;

        if (operation == ResourceOperation.Read || operation == ResourceOperation.Create) {
            context.Succeed(requirement);
        }

        var userId = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId is not null && restaurant.CreatedById == int.Parse(userId))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}