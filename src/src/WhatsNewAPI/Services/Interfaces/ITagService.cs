using WhatsNewAPI.Models.DTOs.Tags;

namespace WhatsNewAPI.Services.Interfaces;

public interface ITagService
{
    Task<List<TagDto>> GetAllTagsAsync(bool activeOnly = true);
    Task<TagDto?> GetByIdAsync(int tagId);
    Task<TagDto> CreateTagAsync(CreateTagDto request, int userId);
    Task<bool> UpdateTagAsync(int tagId, UpdateTagDto request, int userId);
    Task<bool> DeleteTagAsync(int tagId, int userId);
}
