using System.Threading.Tasks;
using ZestMonitor.Api.Data.Models;

namespace ZestMonitor.Api.Data.Abstract.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> Get(string username);
        Task<bool> Exists(string username);
    }
}