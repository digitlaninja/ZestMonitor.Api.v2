using System.Collections.Generic;
using System.Threading.Tasks;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;
using ZestMonitor.Api.Helpers;

namespace ZestMonitor.Api.Data.Abstract.Interfaces
{
    public interface IProposalPaymentsRepository : IRepository<ProposalPayments>
    {
        Task<PagedList<ProposalPayments>> GetPaged(PagingParams pagingParams);
        Task<ProposalPayments> GetLatest(string hash);
    }
}