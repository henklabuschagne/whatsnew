using System;
using System.Collections.Generic;

namespace WhatsNewAPI.Models
{
    public class Release
    {
        public Guid ReleaseId { get; set; }
        public string Version { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<Change> Changes { get; set; } = new List<Change>();
    }
}
