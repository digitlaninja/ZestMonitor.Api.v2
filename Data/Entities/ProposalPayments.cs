namespace ZestMonitor.Api.Data.Entities
{
    public class ProposalPayments : EntityBase
    {
        public string Hash { get; set; }
        public string ShortDescription { get; set; }
        public int Amount { get; set; }
        public int ExpectedPayment { get; set; }
    }
}