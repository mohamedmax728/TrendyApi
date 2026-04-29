using Application.Common.Helpers.Utilities.Validation;
using FluentValidation;

namespace Application.Features.Authentication.Dtos.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestValidator()
        {
            RuleFor(s => s.Email)
                .EmailAddress()
                .NotEmpty()
                .NotNull();

            RuleFor(s => s.PhoneNumber)
                .NotEmpty()
                .NotNull();

            RuleFor(s => s.Password)
                .MinimumLength(8).NotNull().NotEmpty()
                .Equal(s => s.ConfirmPassword).WithMessage("Passwords do not match");

            RuleFor(s => s.FullNameEn)
                .NotEmpty()
                .NotNull()
                .Matches(ValidationRegex.EnglishLetters);

            RuleFor(s => s.FullNameAr)
                .NotEmpty()
                .NotNull()
                .Matches(ValidationRegex.ArabicLetters);
        }
    }
}

