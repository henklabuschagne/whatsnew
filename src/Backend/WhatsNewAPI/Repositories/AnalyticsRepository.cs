using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WhatsNewAPI.DTOs;

namespace WhatsNewAPI.Repositories
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly string _connectionString;

        public AnalyticsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<ReleaseTimelineDto>> GetReleaseTimelineAsync(int months = 12)
        {
            var timeline = new List<ReleaseTimelineDto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetReleaseTimeline", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Months", months);
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            timeline.Add(new ReleaseTimelineDto
                            {
                                Year = reader.GetInt32(reader.GetOrdinal("Year")),
                                Month = reader.GetInt32(reader.GetOrdinal("Month")),
                                MonthName = reader.GetString(reader.GetOrdinal("MonthName")),
                                ReleaseCount = reader.GetInt32(reader.GetOrdinal("ReleaseCount")),
                                TotalChanges = reader.GetInt32(reader.GetOrdinal("TotalChanges")),
                                BugFixes = reader.GetInt32(reader.GetOrdinal("BugFixes")),
                                NewFeatures = reader.GetInt32(reader.GetOrdinal("NewFeatures")),
                                Enhancements = reader.GetInt32(reader.GetOrdinal("Enhancements"))
                            });
                        }
                    }
                }
            }

            return timeline;
        }

        public async Task<IEnumerable<ModuleDistributionDto>> GetModuleDistributionAsync()
        {
            var distribution = new List<ModuleDistributionDto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetModuleDistribution", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            distribution.Add(new ModuleDistributionDto
                            {
                                TagId = reader.GetGuid(reader.GetOrdinal("TagId")),
                                ModuleName = reader.GetString(reader.GetOrdinal("ModuleName")),
                                ModuleValue = reader.GetString(reader.GetOrdinal("ModuleValue")),
                                ChangeCount = reader.GetInt32(reader.GetOrdinal("ChangeCount")),
                                BugFixes = reader.GetInt32(reader.GetOrdinal("BugFixes")),
                                NewFeatures = reader.GetInt32(reader.GetOrdinal("NewFeatures")),
                                Enhancements = reader.GetInt32(reader.GetOrdinal("Enhancements"))
                            });
                        }
                    }
                }
            }

            return distribution;
        }

        public async Task<IEnumerable<ChangeTypeDistributionDto>> GetChangeTypeDistributionAsync()
        {
            var distribution = new List<ChangeTypeDistributionDto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetChangeTypeDistribution", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            distribution.Add(new ChangeTypeDistributionDto
                            {
                                ChangeType = reader.GetString(reader.GetOrdinal("ChangeType")),
                                Count = reader.GetInt32(reader.GetOrdinal("Count")),
                                Percentage = reader.GetDecimal(reader.GetOrdinal("Percentage"))
                            });
                        }
                    }
                }
            }

            return distribution;
        }

        public async Task<IEnumerable<RecentActivityDto>> GetRecentActivityAsync(int topN = 20)
        {
            var activities = new List<RecentActivityDto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetRecentActivity", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TopN", topN);
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            activities.Add(new RecentActivityDto
                            {
                                ActivityType = reader.GetString(reader.GetOrdinal("ActivityType")),
                                EntityId = reader.GetGuid(reader.GetOrdinal("EntityId")),
                                EntityName = reader.GetString(reader.GetOrdinal("EntityName")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                ActivityDate = reader.GetDateTime(reader.GetOrdinal("ActivityDate"))
                            });
                        }
                    }
                }
            }

            return activities;
        }

        public async Task<ReleaseVelocityDto> GetReleaseVelocityAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetReleaseVelocity", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new ReleaseVelocityDto
                            {
                                ReleasesLast30Days = reader.GetInt32(reader.GetOrdinal("ReleasesLast30Days")),
                                ReleasesLast90Days = reader.GetInt32(reader.GetOrdinal("ReleasesLast90Days")),
                                ReleasesLast365Days = reader.GetInt32(reader.GetOrdinal("ReleasesLast365Days")),
                                AvgDaysBetweenReleases = reader.IsDBNull(reader.GetOrdinal("AvgDaysBetweenReleases")) ? null : reader.GetDecimal(reader.GetOrdinal("AvgDaysBetweenReleases"))
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<IEnumerable<TopReleaseDto>> GetTopReleasesAsync(int topN = 10)
        {
            var topReleases = new List<TopReleaseDto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetTopReleases", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TopN", topN);
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            topReleases.Add(new TopReleaseDto
                            {
                                ReleaseId = reader.GetGuid(reader.GetOrdinal("ReleaseId")),
                                Version = reader.GetString(reader.GetOrdinal("Version")),
                                ReleaseDate = reader.GetDateTime(reader.GetOrdinal("ReleaseDate")),
                                ChangeCount = reader.GetInt32(reader.GetOrdinal("ChangeCount")),
                                BugFixes = reader.GetInt32(reader.GetOrdinal("BugFixes")),
                                NewFeatures = reader.GetInt32(reader.GetOrdinal("NewFeatures")),
                                Enhancements = reader.GetInt32(reader.GetOrdinal("Enhancements"))
                            });
                        }
                    }
                }
            }

            return topReleases;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetDashboardSummary", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new DashboardSummaryDto
                            {
                                TotalReleases = reader.GetInt32(reader.GetOrdinal("TotalReleases")),
                                TotalChanges = reader.GetInt32(reader.GetOrdinal("TotalChanges")),
                                TotalModules = reader.GetInt32(reader.GetOrdinal("TotalModules")),
                                ReleasesThisMonth = reader.GetInt32(reader.GetOrdinal("ReleasesThisMonth")),
                                ChangesThisMonth = reader.GetInt32(reader.GetOrdinal("ChangesThisMonth")),
                                LatestReleaseDate = reader.IsDBNull(reader.GetOrdinal("LatestReleaseDate")) ? null : reader.GetDateTime(reader.GetOrdinal("LatestReleaseDate")),
                                LatestVersion = reader.IsDBNull(reader.GetOrdinal("LatestVersion")) ? null : reader.GetString(reader.GetOrdinal("LatestVersion"))
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<IEnumerable<ChangeTrendDto>> GetChangeTrendsAsync(int days = 30)
        {
            var trends = new List<ChangeTrendDto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetChangeTrends", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Days", days);
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            trends.Add(new ChangeTrendDto
                            {
                                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                TotalChanges = reader.GetInt32(reader.GetOrdinal("TotalChanges")),
                                BugFixes = reader.GetInt32(reader.GetOrdinal("BugFixes")),
                                NewFeatures = reader.GetInt32(reader.GetOrdinal("NewFeatures")),
                                Enhancements = reader.GetInt32(reader.GetOrdinal("Enhancements"))
                            });
                        }
                    }
                }
            }

            return trends;
        }

        public async Task<IEnumerable<ClientDistributionDto>> GetClientDistributionAsync()
        {
            var distribution = new List<ClientDistributionDto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetClientDistribution", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            distribution.Add(new ClientDistributionDto
                            {
                                ClientId = reader.IsDBNull(reader.GetOrdinal("ClientId")) ? null : reader.GetGuid(reader.GetOrdinal("ClientId")),
                                ClientName = reader.GetString(reader.GetOrdinal("ClientName")),
                                ClientCode = reader.GetString(reader.GetOrdinal("ClientCode")),
                                ChangeCount = reader.GetInt32(reader.GetOrdinal("ChangeCount")),
                                BugFixes = reader.GetInt32(reader.GetOrdinal("BugFixes")),
                                Enhancements = reader.GetInt32(reader.GetOrdinal("Enhancements")),
                                NewFeatures = reader.GetInt32(reader.GetOrdinal("NewFeatures")),
                                Count = reader.GetInt32(reader.GetOrdinal("Count")),
                                Percentage = reader.GetInt32(reader.GetOrdinal("Percentage"))
                            });
                        }
                    }
                }
            }

            return distribution;
        }

        public async Task<TimeToActionMetricsDto> GetTimeToActionMetricsAsync()
        {
            var metrics = new TimeToActionMetricsDto();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetTimeToActionMetrics", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // First result set: By Change Type
                        while (await reader.ReadAsync())
                        {
                            metrics.ByChangeType.Add(new ChangeTypeMetricDto
                            {
                                ChangeType = reader.GetString(reader.GetOrdinal("ChangeType")),
                                Label = reader.GetString(reader.GetOrdinal("Label")),
                                AverageTotalTime = reader.IsDBNull(reader.GetOrdinal("AverageTotalTime")) ? 0 : Convert.ToDouble(reader.GetValue(reader.GetOrdinal("AverageTotalTime"))),
                                AverageDevTime = reader.IsDBNull(reader.GetOrdinal("AverageDevTime")) ? 0 : Convert.ToDouble(reader.GetValue(reader.GetOrdinal("AverageDevTime"))),
                                AverageTestTime = reader.IsDBNull(reader.GetOrdinal("AverageTestTime")) ? 0 : Convert.ToDouble(reader.GetValue(reader.GetOrdinal("AverageTestTime"))),
                                AverageReleaseTime = reader.IsDBNull(reader.GetOrdinal("AverageReleaseTime")) ? 0 : Convert.ToDouble(reader.GetValue(reader.GetOrdinal("AverageReleaseTime"))),
                                SubmittedToDeveloped = reader.IsDBNull(reader.GetOrdinal("SubmittedToDeveloped")) ? 0 : Convert.ToDouble(reader.GetValue(reader.GetOrdinal("SubmittedToDeveloped"))),
                                DevelopedToTested = reader.IsDBNull(reader.GetOrdinal("DevelopedToTested")) ? 0 : Convert.ToDouble(reader.GetValue(reader.GetOrdinal("DevelopedToTested"))),
                                TestedToReleased = reader.IsDBNull(reader.GetOrdinal("TestedToReleased")) ? 0 : Convert.ToDouble(reader.GetValue(reader.GetOrdinal("TestedToReleased"))),
                                Count = reader.GetInt32(reader.GetOrdinal("Count"))
                            });
                        }

                        // Second result set: Overall statistics
                        if (await reader.NextResultAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                metrics.Overall = new OverallMetricsDto
                                {
                                    AverageTotalTime = reader.IsDBNull(reader.GetOrdinal("AverageTotalTime")) ? 0 : Convert.ToDouble(reader.GetValue(reader.GetOrdinal("AverageTotalTime"))),
                                    FastestCompletion = reader.IsDBNull(reader.GetOrdinal("FastestCompletion")) ? 0 : Convert.ToDouble(reader.GetValue(reader.GetOrdinal("FastestCompletion"))),
                                    SlowestCompletion = reader.IsDBNull(reader.GetOrdinal("SlowestCompletion")) ? 0 : Convert.ToDouble(reader.GetValue(reader.GetOrdinal("SlowestCompletion"))),
                                    MedianTime = reader.IsDBNull(reader.GetOrdinal("MedianTime")) ? 0 : Convert.ToDouble(reader.GetValue(reader.GetOrdinal("MedianTime")))
                                };
                            }
                        }

                        // Third result set: Timeline
                        if (await reader.NextResultAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                metrics.Timeline.Add(new TimelineDataDto
                                {
                                    Month = reader.GetString(reader.GetOrdinal("Month")),
                                    MonthName = reader.GetString(reader.GetOrdinal("MonthName")),
                                    BugFix = reader.IsDBNull(reader.GetOrdinal("BugFix")) ? null : Convert.ToDouble(reader.GetValue(reader.GetOrdinal("BugFix"))),
                                    Enhancement = reader.IsDBNull(reader.GetOrdinal("Enhancement")) ? null : Convert.ToDouble(reader.GetValue(reader.GetOrdinal("Enhancement"))),
                                    NewFeature = reader.IsDBNull(reader.GetOrdinal("NewFeature")) ? null : Convert.ToDouble(reader.GetValue(reader.GetOrdinal("NewFeature")))
                                });
                            }
                        }
                    }
                }
            }

            return metrics;
        }
    }
}