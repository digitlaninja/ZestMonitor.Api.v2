using FluentValidation.Attributes;
using ZestMonitor.Api.Validation;

namespace ZestMonitor.Api.Data.Models
{

    [Validator(typeof(UserRegistrationValidator))]
    public class UserRegistrationModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}