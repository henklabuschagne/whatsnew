using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhatsNewAPI.DTOs;
using WhatsNewAPI.Models;

namespace WhatsNewAPI.Repositories
{
    public interface IReleaseRepository
    {
        Task<IEnumerable<Release>> GetAllReleasesAsync();
        Task<Release> GetReleaseByIdAsync(Guid releaseId);
        Task<Release> CreateReleaseAsync(Release release);
        Task<Release> UpdateReleaseAsync(Release release);
        Task<bool> DeleteReleaseAsync(Guid releaseId);
        
        // Enhanced queries
        Task<IEnumerable<Release>> GetReleasesWithFiltersAsync(ReleaseFilterDto filter);
        Task<ReleaseStatisticsDto> GetReleaseStatisticsAsync();
        Task<IEnumerable<PopularTagDto>> GetPopularTagsAsync(int topN = 10);
        Task<IEnumerable<VersionListItemDto>> GetVersionListAsync();
        Task<IEnumerable<ChangeSearchResultDto>> SearchChangesAsync(string searchTerm);
    }
}