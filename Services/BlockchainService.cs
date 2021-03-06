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
using ZestMonitor.Api.Repositories;

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
        public IBlockCountRepository BlockCountRepository { get; set; }
        public IConfiguration IConfiguration { get; }

        public BlockchainService(ILogger<BlockchainService> logger, IBlockchainRepository BlockchainRepository, ProposalPaymentsService proposalPaymentsService, ILocalBlockchainRepository localBlockchainRepository, IProposalPaymentsRepository proposalPaymentsRepository, IMasternodeCountRepository masternodeCountRepository, IBlockCountRepository blockCountRepository, IConfiguration iConfiguration)
        {
            this.Logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this.BlockchainRepository = BlockchainRepository ?? throw new ArgumentNullException(nameof(BlockchainRepository));
            this.ProposalPaymentsService = proposalPaymentsService ?? throw new ArgumentNullException(nameof(proposalPaymentsService));
            this.proposalPaymentsRepository = proposalPaymentsRepository ?? throw new ArgumentNullException(nameof(proposalPaymentsRepository));
            this.LocalBlockchainRepository = localBlockchainRepository ?? throw new ArgumentNullException(nameof(localBlockchainRepository));
            this.MasternodeCountRepository = masternodeCountRepository ?? throw new ArgumentNullException(nameof(masternodeCountRepository));
            this.BlockCountRepository = blockCountRepository ?? throw new ArgumentNullException(nameof(blockCountRepository));
            this.IConfiguration = iConfiguration ?? throw new ArgumentNullException(nameof(iConfiguration));
        }

        public async Task SaveBlockchainData()
        {
            await this.SaveMasternodeCount();
            await this.SaveProposals();
            await this.SaveBlockCount();
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

            var masternodeCount = await this.MasternodeCountRepository.GetLatestLocalMasternodeCount();
            if (masternodeCount == null)
                return;

            var proposalPayments = await this.ProposalPaymentsService.GetAll();
            if (proposalPayments.Count() <= 0 || proposalPayments == null)
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

        public async Task SaveBlockCount()
        {
            var updatedToday = await this.BlockCountRepository.UpdatedToday();
            if (!updatedToday)
                await this.BlockCountRepository.AddBlockCount();

            if (this.LocalBlockchainRepository.AnyTracked())
                await this.BlockCountRepository.SaveAll();
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
            entity.TotalPayment = blockchainProposal.TotalPayment;
            entity.TotalPaymentCount = blockchainProposal.TotalPaymentCount;
            entity.RemainingPaymentCount = blockchainProposal.RemainingPaymentCount;
            entity.MonthlyPayment = blockchainProposal.MonthlyPayment;
            entity.BlockStart = blockchainProposal.BlockStart;
            entity.BlockEnd = blockchainProposal.BlockEnd;
            entity.UpdatedAt = DateTime.Now;
            return entity;
        }

        private bool CalculateIsFunded(BlockchainProposal blockchainProposal, int masternodeCount)
        {
            if (masternodeCount <= 0)
                return false;

            var fundedThreshold = this.IConfiguration.GetValue<int>("FundedThreshold");
            return (blockchainProposal.Yeas - blockchainProposal.Nays) / masternodeCount > fundedThreshold ? true : false;
        }
    }
}