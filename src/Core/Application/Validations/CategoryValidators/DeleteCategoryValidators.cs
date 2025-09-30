using Application.DTOs.CategoryDTOs.Delete;
using FluentValidation;

namespace Application.Validations.CategoryValidators;

public class CategoryDeleteDtoValidator : AbstractValidator<DeleteDepartmentDto>
{
    public CategoryDeleteDtoValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty().WithMessage("Id can't be empty");
    }
}
