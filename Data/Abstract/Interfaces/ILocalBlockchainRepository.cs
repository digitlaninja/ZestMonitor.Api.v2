using System.Collections.Generic;
using System.Threading.Tasks;
using ZestMonitor.Api.Data.Entities;

namespace ZestMonitor.Api.Data.Abstract.Interfaces
{
    public interface ILocalBlockchainRepository
    {
        Task<List<BlockchainProposal>> GetProposals();
        Task<BlockchainProposal> GetProposal(string name);
        Task<int> GetValidCount();
        Task<int> GetFundedCount();
    }
}