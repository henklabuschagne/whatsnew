using Dapper;
using Microsoft.Data.SqlClient;
using WhatsNewAPI.Models.Entities;
using WhatsNewAPI.Repositories.Interfaces;

namespace WhatsNewAPI.Repositories.Implementations;

public class AuditRepository : IAuditRepository
{
    private readonly string _connectionString;

    public AuditRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task LogAsync(AuditLog auditLog)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            @"INSERT INTO AuditLogs (UserId, Action, EntityType, EntityId, OldValue, NewValue, IpAddress, UserAgent, CreatedAt)
              VALUES (@UserId, @Action, @EntityType, @EntityId, @OldValue, @NewValue, @IpAddress, @UserAgent, @CreatedAt)",
            new
            {
                auditLog.UserId,
                auditLog.Action,
                auditLog.EntityType,
                auditLog.EntityId,
                auditLog.OldValue,
                auditLog.NewValue,
                auditLog.IpAddress,
                auditLog.UserAgent,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
