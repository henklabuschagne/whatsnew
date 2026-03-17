namespace WhatsNewAPI.Models.Entities;

public class Change
{
    public int ChangeId { get; set; }
    public int ReleaseId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ChangeType { get; set; } = string.Empty;
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public List<string> ModuleTags { get; set; } = new();
}
