using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhatsNewAPI.Models;

namespace WhatsNewAPI.Repositories
{
    public interface ISqlIntegrationRepository
    {
        // SQL Connections
        Task<IEnumerable<SqlConnection>> GetAllConnectionsAsync();
        Task<SqlConnection> GetConnectionByIdAsync(Guid connectionId);
        Task<SqlConnection> CreateConnectionAsync(SqlConnection connection);
        Task<SqlConnection> UpdateConnectionAsync(SqlConnection connection);
        Task<bool> DeleteConnectionAsync(Guid connectionId);

        // SQL Queries
        Task<IEnumerable<SqlQuery>> GetAllQueriesAsync();
        Task<SqlQuery> GetQueryByIdAsync(Guid queryId);
        Task<SqlQuery> CreateQueryAsync(SqlQuery query);
        Task<SqlQuery> UpdateQueryAsync(SqlQuery query);
        Task<bool> DeleteQueryAsync(Guid queryId);
    }
}
