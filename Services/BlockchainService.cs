using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ZestMonitor.Api.Data.Abstract.Interfaces;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;
using ZestMonitor.Api.Helpers;

namespace ZestMonitor.Api.Services
{
    public class BlockchainService
    {
        public ILogger<BlockchainService> Logger { get; }
        public IBlockchainRepository BlockchainRepository { get; }
        public ProposalPaymentsService ProposalPaymentsService { get; }

        public BlockchainService(ILogger<BlockchainService> logger, IBlockchainRepository BlockchainRepository, ProposalPaymentsService proposalPaymentsService)
        {
            this.Logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this.BlockchainRepository = BlockchainRepository ?? throw new ArgumentNullException(nameof(BlockchainRepository));
            this.ProposalPaymentsService = proposalPaymentsService ?? throw new ArgumentNullException(nameof(proposalPaymentsService));
        }

        public async Task<PagedList<BlockchainProposalModel>> GetPagedProposals(PagingParams pagingParams)
        {
            var viewModel = new List<BlockchainProposalModel>();
            var blockchainProposals = await this.BlockchainRepository.GetPagedProposals(pagingParams);
            var localProposals = await this.ProposalPaymentsService.GetAll();

            foreach (var blockchainProposal in blockchainProposals)
            {
                var model = new BlockchainProposalModel();
                var matchingProposal = localProposals
                                        .OrderByDescending(x => x.DateCreated)
                                        .FirstOrDefault(x => x.Hash == blockchainProposal.Hash);

                if (matchingProposal != null)
                    model.Amount = matchingProposal.Amount;

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

                viewModel.Add(model);
            }
            var pagedProposals = PagedList<BlockchainProposalModel>.CreateAsync(viewModel, pagingParams.PageNumber, pagingParams.PageSize);
            return pagedProposals;
        }

        public async Task<BlockchainProposalJson> GetProposal(string name)
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

        public async Task<ProposalMetadataModel> GetProposalMetadata()
        {
            return await this.BlockchainRepository.GetMetadata();
        }
    }
}