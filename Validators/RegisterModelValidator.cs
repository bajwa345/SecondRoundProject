using FluentValidation;
using SecondRoundProject.DTOs;

namespace SecondRoundProject.Validators
{
    public class RegisterModelValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterModelValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First is required.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("First is required.");
            RuleFor(x => x.Role).NotEmpty().WithMessage("Role is required.")
                .Must(role => role == "Admin" || role == "User").WithMessage("Role must be either 'Admin' or 'User'."); ;
        }
    }
}
