namespace WhatsNewAPI.Models.Entities;

public class Release
{
    public int ReleaseId { get; set; }
    public string Version { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public string? Description { get; set; }
    public bool IsPublished { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public List<Change> Changes { get; set; } = new();
    public string? CreatedByUsername { get; set; }
    public int ChangeCount { get; set; }
}
