using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ZestMonitor.Api.Data.Abstract.Interfaces;
using ZestMonitor.Api.Data.Models;

namespace ZestMonitor.Api.Services
{
    public class BlockchainService
    {
        public ILogger<BlockchainService> Logger { get; }
        public IBlockchainRepository BlockchainRepository { get; }

        public BlockchainService(ILogger<BlockchainService> logger, IBlockchainRepository BlockchainRepository)
        {
            this.Logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this.BlockchainRepository = BlockchainRepository ?? throw new ArgumentNullException(nameof(BlockchainRepository));
        }

        public async Task<List<BlockchainProposal>> GetProposals()
        {
            return await this.BlockchainRepository.GetProposals();
        }

        public async Task<BlockchainProposal> GetProposal(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            return await this.BlockchainRepository.GetProposal(name);
        }

        public async Task<int> GetValidCount()
        {
            return await this.BlockchainRepository.GetValidCount();
        }

        public async Task<int> GetFundedCount()
        {
            return await this.BlockchainRepository.GetFundedCount();
        }
    }
}