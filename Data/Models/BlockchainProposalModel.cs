using System;
using Newtonsoft.Json;

namespace ZestMonitor.Api.Data.Models
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
        public double Ratio { get; internal set; }
        public int RatioPercent { get; internal set; }
        public string IsEstablished { get; set; }
        public string IsValid { get; set; }
        public string IsValidReason { get; set; }
        public string FValid { get; set; }
        public DateTime? Time { get; internal set; }
    }
}