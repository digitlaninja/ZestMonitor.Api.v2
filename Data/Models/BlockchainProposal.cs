using Newtonsoft.Json;

namespace ZestMonitor.Api.Data.Models
{
    public class BlockchainProposal
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
        public long Yeas { get; set; }

        [JsonProperty("nays")]
        public long Nays { get; set; }

        [JsonProperty("abstains")]
        public long Abstains { get; set; }

        [JsonProperty("isEstablished")]
        public bool IsEstablished { get; set; }

        [JsonProperty("isValid")]
        public bool IsValid { get; set; }

        [JsonProperty("isValidReason")]
        public string IsValidReason { get; set; }

        [JsonProperty("fValid")]
        public bool FValid { get; set; }
    }
}