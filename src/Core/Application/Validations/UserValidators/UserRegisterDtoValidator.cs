using Application.DTOs.UserDTOs;
using FluentValidation;

namespace Application.Validations.UserValidators;

public class UserRegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public UserRegisterDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MinimumLength(3).WithMessage("Full name must be at least 3 characters.")
            .MaximumLength(50).WithMessage("Full name must not exceed 50 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is not valid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.");
    }
}
