using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WhatsNewAPI.Models;

namespace WhatsNewAPI.Repositories
{
    public class ReleaseRepository : IReleaseRepository
    {
        private readonly string _connectionString;

        public ReleaseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Release>> GetAllReleasesAsync()
        {
            var releases = new List<Release>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetAllReleases", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            releases.Add(new Release
                            {
                                ReleaseId = reader.GetGuid(reader.GetOrdinal("ReleaseId")),
                                Version = reader.GetString(reader.GetOrdinal("Version")),
                                ReleaseDate = reader.GetDateTime(reader.GetOrdinal("ReleaseDate")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                            });
                        }
                    }
                }
            }

            return releases;
        }

        public async Task<Release> GetReleaseByIdAsync(Guid releaseId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetReleaseById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ReleaseId", releaseId);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Release
                            {
                                ReleaseId = reader.GetGuid(reader.GetOrdinal("ReleaseId")),
                                Version = reader.GetString(reader.GetOrdinal("Version")),
                                ReleaseDate = reader.GetDateTime(reader.GetOrdinal("ReleaseDate")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<Release> CreateReleaseAsync(Release release)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_CreateRelease", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    var releaseIdParam = new SqlParameter("@ReleaseId", SqlDbType.UniqueIdentifier)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(releaseIdParam);

                    command.Parameters.AddWithValue("@Version", release.Version);
                    command.Parameters.AddWithValue("@ReleaseDate", release.ReleaseDate);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Release
                            {
                                ReleaseId = reader.GetGuid(reader.GetOrdinal("ReleaseId")),
                                Version = reader.GetString(reader.GetOrdinal("Version")),
                                ReleaseDate = reader.GetDateTime(reader.GetOrdinal("ReleaseDate")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<Release> UpdateReleaseAsync(Release release)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_UpdateRelease", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ReleaseId", release.ReleaseId);
                    command.Parameters.AddWithValue("@Version", release.Version);
                    command.Parameters.AddWithValue("@ReleaseDate", release.ReleaseDate);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Release
                            {
                                ReleaseId = reader.GetGuid(reader.GetOrdinal("ReleaseId")),
                                Version = reader.GetString(reader.GetOrdinal("Version")),
                                ReleaseDate = reader.GetDateTime(reader.GetOrdinal("ReleaseDate")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<bool> DeleteReleaseAsync(Guid releaseId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_DeleteRelease", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ReleaseId", releaseId);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return reader.GetInt32(reader.GetOrdinal("Success")) == 1;
                        }
                    }
                }
            }

            return false;
        }

        // Enhanced queries
        public async Task<IEnumerable<Release>> GetReleasesWithFiltersAsync(ReleaseFilterDto filter)
        {
            var releases = new List<Release>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetReleasesWithFilters", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SearchTerm", (object)filter.SearchTerm ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ChangeType", (object)filter.ChangeType ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ModuleTagId", (object)filter.ModuleTagId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FromDate", (object)filter.FromDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ToDate", (object)filter.ToDate ?? DBNull.Value);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            releases.Add(new Release
                            {
                                ReleaseId = reader.GetGuid(reader.GetOrdinal("ReleaseId")),
                                Version = reader.GetString(reader.GetOrdinal("Version")),
                                ReleaseDate = reader.GetDateTime(reader.GetOrdinal("ReleaseDate")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                            });
                        }
                    }
                }
            }

            return releases;
        }

        public async Task<ReleaseStatisticsDto> GetReleaseStatisticsAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetReleaseStatistics", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new ReleaseStatisticsDto
                            {
                                TotalReleases = reader.GetInt32(reader.GetOrdinal("TotalReleases")),
                                TotalChanges = reader.GetInt32(reader.GetOrdinal("TotalChanges")),
                                BugFixCount = reader.GetInt32(reader.GetOrdinal("BugFixCount")),
                                NewFeatureCount = reader.GetInt32(reader.GetOrdinal("NewFeatureCount")),
                                EnhancementCount = reader.GetInt32(reader.GetOrdinal("EnhancementCount")),
                                FirstReleaseDate = reader.IsDBNull(reader.GetOrdinal("FirstReleaseDate")) ? null : reader.GetDateTime(reader.GetOrdinal("FirstReleaseDate")),
                                LatestReleaseDate = reader.IsDBNull(reader.GetOrdinal("LatestReleaseDate")) ? null : reader.GetDateTime(reader.GetOrdinal("LatestReleaseDate"))
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<IEnumerable<PopularTagDto>> GetPopularTagsAsync(int topN = 10)
        {
            var tags = new List<PopularTagDto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetPopularTags", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TopN", topN);
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tags.Add(new PopularTagDto
                            {
                                TagId = reader.GetGuid(reader.GetOrdinal("TagId")),
                                Label = reader.GetString(reader.GetOrdinal("Label")),
                                Value = reader.GetString(reader.GetOrdinal("Value")),
                                Type = reader.GetString(reader.GetOrdinal("Type")),
                                UsageCount = reader.GetInt32(reader.GetOrdinal("UsageCount"))
                            });
                        }
                    }
                }
            }

            return tags;
        }

        public async Task<IEnumerable<VersionListItemDto>> GetVersionListAsync()
        {
            var versions = new List<VersionListItemDto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetVersionList", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            versions.Add(new VersionListItemDto
                            {
                                ReleaseId = reader.GetGuid(reader.GetOrdinal("ReleaseId")),
                                Version = reader.GetString(reader.GetOrdinal("Version")),
                                ReleaseDate = reader.GetDateTime(reader.GetOrdinal("ReleaseDate")),
                                ChangeCount = reader.GetInt32(reader.GetOrdinal("ChangeCount"))
                            });
                        }
                    }
                }
            }

            return versions;
        }

        public async Task<IEnumerable<ChangeSearchResultDto>> SearchChangesAsync(string searchTerm)
        {
            var results = new List<ChangeSearchResultDto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_SearchChanges", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SearchTerm", searchTerm);
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(new ChangeSearchResultDto
                            {
                                ChangeId = reader.GetGuid(reader.GetOrdinal("ChangeId")),
                                ReleaseId = reader.GetGuid(reader.GetOrdinal("ReleaseId")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                ChangeType = reader.GetString(reader.GetOrdinal("ChangeType")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                                Version = reader.GetString(reader.GetOrdinal("Version")),
                                ReleaseDate = reader.GetDateTime(reader.GetOrdinal("ReleaseDate"))
                            });
                        }
                    }
                }
            }

            return results;
        }
    }
}