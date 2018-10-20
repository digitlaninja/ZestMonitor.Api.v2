using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
    public class LocalBlockchainService
    {
        private IProposalPaymentsRepository ProposalPaymentsRepository { get; }
        private ILogger<LocalBlockchainService> Logger { get; }
        private ILocalBlockchainRepository LocalBlockchainRepository { get; }
        private ProposalPaymentsService ProposalPaymentsService { get; }
        private IMasternodeCountRepository MasternodeCountRepository { get; }
        private IBlockCountRepository BlockCountRepository { get; }
        public IConfiguration IConfiguration { get; }
        public LocalBlockchainService(ILogger<LocalBlockchainService> logger, ProposalPaymentsService proposalPaymentsService, ILocalBlockchainRepository localBlockchainRepository, IProposalPaymentsRepository proposalPaymentsRepository, IMasternodeCountRepository masternodeCountRepository, IBlockCountRepository blockCountRepository, IConfiguration iConfiguration)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.ProposalPaymentsService = proposalPaymentsService ?? throw new ArgumentNullException(nameof(proposalPaymentsService));
            this.LocalBlockchainRepository = localBlockchainRepository ?? throw new ArgumentNullException(nameof(localBlockchainRepository));
            this.ProposalPaymentsRepository = proposalPaymentsRepository ?? throw new ArgumentNullException(nameof(proposalPaymentsRepository));
            this.MasternodeCountRepository = masternodeCountRepository ?? throw new ArgumentNullException(nameof(masternodeCountRepository));
            this.BlockCountRepository = blockCountRepository ?? throw new ArgumentNullException(nameof(blockCountRepository));
            this.IConfiguration = iConfiguration ?? throw new ArgumentNullException(nameof(iConfiguration));
        }

        #region Get
        public async Task<PagedList<BlockchainProposalModel>> GetPagedProposals(PagingParams pagingParams)
        {
            var viewModel = new List<BlockchainProposalModel>();
            var blockchainProposals = await this.LocalBlockchainRepository.GetProposals();
            var localProposals = this.ProposalPaymentsRepository.GetAll();

            var result = await this.CreateBlockchainProposalPagedList(pagingParams, blockchainProposals, localProposals);

            if (result.Count <= 0 || result == null)
                return null;

            return result;
        }

        public async Task<BlockchainProposalModel> GetProposal(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            var localProposal = (await this.LocalBlockchainRepository.GetProposal(name))?.ToModel();
            if (localProposal == null)
                return null;

            var proposalPayment = await this.ProposalPaymentsRepository.GetLatest(localProposal.Hash);
            if (proposalPayment != null)
                localProposal.Amount = proposalPayment.Amount;

            return localProposal;
        }
        public int GetMasternodeCount()
        {
            var data = this.MasternodeCountRepository.GetMasternodeCountFromChain();
            if (data == null)
                return -1;

            return data.Total;
        }

        private async Task<int> GetValidCount() => await this.LocalBlockchainRepository.GetValidCount();

        private async Task<int> GetFundedCount()
        {
            var masternodeCount = (await this.MasternodeCountRepository.GetLatestLocalMasternodeCount()).Total;
            var proposals = this.LocalBlockchainRepository.GetAll();
            if (proposals == null)
                return -1;

            var fundedCount = 0;
            foreach (var proposal in proposals)
            {
                var netVotePercent = (double)(proposal.Yeas - proposal.Nays) / (double)masternodeCount;
                if (netVotePercent > 5)
                    fundedCount++;
            }
            return fundedCount;
        }

        public async Task<ProposalMetadataModel> GetProposalMetadata()
        {
            var validCount = await this.GetValidCount();
            var fundedCount = await this.GetFundedCount();
            var fundedAmount = await this.ProposalPaymentsService.GetFundedAmountTotal();

            var daysLeft = await this.GetDaysLeft();
            var deadline = this.GetDeadline(daysLeft);

            return new ProposalMetadataModel()
            {
                ValidProposalCount = validCount,
                FundedProposalCount = fundedCount,
                FundedProposalAmount = fundedAmount,
                DaysLeft = (daysLeft == null || daysLeft <= 0) ? null : daysLeft,
                VoteDeadline = deadline?.ToString()
            };
        }

        private DateTime? GetDeadline(double? daysLeft)
        {
            if (daysLeft <= 0 || daysLeft == null)
                return null;

            try
            {
                var dt = DateTime.Now;
                return dt.AddDays((double)daysLeft);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex.Message);
                return null;
            }
        }

        private async Task<double?> GetDaysLeft()
        {
            var latestProposal = await this.LocalBlockchainRepository.GetLatest();
            if (latestProposal == null)
                return null;

            var latestBlockCount = await this.BlockCountRepository.GetLatestLocalBlockCount();
            if (latestBlockCount == null)
                return null;

            var averageBlocksPerDay = this.IConfiguration.GetValue<int>("AvgBlocksPerDay");
            var result = ((double)latestProposal.BlockStart - (double)latestBlockCount.Count) / (double)averageBlocksPerDay;
            return Math.Round(result, 2);
        }

        private DateTime? MillisecondsToTime(int milliseconds)
        {
            var ms = Convert.ToDouble(milliseconds);
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            var result = dateTime.AddSeconds(ms);
            return result;
        }

        #endregion

        #region Create
        public async Task<PagedList<BlockchainProposalModel>> CreateBlockchainProposalPagedList(PagingParams pagingParams, IEnumerable<BlockchainProposal> blockchainProposals, IQueryable<ProposalPayments> localProposals)
        {
            var proposals = await this.CreateBlockchainProposalList(pagingParams, blockchainProposals, localProposals);
            var pagedProposals = PagedList<BlockchainProposalModel>.Create(proposals.ToList(), pagingParams.PageNumber, pagingParams.PageSize);
            return pagedProposals;
        }

        // Constructs list of complete blockchain proposals
        public async Task<List<BlockchainProposalModel>> CreateBlockchainProposalList(PagingParams pagingParams, IEnumerable<BlockchainProposal> blockchainProposals, IQueryable<ProposalPayments> localProposals)
        {
            if (pagingParams == null || blockchainProposals == null || localProposals == null)
                return null;

            var proposals = new List<BlockchainProposalModel>();
            foreach (var blockchainProposal in blockchainProposals)
            {
                var model = await this.ConstructBlockchainProposalModel(localProposals, blockchainProposal);
                proposals.Add(model);
            }
            return proposals;
        }

        // Builds a complete blockchain proposal 
        // with local, manually-entered payment amount and time (for ui)
        private async Task<BlockchainProposalModel> ConstructBlockchainProposalModel(IQueryable<ProposalPayments> localProposals, BlockchainProposal blockchainProposal)
        {
            if (localProposals == null || blockchainProposal == null)
                return null;

            var model = new BlockchainProposalModel();

            var blockLocalProposalMatch = await localProposals.OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync(x => x.Hash == blockchainProposal.Hash);
            var masternodeCount = await this.MasternodeCountRepository.GetLatestLocalMasternodeCount();
            if (masternodeCount == null)
                return null;

            if (blockLocalProposalMatch != null)
                model.Amount = blockchainProposal.TotalPayment == 0.0m ? blockLocalProposalMatch.Amount : Convert.ToInt32(blockchainProposal.TotalPayment) + blockLocalProposalMatch.Amount;

            model.Time = blockchainProposal.Time.ToString();
            model.Name = blockchainProposal.Name;
            model.Url = blockchainProposal.Url;
            model.Hash = blockchainProposal.Hash;
            model.FeeHash = blockchainProposal.FeeHash;
            model.Yeas = blockchainProposal.Yeas;
            model.Nays = blockchainProposal.Nays;
            model.Abstains = blockchainProposal.Abstains;
            model.Ratio = blockchainProposal.Ratio;
            model.IsEstablished = blockchainProposal.IsEstablished ? "Yes" : "No";
            model.IsValid = blockchainProposal.IsValid ? "Yes" : "No";
            model.IsFunded = blockchainProposal.IsFunded ? "Yes" : "No";
            model.IsValidReason = blockchainProposal.IsValidReason;
            model.FValid = blockchainProposal.FValid ? "Yes" : "No";
            model.TotalPayment = blockchainProposal.TotalPayment;
            return model;
        }
        #endregion
    }
}