using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
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
        public IMasternodeCountRepository MasternodeCountRepository { get; private set; }
        public IConfiguration IConfiguration { get; }

        public BlockchainService(ILogger<BlockchainService> logger, IBlockchainRepository BlockchainRepository, ProposalPaymentsService proposalPaymentsService, ILocalBlockchainRepository localBlockchainRepository, IProposalPaymentsRepository proposalPaymentsRepository, IMasternodeCountRepository masternodeCountRepository, IConfiguration iConfiguration)
        {
            this.Logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this.BlockchainRepository = BlockchainRepository ?? throw new ArgumentNullException(nameof(BlockchainRepository));
            this.ProposalPaymentsService = proposalPaymentsService ?? throw new ArgumentNullException(nameof(proposalPaymentsService));
            this.proposalPaymentsRepository = proposalPaymentsRepository ?? throw new ArgumentNullException(nameof(proposalPaymentsRepository));
            this.LocalBlockchainRepository = localBlockchainRepository ?? throw new ArgumentNullException(nameof(localBlockchainRepository));
            this.MasternodeCountRepository = masternodeCountRepository ?? throw new ArgumentNullException(nameof(masternodeCountRepository));
            this.IConfiguration = iConfiguration ?? throw new ArgumentNullException(nameof(iConfiguration));
        }

        public async Task SaveBlockchainData()
        {
            await this.SaveMasternodeCount();
            await this.SaveProposals();
        }

        private async Task SaveMasternodeCount()
        {
            var updatedToday = await this.MasternodeCountRepository.UpdatedToday();
            if (!updatedToday)
                await this.MasternodeCountRepository.AddMasternodeCount();

            if (this.LocalBlockchainRepository.AnyTracked())
                await this.LocalBlockchainRepository.SaveAll();
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
            var masternodeCount = this.MasternodeCountRepository.GetLatestLocalMasternodeCount();
            if (proposalPayments.Count() <= 0 || proposalPayments == null)
                return;
            if (masternodeCount == null)
                return;

            foreach (var blockchainProposal in blockchainProposals)
            {
                var existingProposal = localBlockchainProposals?.FirstOrDefault(x => x.Name == blockchainProposal.Name);
                var newProposal = existingProposal == null;
                if (newProposal)
                {
                    // Create full blockchain proposal
                    var completeBlockchainProposal = this.ConstructFullBlockchainProposal(proposalPayments, blockchainProposal, masternodeCount.Total);
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

            entity.Time = !string.IsNullOrEmpty(blockchainProposal.FeeHash) ? this.BlockchainRepository.GetTime(blockchainProposal.FeeHash) : null;
            entity.Name = blockchainProposal.Name;
            entity.Url = blockchainProposal.Url;
            entity.Hash = blockchainProposal.Hash;
            entity.FeeHash = blockchainProposal.FeeHash;
            entity.Yeas = blockchainProposal.Yeas;
            entity.Nays = blockchainProposal.Nays;
            entity.Abstains = blockchainProposal.Abstains;
            entity.Ratio = blockchainProposal.Ratio;
            entity.IsEstablished = blockchainProposal.IsEstablished;
            entity.IsValid = blockchainProposal.IsValid;
            entity.IsValidReason = blockchainProposal.IsValidReason;
            entity.IsFunded = this.CalculateIsFunded(blockchainProposal, masternodeCount);
            entity.FValid = blockchainProposal.FValid;
            return entity;
        }


        // TODO: Get the "5" from config
        private bool CalculateIsFunded(BlockchainProposal blockchainProposal, int masternodeCount)
        {
            var fundedThreshold = this.IConfiguration.GetValue<int>("FundedThreshold");
            return (blockchainProposal.Yeas - blockchainProposal.Nays) / masternodeCount > fundedThreshold ? true : false;
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