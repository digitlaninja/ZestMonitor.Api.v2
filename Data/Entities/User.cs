using ZestMonitor.Api.Data.Entities;

namespace ZestMonitor.Api.Data.Models
{
    public class User : EntityBase
    {
        public string Username { get; set; }
        public byte[] PasswordSalt { get; set; }
        public byte[] PasswordHash { get; set; }
    }
}