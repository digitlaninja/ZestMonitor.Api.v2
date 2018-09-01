using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ZestMonitor.Api.Data.Models;

namespace ZestMonitor.Api.Data.Abstract.Interfaces
{
    public interface IBlockchainRepository
    {
        Task<List<BlockchainProposal>> GetProposals();
        HttpWebRequest CreateRequest(string json);
        Task<int> GetValidCount();
        Task<int> GetFundedCount();
    }
}