using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using WhatsNewAPI.Models.Entities;
using WhatsNewAPI.Repositories.Interfaces;

namespace WhatsNewAPI.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@Username", username);

        return await connection.QueryFirstOrDefaultAsync<User>(
            "sp_GetUserByUsername",
            parameters,
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<User?> GetByIdAsync(int userId)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE UserId = @UserId",
            new { UserId = userId }
        );
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@Email", email);

        return await connection.QueryFirstOrDefaultAsync<User>(
            "sp_GetUserByEmail",
            parameters,
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<List<User>> GetAllAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        var users = await connection.QueryAsync<User>(
            "sp_GetAllUsers",
            commandType: CommandType.StoredProcedure
        );
        return users.ToList();
    }

    public async Task UpdateLastLoginAsync(int userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);

        await connection.ExecuteAsync(
            "sp_UpdateLastLogin",
            parameters,
            commandType: CommandType.StoredProcedure
        );
    }
}
