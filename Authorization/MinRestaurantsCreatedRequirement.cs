using Microsoft.AspNetCore.Authorization;

namespace RestaurantApi.Authorization;
public class MinRestaurantsCreatedRequirement : IAuthorizationRequirement
{
    public int MinRestaurantsCreated { get; }
    public MinRestaurantsCreatedRequirement(int minRestaurantsCreated)
    {
        MinRestaurantsCreated = minRestaurantsCreated;
    }
}