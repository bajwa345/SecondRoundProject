using FluentValidation;
using SecondRoundProject.DTOs;

namespace SecondRoundProject.Validators
{
    public class ClientModelValidator : AbstractValidator<ClientDTO>
    {
        public ClientModelValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(60);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(60);
            RuleFor(x => x.MobileNumber).NotEmpty().Matches(@"^\+\d{1,3}\d{1,14}$");
            RuleFor(x => x.PersonalId).NotEmpty().Length(11);
            RuleFor(x => x.Sex).NotEmpty().Must(x => x == "Male" || x == "Female");
            RuleFor(x => x.Accounts).NotEmpty().WithMessage("At least one account is required.");
        }
    }
}
