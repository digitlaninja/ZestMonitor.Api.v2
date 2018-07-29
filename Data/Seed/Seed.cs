using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ZestMonitor.Api.Data.Contexts;
using ZestMonitor.Api.Data.Entities;

namespace ZestMonitor.Api.Data.Seed
{
    public class Seed
    {
        private ZestContext context { get; }
        public Seed(ZestContext context)
        {
            this.context = context ??
                throw new ArgumentNullException(nameof(context));
        }

        public async void ProposalPayments()
        {
            // Clear
            // this.context.RemoveRange(this.context.ProposalPayments);
            // await this.context.SaveChangesAsync();

            var json = await System.IO.File.ReadAllTextAsync("Data/Seed/ProposalPaymentsSeed.json");
            var proposalPayments = JsonConvert.DeserializeObject<List<ProposalPayments>>(json);

            foreach (var proposalPayment in proposalPayments)
            {
                await this.context.AddAsync(proposalPayment);
            }
            await this.context.SaveChangesAsync();
        }

        public bool ProposalPaymentsHasData()
        {
            return this.context.ProposalPayments.Any();
        }

    }
}