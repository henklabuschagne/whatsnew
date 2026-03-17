using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WhatsNewAPI.Models.Entities;
using WhatsNewAPI.Repositories.Interfaces;

namespace WhatsNewAPI.Repositories.Implementations;

public class TagRepository : ITagRepository
{
    private readonly string _connectionString;

    public TagRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<Tag>> GetAllAsync(bool activeOnly = true)
    {
        using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@ActiveOnly", activeOnly ? 1 : 0);

        var tags = await connection.QueryAsync<Tag>(
            "sp_GetAllTags",
            parameters,
            commandType: CommandType.StoredProcedure
        );
        return tags.ToList();
    }

    public async Task<Tag?> GetByIdAsync(int tagId)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Tag>(
            "SELECT * FROM Tags WHERE TagId = @TagId",
            new { TagId = tagId }
        );
    }

    public async Task<Tag?> GetByValueAsync(string tagValue)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Tag>(
            "SELECT * FROM Tags WHERE TagValue = @TagValue",
            new { TagValue = tagValue }
        );
    }

    public async Task<int> CreateAsync(Tag tag, int createdBy)
    {
        using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@TagValue", tag.TagValue);
        parameters.Add("@TagLabel", tag.TagLabel);
        parameters.Add("@TagType", tag.TagType);
        parameters.Add("@CreatedBy", createdBy);
        parameters.Add("@TagId", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await connection.ExecuteAsync(
            "sp_CreateTag",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return parameters.Get<int>("@TagId");
    }

    public async Task<bool> UpdateAsync(int tagId, Tag tag, int updatedBy)
    {
        using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@TagId", tagId);
        parameters.Add("@TagLabel", tag.TagLabel);
        parameters.Add("@IsActive", tag.IsActive);
        parameters.Add("@UpdatedBy", updatedBy);

        var rowsAffected = await connection.ExecuteAsync(
            "sp_UpdateTag",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int tagId, int deletedBy)
    {
        using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@TagId", tagId);
        parameters.Add("@DeletedBy", deletedBy);

        var rowsAffected = await connection.ExecuteAsync(
            "sp_DeleteTag",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return rowsAffected > 0;
    }
}
