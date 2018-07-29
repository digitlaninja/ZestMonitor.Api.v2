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

        public async Task<IEnumerable<ProposalPaymentsModel>> GetProposals()
        {
            var proposals = await this.ProposalPaymentsRepository.GetAll();
            return proposals.ToModel();
        }

        public async void Create(ProposalPaymentsModel model)
        {
            throw new DbUpdateException("could not update", new ArgumentException());
            // if (model == null)
            //     throw new ArgumentNullException(nameof(model));

            // var entity = model.ToEntity();
            // this.ProposalPaymentsRepository.Add(entity);
        }

    }
}