using System.ComponentModel.DataAnnotations;

namespace WhatsNewAPI.Models.DTOs.Tags;

public class CreateTagDto
{
    [Required(ErrorMessage = "Tag value is required")]
    [StringLength(100, ErrorMessage = "Tag value cannot exceed 100 characters")]
    [RegularExpression("^[a-z_]+$", 
        ErrorMessage = "Tag value must contain only lowercase letters and underscores")]
    public string Value { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tag label is required")]
    [StringLength(100, ErrorMessage = "Tag label cannot exceed 100 characters")]
    public string Label { get; set; } = string.Empty;

    public string Type { get; set; } = "module";
}
