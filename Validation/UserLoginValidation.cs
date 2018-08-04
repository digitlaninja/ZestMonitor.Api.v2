using FluentValidation;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;

namespace ZestMonitor.Api.Validation
{
    public class UserLoginValidator : AbstractValidator<UserLoginModel>
    {
        public UserLoginValidator()
        {
            RuleFor(x => x.Username).NotNull().NotEmpty().WithMessage("Please enter your existing username.");
            RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("Please enter your existing password.");
        }
    }
}