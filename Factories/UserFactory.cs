using System;
using ZestMonitor.Api.Data.Models;

namespace ZestMonitor.Api.Factories
{
    public static class UserFactory
    {
        public static UserRegistrationModel ToModel(this User entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new UserRegistrationModel()
            {
                Username = entity.Username
            };
        }
    }
}