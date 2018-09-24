using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZestMonitor.Api.Data.Entities;

namespace ZestMonitor.Api.Data.Maps
{
    public class MasternodeCountMap : IEntityTypeConfiguration<MasternodeCount>
    {
        public void Configure(EntityTypeBuilder<MasternodeCount> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Total);
            builder.Property(x => x.Stable);
            builder.Property(x => x.ObfCompat);
            builder.Property(x => x.Enabled);
            builder.Property(x => x.InQueue);
            builder.Property(x => x.IPv4);
            builder.Property(x => x.IPv6);
            builder.Property(x => x.Onion);
        }
    }
}