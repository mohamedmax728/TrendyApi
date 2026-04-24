using FluentValidation;

namespace Application.Features.Authentication.Dtos.Validators
{
    public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequestDto>
    {
        public ForgotPasswordRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .NotNull()
                .EmailAddress();
        }
    }
}
