using System;
using System.Collections.Generic;

namespace WhatsNewAPI.DTOs
{
    public class ExcelImportResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int ReleasesImported { get; set; }
        public int ChangesImported { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class ExcelReleaseRow
    {
        public string Version { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string ChangeType { get; set; }
        public string Description { get; set; }
        public string ModuleTags { get; set; }
    }
}
