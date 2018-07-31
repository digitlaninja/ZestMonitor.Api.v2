using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZestMonitor.Api.Data.Entities;

namespace ZestMonitor.Api.Data.Maps
{
   public class ProposalPaymentsMap : IEntityTypeConfiguration<ProposalPayments>
{
    public void Configure(EntityTypeBuilder<ProposalPayments> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).IsRequired();
        builder.Property(x => x.Hash).IsRequired();
        builder.Property(x => x.ExpectedPayment).IsRequired();
        builder.Property(x => x.ShortDescription);
        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.UpdatedAt);
    }
}
}