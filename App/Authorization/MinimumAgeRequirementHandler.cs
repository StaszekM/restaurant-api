using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace RestaurantApi.Authorization;
public class MinimumAgeRequirementHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    private readonly ILogger<MinimumAgeRequirementHandler> _logger;
    public MinimumAgeRequirementHandler(ILogger<MinimumAgeRequirementHandler> logger)
    {
        _logger = logger;
    }
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
    {
        var user = context.User;

        if (user is null)
        {
            return Task.CompletedTask;
        }

        var dateOfBirthClaim = user.FindFirst(c => c.Type == "DateOfBirth");
        var emailClaim = user.FindFirst(c => c.Type == ClaimTypes.Name);

        if (dateOfBirthClaim is null || emailClaim is null)
        {
            return Task.CompletedTask;
        }

        var dateOfBirth = DateTime.Parse(dateOfBirthClaim.Value);
        var userEmail = emailClaim.Value;

        _logger.LogInformation($"User {userEmail} with date of birth {dateOfBirth} is being authorized");

        if (dateOfBirth.AddYears(requirement.MinimumAge) < DateTime.Today)
        {
            _logger.LogInformation($"User {userEmail} succeeded authorization");
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogInformation($"User {userEmail} failed authorization");
        }

        return Task.CompletedTask;
    }
}