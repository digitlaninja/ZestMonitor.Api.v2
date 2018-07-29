using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZestMonitor.Api.Data.Abstract.Interfaces;
using ZestMonitor.Api.Data.Contexts;
using ZestMonitor.Api.Data.Entities;

namespace ZestMonitor.Api.Repositories
{
    public class ProposalPaymentsRepository : Repository<ProposalPayments>, IProposalPaymentsRepository
    {
        public ProposalPaymentsRepository(ZestContext context) : base(context) { }

        public async Task<IEnumerable<ProposalPayments>> GetPaged(int pageIndex, int pageSize)
        {
            var entities = await this.GetAll();
            if (entities.Count() <= 0 || entities == null)
                return new List<ProposalPayments>();

            return entities.Skip((pageIndex - 1) * 10).Take(pageSize).ToList();
        }
    }
}