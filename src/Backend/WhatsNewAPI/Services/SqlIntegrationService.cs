using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WhatsNewAPI.DTOs;
using WhatsNewAPI.Models;
using WhatsNewAPI.Repositories;

namespace WhatsNewAPI.Services
{
    public class SqlIntegrationService : ISqlIntegrationService
    {
        private readonly ISqlIntegrationRepository _sqlIntegrationRepository;
        private readonly IReleaseRepository _releaseRepository;
        private readonly IChangeRepository _changeRepository;
        private readonly ITagRepository _tagRepository;

        public SqlIntegrationService(
            ISqlIntegrationRepository sqlIntegrationRepository,
            IReleaseRepository releaseRepository,
            IChangeRepository changeRepository,
            ITagRepository tagRepository)
        {
            _sqlIntegrationRepository = sqlIntegrationRepository;
            _releaseRepository = releaseRepository;
            _changeRepository = changeRepository;
            _tagRepository = tagRepository;
        }

        public async Task<TestConnectionDto> TestConnectionAsync(SqlConnection sqlConnection)
        {
            try
            {
                var connectionString = BuildConnectionString(sqlConnection);

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    return new TestConnectionDto
                    {
                        Success = true,
                        Message = "Connection successful"
                    };
                }
            }
            catch (Exception ex)
            {
                return new TestConnectionDto
                {
                    Success = false,
                    Message = $"Connection failed: {ex.Message}"
                };
            }
        }

        public async Task<ExecuteQueryResultDto> ExecuteQueryAsync(Guid queryId)
        {
            var result = new ExecuteQueryResultDto { Success = true };

            try
            {
                // Get query and connection
                var query = await _sqlIntegrationRepository.GetQueryByIdAsync(queryId);
                if (query == null)
                {
                    result.Success = false;
                    result.Message = "Query not found";
                    return result;
                }

                var sqlConnection = await _sqlIntegrationRepository.GetConnectionByIdAsync(query.ConnectionId);
                if (sqlConnection == null)
                {
                    result.Success = false;
                    result.Message = "Connection not found";
                    return result;
                }

                // Get all tags for mapping
                var allTags = await _tagRepository.GetAllTagsAsync();
                var tagDict = allTags.ToDictionary(t => t.Value.ToLower(), t => t);

                // Execute query
                var connectionString = BuildConnectionString(sqlConnection);
                var releaseGroups = new Dictionary<string, List<ExcelReleaseRow>>();

                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand(query.QueryText, connection))
                    {
                        await connection.OpenAsync();

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                try
                                {
                                    // Expected columns: Version, ReleaseDate, ChangeType, Description, ModuleTags
                                    var version = reader["Version"]?.ToString();
                                    var releaseDateStr = reader["ReleaseDate"]?.ToString();
                                    var changeType = reader["ChangeType"]?.ToString();
                                    var description = reader["Description"]?.ToString();
                                    var moduleTags = reader["ModuleTags"]?.ToString();

                                    if (string.IsNullOrWhiteSpace(version) || string.IsNullOrWhiteSpace(description))
                                        continue;

                                    DateTime releaseDate;
                                    if (!DateTime.TryParse(releaseDateStr, out releaseDate))
                                    {
                                        releaseDate = DateTime.UtcNow;
                                    }

                                    var row = new ExcelReleaseRow
                                    {
                                        Version = version,
                                        ReleaseDate = releaseDate,
                                        ChangeType = changeType ?? "new-feature",
                                        Description = description,
                                        ModuleTags = moduleTags ?? ""
                                    };

                                    if (!releaseGroups.ContainsKey(version))
                                    {
                                        releaseGroups[version] = new List<ExcelReleaseRow>();
                                    }
                                    releaseGroups[version].Add(row);
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }

                // Create releases and changes
                foreach (var group in releaseGroups)
                {
                    try
                    {
                        var firstRow = group.Value.First();

                        // Create release
                        var release = new Release
                        {
                            Version = group.Key,
                            ReleaseDate = firstRow.ReleaseDate
                        };

                        var createdRelease = await _releaseRepository.CreateReleaseAsync(release);
                        if (createdRelease != null)
                        {
                            result.ReleasesImported++;

                            // Create changes for this release
                            foreach (var row in group.Value)
                            {
                                try
                                {
                                    // Parse module tags
                                    var tagIds = new List<Guid>();
                                    if (!string.IsNullOrWhiteSpace(row.ModuleTags))
                                    {
                                        var tagNames = row.ModuleTags.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                                        foreach (var tagName in tagNames)
                                        {
                                            var normalizedTag = tagName.Trim().ToLower();
                                            if (tagDict.ContainsKey(normalizedTag))
                                            {
                                                tagIds.Add(tagDict[normalizedTag].TagId);
                                            }
                                        }
                                    }

                                    var change = new Change
                                    {
                                        ReleaseId = createdRelease.ReleaseId,
                                        Description = row.Description,
                                        ChangeType = NormalizeChangeType(row.ChangeType),
                                        TagIds = tagIds
                                    };

                                    var createdChange = await _changeRepository.CreateChangeAsync(change);
                                    if (createdChange != null)
                                    {
                                        result.ChangesImported++;
                                    }
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                result.Message = $"Import completed. {result.ReleasesImported} releases, {result.ChangesImported} changes imported";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Failed to execute query: {ex.Message}";
            }

            return result;
        }

        private string BuildConnectionString(SqlConnection sqlConnection)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = sqlConnection.Server,
                InitialCatalog = sqlConnection.Database
            };

            if (sqlConnection.UseIntegratedSecurity)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = sqlConnection.Username;
                builder.Password = sqlConnection.Password;
            }

            return builder.ConnectionString;
        }

        private string NormalizeChangeType(string changeType)
        {
            if (string.IsNullOrWhiteSpace(changeType))
                return "new-feature";

            var normalized = changeType.ToLower().Trim();

            if (normalized.Contains("bug") || normalized.Contains("fix"))
                return "bug-fix";
            if (normalized.Contains("enhance") || normalized.Contains("improve"))
                return "enhancement";
            if (normalized.Contains("new") || normalized.Contains("feature"))
                return "new-feature";

            return "new-feature";
        }
    }
}
