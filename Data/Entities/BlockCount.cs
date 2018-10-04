using System;
using Newtonsoft.Json;

namespace ZestMonitor.Api.Data.Entities
{
    public class BlockCount : EntityBase
    {
        public string Count { get; set; }
    }
}