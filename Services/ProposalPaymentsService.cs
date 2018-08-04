using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZestMonitor.Api.Data.Abstract.Interfaces;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;

namespace ZestMonitor.Api.Services
{
    public class ProposalPaymentsService
    {
        private IProposalPaymentsRepository ProposalPaymentsRepository { get; }

        public ProposalPaymentsService(IProposalPaymentsRepository proposalPaymentsRepository)
        {
            if (proposalPaymentsRepository == null) throw new ArgumentNullException(nameof(proposalPaymentsRepository));
            this.ProposalPaymentsRepository = proposalPaymentsRepository;
        }

        public async Task<IEnumerable<ProposalPaymentsModel>> GetAll()
        {
            var proposals = await this.ProposalPaymentsRepository.GetAll();
            return proposals.ToModel();
        }

        public async Task<IEnumerable<ProposalPaymentsModel>> GetPaged(int page = 1, int limit = 10)
        {
            var proposals = await this.ProposalPaymentsRepository.GetPaged(page, limit);
            return proposals.ToModel();
        }

        public async Task<bool> Create(ProposalPaymentsModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            
            var entity = model.ToEntity();
            entity.CreatedAt = DateTime.Now;
            
            await this.ProposalPaymentsRepository.Add(entity);

            var result = await this.ProposalPaymentsRepository.SaveAll();
            if (!result)
                return false;

            return true;
        }

    }
}