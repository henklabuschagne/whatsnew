using WhatsNewAPI.Models.DTOs.Changes;

namespace WhatsNewAPI.Services.Interfaces;

public interface IChangeService
{
    Task<ChangeDto?> GetByIdAsync(int changeId);
    Task<List<ChangeDto>> GetByReleaseIdAsync(int releaseId);
    Task<ChangeDto> CreateChangeAsync(CreateChangeDto request, int userId);
    Task<bool> UpdateChangeAsync(int changeId, UpdateChangeDto request, int userId);
    Task<bool> DeleteChangeAsync(int changeId, int userId);
}
