using ZestMonitor.Api.Data.Entities;

namespace ZestMonitor.Api.Data.Models
{
    public class User : EntityBase
    {
        public string Username { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordHash { get; set; }
    }
}