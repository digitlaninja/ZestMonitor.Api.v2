using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;
using ZestMonitor.Api.Helpers;

namespace ZestMonitor.Api.Data.Abstract.Interfaces
{
    public interface IBlockchainRepository
    {
        IEnumerable<BlockchainProposalJson> GetProposals();
        BlockchainProposalJson GetProposal(string name);
        DateTime? GetTime(string hash);
        JToken ExecuteRPCCommand(string command, params object[] parameters);
        int GetCurrentBlockCount();
    }
}