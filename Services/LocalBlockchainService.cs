using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
    public class LocalBlockchainService
    {
        private IProposalPaymentsRepository ProposalPaymentsRepository { get; }
        private ILogger<LocalBlockchainService> Logger { get; }
        private ILocalBlockchainRepository LocalBlockchainRepository { get; }
        private ProposalPaymentsService ProposalPaymentsService { get; }
        private IMasternodeCountRepository MasternodeCountRepository { get; }
        public LocalBlockchainService(ILogger<LocalBlockchainService> logger, ProposalPaymentsService proposalPaymentsService, ILocalBlockchainRepository localBlockchainRepository, IProposalPaymentsRepository proposalPaymentsRepository, IMasternodeCountRepository masternodeCountRepository)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.ProposalPaymentsService = proposalPaymentsService ?? throw new ArgumentNullException(nameof(proposalPaymentsService));
            this.LocalBlockchainRepository = localBlockchainRepository ?? throw new ArgumentNullException(nameof(localBlockchainRepository));
            this.ProposalPaymentsRepository = proposalPaymentsRepository ?? throw new ArgumentNullException(nameof(proposalPaymentsRepository));
            this.MasternodeCountRepository = masternodeCountRepository ?? throw new ArgumentNullException(nameof(masternodeCountRepository));
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

            var proposalPayment = await this.ProposalPaymentsRepository.Get(localProposal.Hash);
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

        public async Task<int> GetValidCount() => await this.LocalBlockchainRepository.GetValidCount();

        public int GetFundedCount()
        {
            var masternodeCount = this.MasternodeCountRepository.GetLatestLocalMasternodeCount().Total;
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
            var fundedCount = this.GetFundedCount();
            return new ProposalMetadataModel()
            {
                ValidProposalCount = validCount,
                FundedProposalCount = fundedCount
            };
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
            var masternodeCount = this.MasternodeCountRepository.GetLatestLocalMasternodeCount();
            if (masternodeCount == null)
                return null;

            if (blockLocalProposalMatch != null)
                model.Amount = blockchainProposal.TotalPayment == 0.0m ? blockLocalProposalMatch.Amount : Convert.ToInt32(blockchainProposal.TotalPayment) + blockLocalProposalMatch.Amount;

            model.Time = blockchainProposal.Time;
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