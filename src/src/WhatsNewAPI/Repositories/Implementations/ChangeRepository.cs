using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WhatsNewAPI.Models.Entities;
using WhatsNewAPI.Repositories.Interfaces;

namespace WhatsNewAPI.Repositories.Implementations;

public class ChangeRepository : IChangeRepository
{
    private readonly string _connectionString;

    public ChangeRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Change?> GetByIdAsync(int changeId)
    {
        using var connection = new SqlConnection(_connectionString);
        var change = await connection.QueryFirstOrDefaultAsync<Change>(
            "SELECT * FROM Changes WHERE ChangeId = @ChangeId",
            new { ChangeId = changeId }
        );

        if (change != null)
        {
            var tags = await connection.QueryAsync<string>(
                @"SELECT t.TagValue 
                  FROM Change_Tags ct 
                  INNER JOIN Tags t ON ct.TagId = t.TagId 
                  WHERE ct.ChangeId = @ChangeId",
                new { ChangeId = changeId }
            );
            change.ModuleTags = tags.ToList();
        }

        return change;
    }

    public async Task<List<Change>> GetByReleaseIdAsync(int releaseId)
    {
        using var connection = new SqlConnection(_connectionString);
        
        var changes = (await connection.QueryAsync<Change>(
            "SELECT * FROM Changes WHERE ReleaseId = @ReleaseId ORDER BY CreatedAt",
            new { ReleaseId = releaseId }
        )).ToList();

        if (changes.Any())
        {
            var changeIds = changes.Select(c => c.ChangeId).ToList();
            var tagMappings = await connection.QueryAsync<ChangeTagMapping>(
                @"SELECT ct.ChangeId, t.TagValue 
                  FROM Change_Tags ct 
                  INNER JOIN Tags t ON ct.TagId = t.TagId 
                  WHERE ct.ChangeId IN @ChangeIds",
                new { ChangeIds = changeIds }
            );

            foreach (var change in changes)
            {
                change.ModuleTags = tagMappings
                    .Where(tm => tm.ChangeId == change.ChangeId)
                    .Select(tm => tm.TagValue)
                    .ToList();
            }
        }

        return changes;
    }

    public async Task<int> CreateAsync(Change change, List<string> moduleTags, int createdBy)
    {
        using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@ReleaseId", change.ReleaseId);
        parameters.Add("@Description", change.Description);
        parameters.Add("@ChangeType", change.ChangeType);
        parameters.Add("@ModuleTags", string.Join(",", moduleTags));
        parameters.Add("@CreatedBy", createdBy);
        parameters.Add("@ChangeId", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await connection.ExecuteAsync(
            "sp_CreateChange",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return parameters.Get<int>("@ChangeId");
    }

    public async Task<bool> UpdateAsync(int changeId, Change change, List<string> moduleTags, int updatedBy)
    {
        using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@ChangeId", changeId);
        parameters.Add("@Description", change.Description);
        parameters.Add("@ChangeType", change.ChangeType);
        parameters.Add("@ModuleTags", string.Join(",", moduleTags));
        parameters.Add("@UpdatedBy", updatedBy);

        var rowsAffected = await connection.ExecuteAsync(
            "sp_UpdateChange",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int changeId, int deletedBy)
    {
        using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@ChangeId", changeId);
        parameters.Add("@DeletedBy", deletedBy);

        var rowsAffected = await connection.ExecuteAsync(
            "sp_DeleteChange",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return rowsAffected > 0;
    }

    private class ChangeTagMapping
    {
        public int ChangeId { get; set; }
        public string TagValue { get; set; } = string.Empty;
    }
}
