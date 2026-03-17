using WhatsNewAPI.Models.Entities;

namespace WhatsNewAPI.Repositories.Interfaces;

public interface IReleaseRepository
{
    Task<List<Release>> GetAllAsync(bool includeUnpublished = false);
    Task<Release?> GetByIdAsync(int releaseId);
    Task<int> CreateAsync(Release release, int createdBy);
    Task<bool> UpdateAsync(int releaseId, Release release, int updatedBy);
    Task<bool> DeleteAsync(int releaseId, int deletedBy);
    Task<(int TotalReleases, int PublishedReleases, int TotalChanges, int BugFixes, int NewFeatures, int Enhancements)> GetStatisticsAsync();
}
