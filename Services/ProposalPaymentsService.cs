using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using ZestMonitor.Api.Data.Abstract.Interfaces;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;
using ZestMonitor.Api.Extensions;
using ZestMonitor.Api.Helpers;

namespace ZestMonitor.Api.Services
{
    public class ProposalPaymentsService
    {
        private IManualProposalPaymentsRepository ManualProposalPaymentsRepository { get; }
        public ILogger<ProposalPaymentsService> Logger { get; }
        public IBlockchainRepository BlockchainRepository { get; }

        public ProposalPaymentsService(IManualProposalPaymentsRepository proposalPaymentsRepository, ILogger<ProposalPaymentsService> logger, IBlockchainRepository BlockchainRepository)
        {
            this.ManualProposalPaymentsRepository = proposalPaymentsRepository ?? throw new ArgumentNullException(nameof(proposalPaymentsRepository)); ;
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.BlockchainRepository = BlockchainRepository ?? throw new ArgumentNullException(nameof(BlockchainRepository));
        }

        public async Task<IEnumerable<ProposalPaymentsModel>> GetAll()
        {
            var proposals = await this.ManualProposalPaymentsRepository.GetAll()?.ToListAsync();
            return proposals?.ToModel();
        }

        public async Task<PagedList<ProposalPaymentsModel>> GetPaged(PagingParams pagingParams)
        {
            if (pagingParams == null)
                return null;

            var proposals = await this.ManualProposalPaymentsRepository.GetPaged(pagingParams);
            var model = proposals.ToModel();
            return model;
        }

        public async Task<ProposalPaymentsModel> Get(string hash)
        {
            var proposal = await this.ManualProposalPaymentsRepository.Get(hash);
            var model = proposal?.ToModel();
            return model;
        }

        public async Task<bool> Create(ProposalPaymentsModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));


            var entity = model.ToEntity();
            entity.CreatedAt = DateTime.Now;

            await this.ManualProposalPaymentsRepository.Add(entity);
            var result = await this.ManualProposalPaymentsRepository.SaveAll();
            if (!result)
                return false;

            return true;
        }

        public async Task<bool> Delete(string hash)
        {
            if (string.IsNullOrEmpty(hash))
                throw new ArgumentException("message", nameof(hash));

            var entities = this.ManualProposalPaymentsRepository.GetAll();
            var entity = await entities.FirstOrDefaultAsync(x => x.Hash == hash);
            this.ManualProposalPaymentsRepository.Remove(entity);

            var result = await this.ManualProposalPaymentsRepository.SaveAll();
            if (!result)
                return false;

            return true;
        }


    }
}