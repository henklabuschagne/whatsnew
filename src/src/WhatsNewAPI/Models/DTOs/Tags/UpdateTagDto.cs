using System.ComponentModel.DataAnnotations;

namespace WhatsNewAPI.Models.DTOs.Tags;

public class UpdateTagDto
{
    [Required(ErrorMessage = "Tag label is required")]
    [StringLength(100, ErrorMessage = "Tag label cannot exceed 100 characters")]
    public string Label { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
