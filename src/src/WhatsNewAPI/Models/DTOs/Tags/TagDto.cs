namespace WhatsNewAPI.Models.DTOs.Tags;

public class TagDto
{
    public int TagId { get; set; }
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
