namespace WhatsNewAPI.Models.DTOs.Releases;

public class ReleaseDto
{
    public int ReleaseId { get; set; }
    public string Version { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public string? Description { get; set; }
    public bool IsPublished { get; set; }
    public int ChangeCount { get; set; }
    public string? CreatedByUsername { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
