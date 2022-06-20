using FluentValidation;
using RestaurantApi.Entities;

namespace RestaurantApi.Models.Validators;

public class RestaurantQueryValidator : AbstractValidator<RestaurantQuery>
{
    private int[] allowedPageSizes = new int[] { 5, 10, 15 };
    private string[] allowedSortByColumnNames = new string[] { nameof(Restaurant.Name), nameof(Restaurant.Description), nameof(Restaurant.Category) };
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
        RuleFor(q => q.SortBy).Must(value => string.IsNullOrEmpty(value) || allowedSortByColumnNames.Contains(value)).WithMessage($"SortBy is optional or must be in {string.Join(',', allowedSortByColumnNames)}");
    }
}