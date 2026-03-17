using System;
using System.Collections.Generic;

namespace WhatsNewAPI.Models
{
    public class Change
    {
        public Guid ChangeId { get; set; }
        public Guid ReleaseId { get; set; }
        public string Description { get; set; }
        public string ChangeType { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Guid> TagIds { get; set; } = new List<Guid>();
        public Guid? ClientId { get; set; }
        public string TicketNumber { get; set; }
        public string DevOpsNumber { get; set; }
    }
}