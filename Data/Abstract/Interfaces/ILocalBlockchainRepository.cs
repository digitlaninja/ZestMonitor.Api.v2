using System.Collections.Generic;
using System.Threading.Tasks;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;

namespace ZestMonitor.Api.Data.Abstract.Interfaces
{
    public interface ILocalBlockchainRepository : IRepository<BlockchainProposal>
    {
        Task<List<BlockchainProposal>> GetProposals();
        Task<BlockchainProposal> GetProposal(string name);
        Task<ProposalMetadataModel> GetMetadata();
        Task<int> GetValidCount();
        Task<int> GetFundedCount();

    }
}