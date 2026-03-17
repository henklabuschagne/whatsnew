namespace WhatsNewAPI.Models.DTOs.Common;

public class StatisticsDto
{
    public int TotalReleases { get; set; }
    public int PublishedReleases { get; set; }
    public int TotalChanges { get; set; }
    public int BugFixes { get; set; }
    public int NewFeatures { get; set; }
    public int Enhancements { get; set; }
    public List<ModuleStatDto> ModuleStats { get; set; } = new();
}

public class ModuleStatDto
{
    public string ModuleName { get; set; } = string.Empty;
    public int ChangeCount { get; set; }
}
