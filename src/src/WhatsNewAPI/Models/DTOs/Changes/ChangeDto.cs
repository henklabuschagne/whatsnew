namespace WhatsNewAPI.Models.DTOs.Changes;

public class ChangeDto
{
    public int ChangeId { get; set; }
    public int ReleaseId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ChangeType { get; set; } = string.Empty;
    public List<string> ModuleTags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
