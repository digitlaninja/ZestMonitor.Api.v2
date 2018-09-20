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
        public ILogger<LocalBlockchainService> Logger { get; }
        public ILocalBlockchainRepository LocalBlockchainRepository { get; }
        public ProposalPaymentsService ProposalPaymentsService { get; }

        public LocalBlockchainService(ILogger<LocalBlockchainService> logger, IBlockchainRepository BlockchainRepository, ProposalPaymentsService proposalPaymentsService, ILocalBlockchainRepository localBlockchainRepository, IProposalPaymentsRepository proposalPaymentsRepository)
        {
            this.Logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this.ProposalPaymentsService = proposalPaymentsService ?? throw new ArgumentNullException(nameof(proposalPaymentsService));
            this.LocalBlockchainRepository = localBlockchainRepository ?? throw new ArgumentNullException(nameof(localBlockchainRepository));
            this.ProposalPaymentsRepository = proposalPaymentsRepository ?? throw new ArgumentNullException(nameof(proposalPaymentsRepository));
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

        public async Task<BlockchainProposalModel> GetLocalProposal(string name)
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

        public async Task<int> GetValidCount() => await this.LocalBlockchainRepository.GetValidCount();

        public async Task<int> GetFundedCount() => await this.LocalBlockchainRepository.GetFundedCount();

        public async Task<ProposalMetadataModel> GetProposalMetadata() => await this.LocalBlockchainRepository.GetMetadata();
        #endregion

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

            if (blockLocalProposalMatch != null)
                model.Amount = blockLocalProposalMatch.Amount;

            model.Time = blockchainProposal.Time;
            model.Name = blockchainProposal.Name;
            model.Url = blockchainProposal.Url;
            model.Hash = blockchainProposal.Hash;
            model.FeeHash = blockchainProposal.FeeHash;
            model.Yeas = blockchainProposal.Yeas;
            model.Nays = blockchainProposal.Nays;
            model.Abstains = blockchainProposal.Abstains;
            model.Ratio = Math.Round(blockchainProposal.Ratio, 2);
            model.IsEstablished = blockchainProposal.IsEstablished ? "Yes" : "No";
            model.IsValid = blockchainProposal.IsValid ? "Yes" : "No";
            model.IsValidReason = blockchainProposal.IsValidReason;
            model.FValid = blockchainProposal.FValid ? "Yes" : "No";
            return model;
        }
    }
}