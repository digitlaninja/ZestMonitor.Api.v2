using System.Threading.Tasks;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;

namespace ZestMonitor.Api.Data.Abstract.Interfaces
{
    public interface IMasternodeCountRepository
    {
        Task AddMasternodeCount();
        MasternodeCountJson GetMasternodeCountFromChain();
        MasternodeCount GetLatestLocalMasternodeCount();
        Task<bool> UpdatedToday();
    }
}