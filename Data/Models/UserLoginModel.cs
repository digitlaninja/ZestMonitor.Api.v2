using FluentValidation.Attributes;
using ZestMonitor.Api.Validation;

namespace ZestMonitor.Api.Data.Models
{

    [Validator(typeof(UserLoginValidator))]
    public class UserLoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}