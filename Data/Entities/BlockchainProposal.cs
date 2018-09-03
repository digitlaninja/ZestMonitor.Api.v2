using Newtonsoft.Json;

namespace ZestMonitor.Api.Data.Entities
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
    }
}