using System.ComponentModel.DataAnnotations;

namespace WhatsNewAPI.Models.DTOs.Changes;

public class UpdateChangeDto
{
    [Required(ErrorMessage = "Description is required")]
    [StringLength(5000, ErrorMessage = "Description cannot exceed 5000 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Change type is required")]
    [RegularExpression("^(bug_fix|new_feature|enhancement)$", 
        ErrorMessage = "Change type must be bug_fix, new_feature, or enhancement")]
    public string ChangeType { get; set; } = string.Empty;

    public List<string> ModuleTags { get; set; } = new();
}
