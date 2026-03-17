using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WhatsNewAPI.Models.Entities;
using WhatsNewAPI.Repositories.Interfaces;

namespace WhatsNewAPI.Repositories.Implementations;

public class ReleaseRepository : IReleaseRepository
{
    private readonly string _connectionString;

    public ReleaseRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<Release>> GetAllAsync(bool includeUnpublished = false)
    {
        using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@IncludeUnpublished", includeUnpublished ? 1 : 0);

        var releases = await connection.QueryAsync<Release>(
            "sp_GetAllReleases",
            parameters,
            commandType: CommandType.StoredProcedure
        );
        return releases.ToList();
    }

    public async Task<Release?> GetByIdAsync(int releaseId)
    {
        using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@ReleaseId", releaseId);

        using var multi = await connection.QueryMultipleAsync(
            "sp_GetReleaseById",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        var release = await multi.ReadFirstOrDefaultAsync<Release>();
        if (release == null) return null;

        var changes = (await multi.ReadAsync<Change>()).ToList();
        var changeTags = (await multi.ReadAsync<ChangeTagMapping>()).ToList();

        // Map tags to changes
        foreach (var change in changes)
        {
            change.ModuleTags = changeTags
                .Where(ct => ct.ChangeId == change.ChangeId)
                .Select(ct => ct.TagValue)
                .ToList();
        }

        release.Changes = changes;
        return release;
    }

    public async Task<int> CreateAsync(Release release, int createdBy)
    {
        using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@Version", release.Version);
        parameters.Add("@ReleaseDate", release.ReleaseDate);
        parameters.Add("@Description", release.Description);
        parameters.Add("@IsPublished", release.IsPublished);
        parameters.Add("@CreatedBy", createdBy);
        parameters.Add("@ReleaseId", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await connection.ExecuteAsync(
            "sp_CreateRelease",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return parameters.Get<int>("@ReleaseId");
    }

    public async Task<bool> UpdateAsync(int releaseId, Release release, int updatedBy)
    {
        using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@ReleaseId", releaseId);
        parameters.Add("@Version", release.Version);
        parameters.Add("@ReleaseDate", release.ReleaseDate);
        parameters.Add("@Description", release.Description);
        parameters.Add("@IsPublished", release.IsPublished);
        parameters.Add("@UpdatedBy", updatedBy);

        var rowsAffected = await connection.ExecuteAsync(
            "sp_UpdateRelease",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int releaseId, int deletedBy)
    {
        using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@ReleaseId", releaseId);
        parameters.Add("@DeletedBy", deletedBy);

        var rowsAffected = await connection.ExecuteAsync(
            "sp_DeleteRelease",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return rowsAffected > 0;
    }

    public async Task<(int TotalReleases, int PublishedReleases, int TotalChanges, int BugFixes, int NewFeatures, int Enhancements)> GetStatisticsAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        
        using var multi = await connection.QueryMultipleAsync(
            "sp_GetReleaseStatistics",
            commandType: CommandType.StoredProcedure
        );

        var stats = await multi.ReadFirstAsync<StatisticsResult>();
        
        return (
            stats.TotalReleases,
            stats.PublishedReleases,
            stats.TotalChanges,
            stats.BugFixes,
            stats.NewFeatures,
            stats.Enhancements
        );
    }

    private class ChangeTagMapping
    {
        public int ChangeId { get; set; }
        public int TagId { get; set; }
        public string TagValue { get; set; } = string.Empty;
        public string TagLabel { get; set; } = string.Empty;
    }

    private class StatisticsResult
    {
        public int TotalReleases { get; set; }
        public int PublishedReleases { get; set; }
        public int TotalChanges { get; set; }
        public int BugFixes { get; set; }
        public int NewFeatures { get; set; }
        public int Enhancements { get; set; }
    }
}
