using Newtonsoft.Json;

namespace ZestMonitor.Api.Data.Models
{
    public class BlockchainProposalJson
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("feeHash")]
        public string FeeHash { get; set; }

        [JsonProperty("yeas")]
        public int Yeas { get; set; }

        [JsonProperty("nays")]
        public int Nays { get; set; }

        [JsonProperty("abstains")]
        public int Abstains { get; set; }

        [JsonProperty("isEstablished")]
        public bool IsEstablished { get; set; }

        [JsonProperty("isValid")]
        public bool IsValid { get; set; }

        [JsonProperty("isValidReason")]
        public string IsValidReason { get; set; }

        [JsonProperty("fValid")]
        public bool FValid { get; set; }

        [JsonProperty("ratio")]
        public double Ratio { get; set; }

        [JsonProperty("totalPayment")]
        public decimal TotalPayment { get; set; }

        [JsonProperty("totalPaymentCount")]
        public int TotalPaymentCount { get; set; }

        [JsonProperty("remainingPaymentCount")]
        public int RemainingPaymentCount { get; set; }

        [JsonProperty("monthlyPayment")]
        public decimal MonthlyPayment { get; set; }

        [JsonProperty("blockStart")]
        public int BlockStart { get; set; }

        [JsonProperty("blockEnd")]
        public int BlockEnd { get; set; }
    }
}