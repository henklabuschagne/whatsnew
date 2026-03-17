using System;

namespace WhatsNewAPI.DTOs
{
    public class TagDto
    {
        public Guid TagId { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateTagDto
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }

    public class UpdateTagDto
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
