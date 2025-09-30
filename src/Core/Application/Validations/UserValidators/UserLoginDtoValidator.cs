using Application.DTOs.UserDTOs;
using FluentValidation;

namespace Application.Validations.UserValidators;

public class UserLoginDtoValidator : AbstractValidator<LoginDto>
{
    public UserLoginDtoValidator()
    {
        RuleFor(x => x.Email)
           .NotEmpty().WithMessage("Email is required.")
           .EmailAddress().WithMessage("Email format is not valid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.");
    }
}
