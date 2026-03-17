using WhatsNewAPI.Models.DTOs.Releases;
using WhatsNewAPI.Models.DTOs.Common;

namespace WhatsNewAPI.Services.Interfaces;

public interface IReleaseService
{
    Task<List<ReleaseDto>> GetAllReleasesAsync(bool includeUnpublished = false);
    Task<ReleaseDetailDto?> GetReleaseByIdAsync(int releaseId);
    Task<ReleaseDto> CreateReleaseAsync(CreateReleaseDto request, int userId);
    Task<bool> UpdateReleaseAsync(int releaseId, UpdateReleaseDto request, int userId);
    Task<bool> DeleteReleaseAsync(int releaseId, int userId);
    Task<StatisticsDto> GetStatisticsAsync();
}
