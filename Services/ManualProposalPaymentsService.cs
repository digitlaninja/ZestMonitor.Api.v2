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
    public class ManualProposalPaymentsService
    {
        private IManualProposalPaymentsRepository ManualProposalPaymentsRepository { get; }
        public ILogger<ManualProposalPaymentsService> Logger { get; }
        public ILocalBlockchainRepository LocalBlockchainRepository { get; }

        public ManualProposalPaymentsService(IManualProposalPaymentsRepository manualProposalPaymentsRepository, ILogger<ManualProposalPaymentsService> logger, ILocalBlockchainRepository localBlockchainRepository)
        {
            this.ManualProposalPaymentsRepository = manualProposalPaymentsRepository ?? throw new ArgumentNullException(nameof(manualProposalPaymentsRepository)); ;
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.LocalBlockchainRepository = localBlockchainRepository ?? throw new ArgumentNullException(nameof(LocalBlockchainRepository));
        }

        public async Task<IEnumerable<ProposalPayments>> GetAll()
        {
            var proposals = await this.ManualProposalPaymentsRepository.GetAll()?.ToListAsync();
            return proposals;
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
                return false;

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

            var entities = await this.ManualProposalPaymentsRepository.GetAll()
                                                                    .Where(x => x.Hash == hash)
                                                                    .ToListAsync();
            entities.ForEach(x => this.ManualProposalPaymentsRepository.Remove(x));


            var result = await this.ManualProposalPaymentsRepository.SaveAll();
            if (!result)
                return false;

            return true;
        }


    }
}