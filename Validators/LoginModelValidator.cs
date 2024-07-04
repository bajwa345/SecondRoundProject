using FluentValidation;
using SecondRoundProject.DTOs;

namespace SecondRoundProject.Validators
{
    public class LoginModelValidator : AbstractValidator<LoginDTO>
    {
        public LoginModelValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
        }
    }
}
