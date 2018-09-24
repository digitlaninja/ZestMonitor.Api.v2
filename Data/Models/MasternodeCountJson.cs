using System;
using Newtonsoft.Json;

namespace ZestMonitor.Api.Data.Models
{
    public class MasternodeCountJson
    {
        public int Total { get; set; }
        public int Stable { get; set; }
        public int ObfCompat { get; set; }
        public int Enabled { get; set; }
        public int InQueue { get; set; }
        public int IPv4 { get; set; }
        public int IPv6 { get; set; }
        public int Onion { get; set; }
    }
}