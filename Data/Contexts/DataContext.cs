using Microsoft.EntityFrameworkCore;
using ZestMonitor.Api.Data.Entities;
using ZestMonitor.Api.Data.Maps;
using ZestMonitor.Api.Data.Models;

namespace ZestMonitor.Api.Data.Contexts
{
    public class ZestContext : DbContext
    {
        public ZestContext(DbContextOptions<ZestContext> options) : base(options) { }

        public DbSet<ProposalPayments> ProposalPayments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<BlockchainProposal> BlockchainProposal { get; set; }
        public DbSet<MasternodeCount> MasternodeCount { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ProposalPaymentsMap());
            builder.ApplyConfiguration(new UserMap());
            builder.ApplyConfiguration(new MasternodeCountMap());
        }
    }
}