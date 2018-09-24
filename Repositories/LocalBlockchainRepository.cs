using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZestMonitor.Api.Data.Abstract.Interfaces;
using ZestMonitor.Api.Data.Contexts;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;

namespace ZestMonitor.Api.Repositories
{
    // Deals with Locally Saved Proposals from the Blockchain
    public class LocalBlockchainRepository : Repository<BlockchainProposal>, ILocalBlockchainRepository
    {
        public LocalBlockchainRepository(ZestContext context) : base(context)
        {
        }
        public async Task<List<BlockchainProposal>> GetProposals()
        {
            return await this.GetAll()?.ToListAsync();
        }

        public async Task<BlockchainProposal> GetProposal(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            var result = await this.GetAll()?.FirstOrDefaultAsync(x => x.Name == name);
            return result;
        }

        // (yeas / nay) * 100 = % yeas
        public async Task<int> GetValidCount()
        {
            var proposals = this.GetAll();
            if (proposals == null)
                return -1;

            var result = await proposals.CountAsync(x => x.IsValid);
            return result;
        }

        public async Task<int> GetFundedCount()
        {
            var proposals = this.GetAll();
            if (proposals == null)
                return -1;

            var result = await proposals.CountAsync(x => x.IsEstablished);
            return result;
        }

        // public async Task<ProposalMetadataModel> GetMetadata()
        // {
        //     var proposals = await this.GetProposals();
        //     if (proposals == null)
        //         return null;

        //     var validCount = proposals.Count(x => x.IsValid);
        //     var fundedCount = proposals.Count(x => x.IsEstablished);

        //     return new ProposalMetadataModel()
        //     {
        //         ValidProposalCount = validCount,
        //         FundedProposalCount = fundedCount
        //     };
        // }

    }
}