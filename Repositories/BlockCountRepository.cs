using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZestMonitor.Api.Data.Abstract.Interfaces;
using ZestMonitor.Api.Data.Contexts;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Models;
using ZestMonitor.Api.Factories;
using ZestMonitor.Api.Helpers;
using ZestMonitor.Api.Data.Abstract.Interfaces;

namespace ZestMonitor.Api.Repositories
{
    public class BlockCountRepository : Repository<MasternodeCount>, IBlockCountRepository
    {
        private IBlockchainRepository BlockchainRepository { get; }

        public BlockCountRepository(ZestContext context, IBlockchainRepository blockchainRepository) : base(context)
        {
            this.BlockchainRepository = blockchainRepository ?? throw new ArgumentNullException(nameof(blockchainRepository));
        }
        public BlockCountRepository(ZestContext context) : base(context)
        {
        }

        public MasternodeCountJson GetMasternodeCountFromChain()
        {
            var resultKey = this.BlockchainRepository.ExecuteRPCCommand("masternode", new[] { "count" });
            var result = JsonConvert.DeserializeObject<MasternodeCountJson>(resultKey?.ToString());
            return result;
        }

        public async Task<bool> UpdatedToday()
        {
            var updatedToday = await this.FindBy(x => x.CreatedAt.Day == DateTime.Today.Day);
            if (updatedToday.Count() > 0)
                return true;
            return false;
        }

        public async Task AddMasternodeCount()
        {
            var data = this.GetMasternodeCountFromChain();
            await this.Add(data.ToEntity());
        }

        public MasternodeCount GetLatestLocalMasternodeCount()
        {
            var counts = this.GetAll();
            return counts.OrderByDescending(x => x.CreatedAt).FirstOrDefault();
        }
    }
}