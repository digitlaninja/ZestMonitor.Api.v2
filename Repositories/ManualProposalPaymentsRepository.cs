using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZestMonitor.Api.Data.Abstract.Interfaces;
using ZestMonitor.Api.Data.Contexts;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;
using ZestMonitor.Api.Helpers;

namespace ZestMonitor.Api.Repositories
{   // Deals with Manually entered Proposals payments from CMS
    public class ManualProposalPaymentsRepository : Repository<ProposalPayments>, IManualProposalPaymentsRepository
    {
        public ManualProposalPaymentsRepository(ZestContext context) : base(context) { }

        public async Task<PagedList<ProposalPayments>> GetPaged(PagingParams pagingParams)
        {
            if (pagingParams == null)
                throw new System.ArgumentNullException(nameof(pagingParams));

            var query = this.GetAll();
            var entities = query.OrderByDescending(x => x.CreatedAt);
            if (entities.Count() <= 0 || entities == null)
                return new PagedList<ProposalPayments>();

            return await PagedList<ProposalPayments>.CreateAsync(entities, pagingParams.PageNumber, pagingParams.PageSize);
        }

        public async Task<ProposalPayments> Get(string hash)
        {
            var query = this.GetAll();
            var entity = await query.FirstOrDefaultAsync(x => x.Hash == hash);
            return entity;
        }

    }
}