using System;

namespace WhatsNewAPI.Models
{
    public class SqlQuery
    {
        public Guid QueryId { get; set; }
        public Guid ConnectionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string QueryText { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string ConnectionName { get; set; }
    }
}
