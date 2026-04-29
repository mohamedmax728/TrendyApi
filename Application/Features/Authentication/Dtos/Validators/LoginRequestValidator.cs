using FluentValidation;

namespace Application.Features.Authentication.Dtos.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .NotNull()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .NotNull();
        }
    }
}
