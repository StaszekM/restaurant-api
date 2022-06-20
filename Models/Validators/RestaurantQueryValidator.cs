using FluentValidation;

namespace RestaurantApi.Models.Validators;

public class RestaurantQueryValidator : AbstractValidator<RestaurantQuery>
{
    private int[] allowedPageSizes = new int[] { 5, 10, 15 };
    public RestaurantQueryValidator()
    {
        RuleFor(q => q.PageNumber).GreaterThan(0);
        RuleFor(q => q.PageSize).Custom((value, context) =>
        {
            if (!allowedPageSizes.Contains(value))
            {
                context.AddFailure(nameof(RestaurantQuery.PageSize), $"Page size must be in {string.Join(',', allowedPageSizes)}");
            }
        });
    }
}