using System.Collections.Generic;
using System.Threading.Tasks;
using ZestMonitor.Api.Data.Entities;

namespace ZestMonitor.Api.Data.Abstract.Interfaces
{
    public interface IProposalPaymentsRepository : IRepository<ProposalPayments>
    {
        Task<IEnumerable<ProposalPayments>> GetPaged(int pageIndex, int pageSize);
    }
}