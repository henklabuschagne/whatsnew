using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WhatsNewAPI.Models;

namespace WhatsNewAPI.Repositories
{
    public class SqlIntegrationRepository : ISqlIntegrationRepository
    {
        private readonly string _connectionString;

        public SqlIntegrationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // SQL Connections
        public async Task<IEnumerable<SqlConnection>> GetAllConnectionsAsync()
        {
            var connections = new List<SqlConnection>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetAllSqlConnections", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            connections.Add(new SqlConnection
                            {
                                ConnectionId = reader.GetGuid(reader.GetOrdinal("ConnectionId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Server = reader.GetString(reader.GetOrdinal("Server")),
                                Database = reader.GetString(reader.GetOrdinal("Database")),
                                Username = reader.IsDBNull(reader.GetOrdinal("Username")) ? null : reader.GetString(reader.GetOrdinal("Username")),
                                UseIntegratedSecurity = reader.GetBoolean(reader.GetOrdinal("UseIntegratedSecurity")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                            });
                        }
                    }
                }
            }

            return connections;
        }

        public async Task<SqlConnection> GetConnectionByIdAsync(Guid connectionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetSqlConnectionById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ConnectionId", connectionId);
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new SqlConnection
                            {
                                ConnectionId = reader.GetGuid(reader.GetOrdinal("ConnectionId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Server = reader.GetString(reader.GetOrdinal("Server")),
                                Database = reader.GetString(reader.GetOrdinal("Database")),
                                Username = reader.IsDBNull(reader.GetOrdinal("Username")) ? null : reader.GetString(reader.GetOrdinal("Username")),
                                Password = reader.IsDBNull(reader.GetOrdinal("Password")) ? null : reader.GetString(reader.GetOrdinal("Password")),
                                UseIntegratedSecurity = reader.GetBoolean(reader.GetOrdinal("UseIntegratedSecurity")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<SqlConnection> CreateConnectionAsync(SqlConnection sqlConnection)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_CreateSqlConnection", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    var connectionIdParam = new SqlParameter("@ConnectionId", SqlDbType.UniqueIdentifier)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(connectionIdParam);

                    command.Parameters.AddWithValue("@Name", sqlConnection.Name);
                    command.Parameters.AddWithValue("@Server", sqlConnection.Server);
                    command.Parameters.AddWithValue("@Database", sqlConnection.Database);
                    command.Parameters.AddWithValue("@Username", (object)sqlConnection.Username ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Password", (object)sqlConnection.Password ?? DBNull.Value);
                    command.Parameters.AddWithValue("@UseIntegratedSecurity", sqlConnection.UseIntegratedSecurity);
                    command.Parameters.AddWithValue("@IsActive", sqlConnection.IsActive);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new SqlConnection
                            {
                                ConnectionId = reader.GetGuid(reader.GetOrdinal("ConnectionId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Server = reader.GetString(reader.GetOrdinal("Server")),
                                Database = reader.GetString(reader.GetOrdinal("Database")),
                                Username = reader.IsDBNull(reader.GetOrdinal("Username")) ? null : reader.GetString(reader.GetOrdinal("Username")),
                                UseIntegratedSecurity = reader.GetBoolean(reader.GetOrdinal("UseIntegratedSecurity")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<SqlConnection> UpdateConnectionAsync(SqlConnection sqlConnection)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_UpdateSqlConnection", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ConnectionId", sqlConnection.ConnectionId);
                    command.Parameters.AddWithValue("@Name", sqlConnection.Name);
                    command.Parameters.AddWithValue("@Server", sqlConnection.Server);
                    command.Parameters.AddWithValue("@Database", sqlConnection.Database);
                    command.Parameters.AddWithValue("@Username", (object)sqlConnection.Username ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Password", (object)sqlConnection.Password ?? DBNull.Value);
                    command.Parameters.AddWithValue("@UseIntegratedSecurity", sqlConnection.UseIntegratedSecurity);
                    command.Parameters.AddWithValue("@IsActive", sqlConnection.IsActive);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new SqlConnection
                            {
                                ConnectionId = reader.GetGuid(reader.GetOrdinal("ConnectionId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Server = reader.GetString(reader.GetOrdinal("Server")),
                                Database = reader.GetString(reader.GetOrdinal("Database")),
                                Username = reader.IsDBNull(reader.GetOrdinal("Username")) ? null : reader.GetString(reader.GetOrdinal("Username")),
                                UseIntegratedSecurity = reader.GetBoolean(reader.GetOrdinal("UseIntegratedSecurity")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<bool> DeleteConnectionAsync(Guid connectionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_DeleteSqlConnection", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ConnectionId", connectionId);

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

        // SQL Queries
        public async Task<IEnumerable<SqlQuery>> GetAllQueriesAsync()
        {
            var queries = new List<SqlQuery>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetAllSqlQueries", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            queries.Add(new SqlQuery
                            {
                                QueryId = reader.GetGuid(reader.GetOrdinal("QueryId")),
                                ConnectionId = reader.GetGuid(reader.GetOrdinal("ConnectionId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                QueryText = reader.GetString(reader.GetOrdinal("QueryText")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                                ConnectionName = reader.GetString(reader.GetOrdinal("ConnectionName"))
                            });
                        }
                    }
                }
            }

            return queries;
        }

        public async Task<SqlQuery> GetQueryByIdAsync(Guid queryId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetSqlQueryById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@QueryId", queryId);
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new SqlQuery
                            {
                                QueryId = reader.GetGuid(reader.GetOrdinal("QueryId")),
                                ConnectionId = reader.GetGuid(reader.GetOrdinal("ConnectionId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                QueryText = reader.GetString(reader.GetOrdinal("QueryText")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                                ConnectionName = reader.GetString(reader.GetOrdinal("ConnectionName"))
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<SqlQuery> CreateQueryAsync(SqlQuery query)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_CreateSqlQuery", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    var queryIdParam = new SqlParameter("@QueryId", SqlDbType.UniqueIdentifier)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(queryIdParam);

                    command.Parameters.AddWithValue("@ConnectionId", query.ConnectionId);
                    command.Parameters.AddWithValue("@Name", query.Name);
                    command.Parameters.AddWithValue("@Description", (object)query.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@QueryText", query.QueryText);
                    command.Parameters.AddWithValue("@IsActive", query.IsActive);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new SqlQuery
                            {
                                QueryId = reader.GetGuid(reader.GetOrdinal("QueryId")),
                                ConnectionId = reader.GetGuid(reader.GetOrdinal("ConnectionId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                QueryText = reader.GetString(reader.GetOrdinal("QueryText")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                                ConnectionName = reader.GetString(reader.GetOrdinal("ConnectionName"))
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<SqlQuery> UpdateQueryAsync(SqlQuery query)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_UpdateSqlQuery", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@QueryId", query.QueryId);
                    command.Parameters.AddWithValue("@Name", query.Name);
                    command.Parameters.AddWithValue("@Description", (object)query.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@QueryText", query.QueryText);
                    command.Parameters.AddWithValue("@IsActive", query.IsActive);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new SqlQuery
                            {
                                QueryId = reader.GetGuid(reader.GetOrdinal("QueryId")),
                                ConnectionId = reader.GetGuid(reader.GetOrdinal("ConnectionId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                QueryText = reader.GetString(reader.GetOrdinal("QueryText")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                                ConnectionName = reader.GetString(reader.GetOrdinal("ConnectionName"))
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<bool> DeleteQueryAsync(Guid queryId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_DeleteSqlQuery", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@QueryId", queryId);

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
    }
}
