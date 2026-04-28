using Application.Features.Products.Dtos;
using FluentValidation;

namespace Application.Features.Products.Dtos.Validators
{
    public class CategoryValidator : AbstractValidator<CreateUpdateCategoryDto>
    {
        public CategoryValidator()
        {
            RuleFor(c => c.NameEn)
                .NotEmpty().WithMessage("Category name in English is required.")
                .NotNull().WithMessage("Category name in English is required.")
                .MaximumLength(100).WithMessage("Category name in English cannot exceed 100 characters.")
                .Matches("^[a-zA-Z0-9\\s\\-.,!?]+$")
                .WithMessage("Category name in English contains invalid characters.");

            RuleFor(c => c.NameAr)
                .NotEmpty().WithMessage("Category name in Arabic is required.")
                .NotNull().WithMessage("Category name in Arabic is required.")
                .MaximumLength(100).WithMessage("Category name in Arabic cannot exceed 100 characters.")
                .Matches("^[\\p{IsArabic}\\s\\-.,!?0-9]+$")
                .WithMessage("Category name in Arabic contains invalid characters.");
        }
    }
}
