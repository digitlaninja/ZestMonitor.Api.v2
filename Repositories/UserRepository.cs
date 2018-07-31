using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZestMonitor.Api.Data.Abstract.Interfaces;
using ZestMonitor.Api.Data.Contexts;
using ZestMonitor.Api.Data.Models;

namespace ZestMonitor.Api.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
         public UserRepository(ZestContext context) : base(context) { }

          public Task<User> Get(string username) {
              return this.Context.Set<User>().FirstOrDefaultAsync(x => x.Username == username);
          }
        
    }
}