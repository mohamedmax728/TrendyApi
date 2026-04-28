using Application.Common.Helpers.Utilities.Validation;
using Application.Features.Products.Dtos;
using FluentValidation;

namespace Application.Features.Products.Dtos.Validators
{
    public class ProductValidator : AbstractValidator<CreateUpdateProductDto>
    {
        public ProductValidator()
        {
            RuleFor(p => p.NameEn)
                .NotEmpty().WithMessage("Name in English is required.")
                .NotNull().WithMessage("Name in English is required.")
                .MaximumLength(200).WithMessage("Name in English cannot exceed 200 characters.")
                .Matches(ValidationRegex.EnglishLettersAndNumbersAndPunctuation)
                .WithMessage("Name in English contains invalid characters.");

            RuleFor(p => p.NameAr)
                .NotEmpty().WithMessage("Name in Arabic is required.")
                .NotNull().WithMessage("Name in Arabic is required.")
                .MaximumLength(200).WithMessage("Name in Arabic cannot exceed 200 characters.")
                .Matches(ValidationRegex.ArabicLettersAndNumbersAndPunctuation)
                .WithMessage("Name in Arabic contains invalid characters.");

            RuleFor(p => p.VendorId)
                .NotNull().WithMessage("Vendor ID is required.")
                .GreaterThan(0).WithMessage("Vendor ID must be greater than 0.");


        }
    }
}
