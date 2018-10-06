using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZestMonitor.Api.Data.Entities;

namespace ZestMonitor.Api.Data.Maps
{
    public class BlockCountMap : IEntityTypeConfiguration<BlockCount>
    {
        public void Configure(EntityTypeBuilder<BlockCount> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Count);
            builder.Property(x => x.CreatedAt);
            builder.Property(x => x.UpdatedAt);
        }
    }
}