using System.Collections.Generic;
using System.Threading.Tasks;
using WhatsNewAPI.DTOs;

namespace WhatsNewAPI.Repositories
{
    public interface IAnalyticsRepository
    {
        Task<IEnumerable<ReleaseTimelineDto>> GetReleaseTimelineAsync(int months = 12);
        Task<IEnumerable<ModuleDistributionDto>> GetModuleDistributionAsync();
        Task<IEnumerable<ChangeTypeDistributionDto>> GetChangeTypeDistributionAsync();
        Task<IEnumerable<RecentActivityDto>> GetRecentActivityAsync(int topN = 20);
        Task<ReleaseVelocityDto> GetReleaseVelocityAsync();
        Task<IEnumerable<TopReleaseDto>> GetTopReleasesAsync(int topN = 10);
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();
        Task<IEnumerable<ChangeTrendDto>> GetChangeTrendsAsync(int days = 30);
        Task<IEnumerable<ClientDistributionDto>> GetClientDistributionAsync();
        Task<TimeToActionMetricsDto> GetTimeToActionMetricsAsync();
    }
}