namespace WhatsNewAPI.Models.Entities;

public class Tag
{
    public int TagId { get; set; }
    public string TagValue { get; set; } = string.Empty;
    public string TagLabel { get; set; } = string.Empty;
    public string TagType { get; set; } = "module";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
