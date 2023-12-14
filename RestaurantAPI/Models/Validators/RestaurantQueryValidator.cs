using FluentValidation;
using RestaurantAPI.Entities;

namespace RestaurantAPI.Models.Validators
{
	public class RestaurantQueryValidator : AbstractValidator<RestaurantQuery>
	{
        private int[] allowedPageSize = {5, 10, 15};
        private string[] allowedSortBy = { nameof(Restaurant.Name), nameof(Restaurant.Description), nameof(Restaurant.Category) };

        public RestaurantQueryValidator()
        {
            RuleFor(r => r.PageNumber)
                .GreaterThanOrEqualTo(1);

            RuleFor(r => r.PageSize)
                .Custom((value, context) =>
                {
                    if(!allowedPageSize.Contains(value))
                    {
                        context.AddFailure("PageSize", $"PageSize must be in [{string.Join(',', allowedPageSize)}]");
                    }
                });

            RuleFor(r => r.SortBy)
                .Must(value => string.IsNullOrEmpty(value) || allowedSortBy.Contains(value))
                .WithMessage($"SortBy is optional, or must be in [{string.Join(',', allowedSortBy)}]");
        }
    }
}
