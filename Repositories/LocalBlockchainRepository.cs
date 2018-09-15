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
    public class LocalBlockchainRepository : Repository<BlockchainProposal>, ILocalBlockchainRepository
    {
        public LocalBlockchainRepository(ZestContext context) : base(context)
        {
        }
        public async Task<List<BlockchainProposal>> GetProposals() => await this.GetAll().ToListAsync();

        public async Task<BlockchainProposal> GetProposal(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            var result = await this.GetAll().FirstOrDefaultAsync(x => x.Name == name);
            return result;
        }

        // (yeas / nay) * 100 = % yeas
        public async Task<int> GetValidCount()
        {
            var proposals = await this.GetAll().ToListAsync();
            if (proposals == null)
                return -1;

            var result = proposals.Count(x => x.IsValid);
            return result;
        }

        public async Task<int> GetFundedCount()
        {
            var proposals = await this.GetAll().ToListAsync();
            if (proposals == null)
                return -1;

            var result = proposals.Count(x => x.IsEstablished);
            return result;
        }

        public async Task<ProposalMetadataModel> GetMetadata()
        {
            var validCount = await this.GetValidCount();
            var fundedCount = await this.GetFundedCount();

            return new ProposalMetadataModel()
            {
                ValidProposalCount = validCount,
                FundedProposalCount = fundedCount
            };
        }
    }
}