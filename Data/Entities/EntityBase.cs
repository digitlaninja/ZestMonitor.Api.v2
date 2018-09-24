using System;

namespace ZestMonitor.Api.Data.Entities
{
    public class EntityBase
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public EntityBase()
        {
            this.UpdatedAt = DateTime.Now;
            this.CreatedAt = DateTime.Now;
        }
    }
}