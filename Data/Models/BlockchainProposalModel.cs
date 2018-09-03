using Newtonsoft.Json;

namespace ZestMonitor.Api.Data.Entities
{
    public class BlockchainProposalModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Hash { get; set; }
        public string FeeHash { get; set; }
        public int Amount { get; set; }
        public int Yeas { get; set; }
        public int Nays { get; set; }
        public int Abstains { get; set; }
        public bool IsEstablished { get; set; }
        public bool IsValid { get; set; }
        public string IsValidReason { get; set; }
        public bool FValid { get; set; }
    }
}