using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            var blockchainProposals = await this.BlockchainRepository.GetLocalPagedBlockchainProposals();
            var localProposals = await this.ProposalPaymentsService.GetAll();
            var t = await this.ConstructCompleteleProposalData(pagingParams);

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

        public async Task<PagedList<BlockchainProposalModel>> ConstructCompleteleProposalData(PagingParams pagingParams)
        {
            var viewModel = new List<BlockchainProposalModel>();
            var blockchainProposals = this.BlockchainRepository.GetPagedProposals(pagingParams);
            var localProposals = await this.ProposalPaymentsService.GetAll();

            foreach (var blockchainProposal in blockchainProposals)
            {
                var model = new BlockchainProposalModel();
                var matchingProposal = localProposals.OrderByDescending(x => x.DateCreated).FirstOrDefault(x => x.Hash == blockchainProposal.Hash);

                if (matchingProposal != null)
                    model.Amount = matchingProposal.Amount;

                model.Time = this.BlockchainRepository.GetTime(blockchainProposal.Hash);
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

        public BlockchainProposalJson GetProposal(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            return this.BlockchainRepository.GetProposal(name);
        }

        public int GetValidCount()
        {
            return this.BlockchainRepository.GetValidCount();
        }

        public int GetFundedCount()
        {
            return this.BlockchainRepository.GetFundedCount();
        }

        public ProposalMetadataModel GetProposalMetadata()
        {
            return this.BlockchainRepository.GetMetadata();
        }
    }
}