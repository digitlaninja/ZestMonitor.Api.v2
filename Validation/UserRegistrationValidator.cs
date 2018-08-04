using FluentValidation;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;

namespace ZestMonitor.Api.Validation
{
    public class UserRegistrationValidator : AbstractValidator<UserRegistrationModel>
    {
        public UserRegistrationValidator()
        {
            RuleFor(x => x.Username).NotNull().NotEmpty().WithMessage("Please enter your Username.");
            RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("Please enter a password of 8 - 20 characters with capital letters, symbols and numbers.");
        }
    }
}