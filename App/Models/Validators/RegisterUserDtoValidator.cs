using FluentValidation;
using RestaurantApi.Entities;

namespace RestaurantApi.Models.Validators;
public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator(RestaurantDbContext dbcontext)
    {

        RuleFor(x => x.Email).NotEmpty().EmailAddress().Custom((email, context) =>
        {
            var emailExists = dbcontext.Users.Any(u => u.Email == email);
            if (emailExists)
            {
                context.AddFailure(nameof(User.Email), "That email is already taken.");
            }
        });

        RuleFor(x => x.Password).MinimumLength(6);
        RuleFor(x => x.ConfirmedPassword).Equal(x => x.Password).WithMessage("Password and confirmed password do not match.");
    }
}