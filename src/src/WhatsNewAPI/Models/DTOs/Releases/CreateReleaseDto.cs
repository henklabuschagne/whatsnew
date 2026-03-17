using System.ComponentModel.DataAnnotations;

namespace WhatsNewAPI.Models.DTOs.Releases;

public class CreateReleaseDto
{
    [Required(ErrorMessage = "Version is required")]
    [StringLength(50, ErrorMessage = "Version cannot exceed 50 characters")]
    public string Version { get; set; } = string.Empty;

    [Required(ErrorMessage = "Release date is required")]
    public DateTime ReleaseDate { get; set; }

    [StringLength(5000, ErrorMessage = "Description cannot exceed 5000 characters")]
    public string? Description { get; set; }

    public bool IsPublished { get; set; } = false;
}
