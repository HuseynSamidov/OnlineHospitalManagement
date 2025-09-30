using Application.DTOs.CategoryDTOs.Create;
using FluentValidation;

namespace Application.Validations.CategoryValidators
{
    public class CreateCategoryValidator : AbstractValidator<CreateDepartmentDto>
    {
        public CreateCategoryValidator()
        {
            RuleFor(c => c.DepartmentName)
                .NotEmpty().WithMessage("Name can not be null")
                .MinimumLength(3).WithMessage("Name should be minumum 3 character")
                .MaximumLength(50).WithMessage("Name can only contains 50 characters");
        }
    }
}
