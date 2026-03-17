using System;
using System.Collections.Generic;

namespace WhatsNewAPI.DTOs
{
    public class ReleaseDto
    {
        public Guid ReleaseId { get; set; }
        public string Version { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<ChangeDto> Changes { get; set; } = new List<ChangeDto>();
    }

    public class CreateReleaseDto
    {
        public string Version { get; set; }
        public DateTime ReleaseDate { get; set; }
    }

    public class UpdateReleaseDto
    {
        public string Version { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
