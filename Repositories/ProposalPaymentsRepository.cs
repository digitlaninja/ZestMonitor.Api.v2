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
{
    public class ProposalPaymentsRepository : Repository<ProposalPayments>, IProposalPaymentsRepository
    {
        public ProposalPaymentsRepository(ZestContext context) : base(context) { }

        public async Task<PagedList<ProposalPayments>> GetPaged(PagingParams pagingParams)
        {
            if (pagingParams == null)
                throw new System.ArgumentNullException(nameof(pagingParams));

            var entities = this.GetAll();
            if (entities.Count() <= 0 || entities == null)
                return new PagedList<ProposalPayments>();

            return await PagedList<ProposalPayments>.CreateAsync(entities, pagingParams.PageNumber, pagingParams.PageSize);
            // return entities.Skip((page - 1) * 10).Take(limit);
        }
    }
}