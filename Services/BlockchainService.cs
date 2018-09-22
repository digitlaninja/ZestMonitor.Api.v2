using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZestMonitor.Api.Data.Abstract.Interfaces;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;
using ZestMonitor.Api.Factories;
using ZestMonitor.Api.Helpers;

namespace ZestMonitor.Api.Services
{
    // Deals directly with the Blockchain
    public class BlockchainService
    {
        private IProposalPaymentsRepository proposalPaymentsRepository { get; }
        public ILogger<BlockchainService> Logger { get; }
        public IBlockchainRepository BlockchainRepository { get; }
        public ILocalBlockchainRepository LocalBlockchainRepository { get; }
        public ProposalPaymentsService ProposalPaymentsService { get; }

        public BlockchainService(ILogger<BlockchainService> logger, IBlockchainRepository BlockchainRepository, ProposalPaymentsService proposalPaymentsService, ILocalBlockchainRepository localBlockchainRepository, IProposalPaymentsRepository proposalPaymentsRepository)
        {
            this.Logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this.BlockchainRepository = BlockchainRepository ?? throw new ArgumentNullException(nameof(BlockchainRepository));
            this.ProposalPaymentsService = proposalPaymentsService ?? throw new ArgumentNullException(nameof(proposalPaymentsService));
            this.proposalPaymentsRepository = proposalPaymentsRepository ?? throw new ArgumentNullException(nameof(proposalPaymentsRepository));
            this.LocalBlockchainRepository = localBlockchainRepository ?? throw new ArgumentNullException(nameof(localBlockchainRepository));
        }

        public async Task SaveProposals()
        {
            // get block proposals -> convert to entities
            var proposalsFromBlockchain = this.BlockchainRepository.GetProposals();
            if (proposalsFromBlockchain.Count() <= 0 || proposalsFromBlockchain == null)
                return;

            var blockchainProposals = proposalsFromBlockchain.ToEntities();
            var localBlockchainProposals = await this.LocalBlockchainRepository.GetProposals();
            var proposalPayments = await this.ProposalPaymentsService.GetAll();
            var masternodeCount = this.GetMasternodeCount();
            foreach (var blockchainProposal in blockchainProposals)
            {
                var existingProposal = localBlockchainProposals.FirstOrDefault(x => x.Name == blockchainProposal.Name);
                var newProposal = existingProposal == null;
                if (newProposal)
                {
                    // Create full blockchain proposal
                    var completeBlockchainProposal = this.ConstructFullBlockchainProposal(proposalPayments, blockchainProposal, masternodeCount);
                    await this.LocalBlockchainRepository.Add(completeBlockchainProposal);
                }
            }

            if (this.LocalBlockchainRepository.AnyTracked())
                await this.LocalBlockchainRepository.SaveAll();
        }

        // Builds a complete blockchain proposal with Time to store in the db
        private BlockchainProposal ConstructFullBlockchainProposal(IEnumerable<ProposalPayments> localProposals, BlockchainProposal blockchainProposal, int masternodeCount)
        {
            if (localProposals == null || blockchainProposal == null)
                return null;

            var entity = new BlockchainProposal();
            var blockLocalProposalMatch = localProposals.OrderByDescending(x => x.CreatedAt).FirstOrDefault(x => x.Hash == blockchainProposal.Hash);

            entity.Time = !string.IsNullOrEmpty(blockchainProposal.FeeHash) ? this.BlockchainRepository.GetTime(blockchainProposal.FeeHash) : null;
            entity.Name = blockchainProposal.Name;
            entity.Url = blockchainProposal.Url;
            entity.Hash = blockchainProposal.Hash;
            entity.FeeHash = blockchainProposal.FeeHash;
            entity.Yeas = blockchainProposal.Yeas;
            entity.Nays = blockchainProposal.Nays;
            entity.Abstains = blockchainProposal.Abstains;
            entity.Ratio = blockchainProposal.Ratio;
            entity.RatioPercent = (blockchainProposal.Yeas - blockchainProposal.Nays) / masternodeCount;
            entity.IsEstablished = blockchainProposal.IsEstablished;
            entity.IsValid = blockchainProposal.IsValid;
            entity.IsValidReason = blockchainProposal.IsValidReason;
            entity.FValid = blockchainProposal.FValid;
            return entity;
        }
        public int GetMasternodeCount()
        {
            return this.BlockchainRepository.GetMasternodeCount();
        }


        private static DateTime? ToTime(JObject responseJObject)
        {
            var resultKey = responseJObject.SelectToken("result");
            var timeKey = resultKey.SelectToken("time");
            var time = JsonConvert.DeserializeObject(timeKey?.ToString());
            var result = Convert.ToDouble(time);

            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(result).ToLocalTime();
            return dateTime;
        }
    }
}