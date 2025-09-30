using Application.DTOs.CategoryDTOs.Update;
using FluentValidation;

namespace Application.Validations.CategoryValidators;

public class CategoryUpdatedDtoValidator : AbstractValidator<UpdateDepartmentDto>
{
    public CategoryUpdatedDtoValidator()
    {
        RuleFor(c => c.NewName)
            .NotEmpty().WithMessage("Name can not be null")
            .MinimumLength(3).WithMessage("Name should be minumum 3 character");
    }
}
