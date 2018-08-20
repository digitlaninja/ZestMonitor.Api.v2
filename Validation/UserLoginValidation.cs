using FluentValidation;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;

namespace ZestMonitor.Api.Validation
{
    public class UserLoginValidator : AbstractValidator<UserLoginModel>
    {
        public UserLoginValidator()
        {
            RuleFor(x => x.Username)
                .NotNull().WithMessage("Please enter your existing username.")
                .NotEmpty().WithMessage("Please enter your existing username.");

            RuleFor(x => x.Password)
                .NotNull().WithMessage("Please enter your existing password.")
                .NotEmpty().WithMessage("Please enter your existing password.");
        }
    }
}