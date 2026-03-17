using System;
using System.Collections.Generic;

namespace WhatsNewAPI.DTOs
{
    public class ChangeDto
    {
        public Guid ChangeId { get; set; }
        public Guid ReleaseId { get; set; }
        public string Description { get; set; }
        public string ChangeType { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Guid> TagIds { get; set; } = new List<Guid>();
        public List<string> ModuleTags { get; set; } = new List<string>();
        public Guid? ClientId { get; set; }
        public string TicketNumber { get; set; }
        public string DevOpsNumber { get; set; }
    }

    public class CreateChangeDto
    {
        public Guid ReleaseId { get; set; }
        public string Description { get; set; }
        public string ChangeType { get; set; }
        public List<Guid> TagIds { get; set; } = new List<Guid>();
        public Guid? ClientId { get; set; }
        public string TicketNumber { get; set; }
        public string DevOpsNumber { get; set; }
    }

    public class UpdateChangeDto
    {
        public string Description { get; set; }
        public string ChangeType { get; set; }
        public List<Guid> TagIds { get; set; } = new List<Guid>();
        public Guid? ClientId { get; set; }
        public string TicketNumber { get; set; }
        public string DevOpsNumber { get; set; }
    }
}