using System;
using Newtonsoft.Json;

namespace ZestMonitor.Api.Data.Entities
{
    public class BlockCount : EntityBase
    {
        public int Count { get; set; }
    }
}