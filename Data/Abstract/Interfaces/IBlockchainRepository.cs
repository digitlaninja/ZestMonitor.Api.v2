using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;
using ZestMonitor.Api.Helpers;

namespace ZestMonitor.Api.Data.Abstract.Interfaces
{
    public interface IBlockchainRepository
    {
        Task<List<BlockchainProposalJson>> GetProposals();
        Task<List<BlockchainProposalJson>> GetPagedProposals(PagingParams pagingParams);
        Task<BlockchainProposalJson> GetProposal(string name);
        HttpWebRequest CreateRequest(string json);
        Task<int> GetValidCount();
        Task<int> GetFundedCount();
        Task<ProposalMetadataModel> GetMetadata();
    }
}