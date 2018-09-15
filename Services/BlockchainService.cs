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
        public ILocalBlockchainRepository LocalBlockchainRepository { get; }
        public ProposalPaymentsService ProposalPaymentsService { get; }

        public BlockchainService(ILogger<BlockchainService> logger, IBlockchainRepository BlockchainRepository, ProposalPaymentsService proposalPaymentsService, ILocalBlockchainRepository localBlockchainRepository)
        {
            this.Logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this.BlockchainRepository = BlockchainRepository ?? throw new ArgumentNullException(nameof(BlockchainRepository));
            this.ProposalPaymentsService = proposalPaymentsService ?? throw new ArgumentNullException(nameof(proposalPaymentsService));
            this.LocalBlockchainRepository = localBlockchainRepository ?? throw new ArgumentNullException(nameof(localBlockchainRepository));
        }

        public async Task<PagedList<BlockchainProposalModel>> GetPagedProposals(PagingParams pagingParams)
        {
            var viewModel = new List<BlockchainProposalModel>();
            var blockchainProposals = await this.LocalBlockchainRepository.GetProposals();
            var localProposals = await this.ProposalPaymentsService.GetAll();
            var result = await this.CreateBlockchainProposalPagedList(pagingParams, blockchainProposals, localProposals);

            if (result.Count <= 0 || result == null)
                return null;

            return result;
        }

        public async Task<BlockchainProposal> GetLocalProposal(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            return await this.LocalBlockchainRepository.GetProposal(name);
        }

        public int GetValidCount() => this.BlockchainRepository.GetValidCount();

        public int GetFundedCount() => this.BlockchainRepository.GetFundedCount();

        public ProposalMetadataModel GetProposalMetadata() => this.BlockchainRepository.GetMetadata();

        public async Task<PagedList<BlockchainProposalModel>> CreateBlockchainProposalPagedList(PagingParams pagingParams, IEnumerable<BlockchainProposal> blockchainProposals, IEnumerable<ProposalPaymentsModel> localProposals)
        {
            if (pagingParams == null || blockchainProposals == null || localProposals == null)
                return null;

            var viewModel = new List<BlockchainProposalModel>();
            foreach (var blockchainProposal in blockchainProposals)
            {
                var model = this.CreateBlockchainProposalModel(localProposals, blockchainProposal);
                viewModel.Add(model);
            }

            var pagedProposals = PagedList<BlockchainProposalModel>.CreateAsync(viewModel, pagingParams.PageNumber, pagingParams.PageSize);
            return pagedProposals;
        }

        private BlockchainProposalModel CreateBlockchainProposalModel(IEnumerable<ProposalPaymentsModel> localProposals, BlockchainProposal blockchainProposal)
        {
            if (localProposals == null || blockchainProposal == null)
                return null;

            var model = new BlockchainProposalModel();
            var blockLocalProposalMatch = localProposals.OrderByDescending(x => x.DateCreated).FirstOrDefault(x => x.Hash == blockchainProposal.Hash);

            if (blockLocalProposalMatch != null)
                model.Amount = blockLocalProposalMatch.Amount;

            model.Time = !string.IsNullOrEmpty(blockchainProposal.FeeHash) ? this.BlockchainRepository.GetTime(blockchainProposal.FeeHash) : null;
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