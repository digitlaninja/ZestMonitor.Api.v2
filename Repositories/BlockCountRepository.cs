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

namespace ZestMonitor.Api.Repositories
{
    public class BlockCountRepository : Repository<BlockCount>, IBlockCountRepository
    {
        private IBlockchainRepository BlockchainRepository { get; }

        public BlockCountRepository(ZestContext context, IBlockchainRepository blockchainRepository) : base(context)
        {
            this.BlockchainRepository = blockchainRepository ?? throw new ArgumentNullException(nameof(blockchainRepository));
        }

        public async Task<bool> UpdatedToday()
        {
            var updatedToday = await this.FindBy(x => x.CreatedAt.Day == DateTime.Today.Day);
            if (updatedToday.Count() > 0)
                return true;
            return false;
        }

        public async Task AddBlockCount()
        {
            var count = this.BlockchainRepository.GetCurrentBlockCount();
            var entity = new BlockCount()
            {
                Count = count
            };
            await this.Add(entity);
        }

        public async Task<BlockCount> GetLatestLocalBlockCount()
        {
            var counts = this.GetAll();
            return await counts.OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync();
        }
    }
}