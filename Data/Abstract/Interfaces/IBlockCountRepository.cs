using System.Threading.Tasks;
using ZestMonitor.Api.Data.Abstract.Interfaces;
using ZestMonitor.Api.Data.Entities;

namespace ZestMonitor.Api.Repositories
{
    public interface IBlockCountRepository : IRepository<BlockCount>
    {
        Task<bool> UpdatedToday();
        Task AddBlockCount();
        Task<BlockCount> GetLatestLocalBlockCount();
    }
}