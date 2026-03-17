using System;

namespace WhatsNewAPI.Models
{
    public class Tag
    {
        public Guid TagId { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
