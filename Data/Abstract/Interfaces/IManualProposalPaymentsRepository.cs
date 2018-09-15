using System.Collections.Generic;
using System.Threading.Tasks;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;
using ZestMonitor.Api.Helpers;

namespace ZestMonitor.Api.Data.Abstract.Interfaces
{
    public interface IManualProposalPaymentsRepository : IRepository<ProposalPayments>
    {
        Task<PagedList<ProposalPayments>> GetPaged(PagingParams pagingParams);
        Task<ProposalPayments> Get(string hash);
    }
}