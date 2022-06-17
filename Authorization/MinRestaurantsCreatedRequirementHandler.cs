using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using RestaurantApi.Entities;

namespace RestaurantApi.Authorization;

public class MinRestaurantsCreatedRequirementHandler : AuthorizationHandler<MinRestaurantsCreatedRequirement>
{
    private readonly RestaurantDbContext _dbContext;

    public MinRestaurantsCreatedRequirementHandler(RestaurantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinRestaurantsCreatedRequirement requirement)
    {
        var userId = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

        var restaurantsCount = _dbContext.Restaurants.Where(r => r.CreatedById == userId).Count();

        if (restaurantsCount >= requirement.MinRestaurantsCreated) {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}