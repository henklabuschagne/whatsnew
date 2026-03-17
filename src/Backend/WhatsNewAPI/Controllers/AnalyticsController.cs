using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsNewAPI.Repositories;

namespace WhatsNewAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsRepository _analyticsRepository;

        public AnalyticsController(IAnalyticsRepository analyticsRepository)
        {
            _analyticsRepository = analyticsRepository;
        }

        [HttpGet("timeline")]
        public async Task<IActionResult> GetReleaseTimeline([FromQuery] int months = 12)
        {
            try
            {
                var timeline = await _analyticsRepository.GetReleaseTimelineAsync(months);
                return Ok(timeline);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving timeline", error = ex.Message });
            }
        }

        [HttpGet("module-distribution")]
        public async Task<IActionResult> GetModuleDistribution()
        {
            try
            {
                var distribution = await _analyticsRepository.GetModuleDistributionAsync();
                return Ok(distribution);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving module distribution", error = ex.Message });
            }
        }

        [HttpGet("change-type-distribution")]
        public async Task<IActionResult> GetChangeTypeDistribution()
        {
            try
            {
                var distribution = await _analyticsRepository.GetChangeTypeDistributionAsync();
                return Ok(distribution);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving change type distribution", error = ex.Message });
            }
        }

        [HttpGet("recent-activity")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetRecentActivity([FromQuery] int topN = 20)
        {
            try
            {
                var activities = await _analyticsRepository.GetRecentActivityAsync(topN);
                return Ok(activities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving recent activity", error = ex.Message });
            }
        }

        [HttpGet("release-velocity")]
        public async Task<IActionResult> GetReleaseVelocity()
        {
            try
            {
                var velocity = await _analyticsRepository.GetReleaseVelocityAsync();
                return Ok(velocity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving release velocity", error = ex.Message });
            }
        }

        [HttpGet("top-releases")]
        public async Task<IActionResult> GetTopReleases([FromQuery] int topN = 10)
        {
            try
            {
                var topReleases = await _analyticsRepository.GetTopReleasesAsync(topN);
                return Ok(topReleases);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving top releases", error = ex.Message });
            }
        }

        [HttpGet("dashboard-summary")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            try
            {
                var summary = await _analyticsRepository.GetDashboardSummaryAsync();
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving dashboard summary", error = ex.Message });
            }
        }

        [HttpGet("change-trends")]
        public async Task<IActionResult> GetChangeTrends([FromQuery] int days = 30)
        {
            try
            {
                var trends = await _analyticsRepository.GetChangeTrendsAsync(days);
                return Ok(trends);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving change trends", error = ex.Message });
            }
        }

        [HttpGet("client-distribution")]
        public async Task<IActionResult> GetClientDistribution()
        {
            try
            {
                var distribution = await _analyticsRepository.GetClientDistributionAsync();
                return Ok(distribution);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving client distribution", error = ex.Message });
            }
        }

        [HttpGet("time-to-action")]
        public async Task<IActionResult> GetTimeToActionMetrics()
        {
            try
            {
                var metrics = await _analyticsRepository.GetTimeToActionMetricsAsync();
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving time to action metrics", error = ex.Message });
            }
        }
    }
}