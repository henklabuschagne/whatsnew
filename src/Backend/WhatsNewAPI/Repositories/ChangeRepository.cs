using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WhatsNewAPI.Models;

namespace WhatsNewAPI.Repositories
{
    public class ChangeRepository : IChangeRepository
    {
        private readonly string _connectionString;

        public ChangeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Change>> GetChangesByReleaseIdAsync(Guid releaseId)
        {
            var changes = new List<Change>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetChangesByReleaseId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ReleaseId", releaseId);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var change = new Change
                            {
                                ChangeId = reader.GetGuid(reader.GetOrdinal("ChangeId")),
                                ReleaseId = reader.GetGuid(reader.GetOrdinal("ReleaseId")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                ChangeType = reader.GetString(reader.GetOrdinal("ChangeType")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                            };

                            // Client tracking fields
                            var clientIdOrdinal = reader.GetOrdinal("ClientId");
                            if (!reader.IsDBNull(clientIdOrdinal))
                            {
                                change.ClientId = reader.GetGuid(clientIdOrdinal);
                            }

                            var ticketNumberOrdinal = reader.GetOrdinal("TicketNumber");
                            if (!reader.IsDBNull(ticketNumberOrdinal))
                            {
                                change.TicketNumber = reader.GetString(ticketNumberOrdinal);
                            }

                            var devOpsNumberOrdinal = reader.GetOrdinal("DevOpsNumber");
                            if (!reader.IsDBNull(devOpsNumberOrdinal))
                            {
                                change.DevOpsNumber = reader.GetString(devOpsNumberOrdinal);
                            }

                            var tagIdsOrdinal = reader.GetOrdinal("TagIds");
                            if (!reader.IsDBNull(tagIdsOrdinal))
                            {
                                var tagIds = reader.GetString(tagIdsOrdinal);
                                if (!string.IsNullOrEmpty(tagIds))
                                {
                                    change.TagIds = tagIds.Split(',')
                                        .Where(t => !string.IsNullOrWhiteSpace(t))
                                        .Select(t => Guid.Parse(t.Trim()))
                                        .ToList();
                                }
                            }

                            changes.Add(change);
                        }
                    }
                }
            }

            return changes;
        }

        public async Task<Change> GetChangeByIdAsync(Guid changeId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetChangeById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ChangeId", changeId);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var change = new Change
                            {
                                ChangeId = reader.GetGuid(reader.GetOrdinal("ChangeId")),
                                ReleaseId = reader.GetGuid(reader.GetOrdinal("ReleaseId")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                ChangeType = reader.GetString(reader.GetOrdinal("ChangeType")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                            };

                            // Client tracking fields
                            var clientIdOrdinal = reader.GetOrdinal("ClientId");
                            if (!reader.IsDBNull(clientIdOrdinal))
                            {
                                change.ClientId = reader.GetGuid(clientIdOrdinal);
                            }

                            var ticketNumberOrdinal = reader.GetOrdinal("TicketNumber");
                            if (!reader.IsDBNull(ticketNumberOrdinal))
                            {
                                change.TicketNumber = reader.GetString(ticketNumberOrdinal);
                            }

                            var devOpsNumberOrdinal = reader.GetOrdinal("DevOpsNumber");
                            if (!reader.IsDBNull(devOpsNumberOrdinal))
                            {
                                change.DevOpsNumber = reader.GetString(devOpsNumberOrdinal);
                            }

                            var tagIdsOrdinal = reader.GetOrdinal("TagIds");
                            if (!reader.IsDBNull(tagIdsOrdinal))
                            {
                                var tagIds = reader.GetString(tagIdsOrdinal);
                                if (!string.IsNullOrEmpty(tagIds))
                                {
                                    change.TagIds = tagIds.Split(',')
                                        .Where(t => !string.IsNullOrWhiteSpace(t))
                                        .Select(t => Guid.Parse(t.Trim()))
                                        .ToList();
                                }
                            }

                            return change;
                        }
                    }
                }
            }

            return null;
        }

        public async Task<Change> CreateChangeAsync(Change change)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_CreateChange", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    var changeIdParam = new SqlParameter("@ChangeId", SqlDbType.UniqueIdentifier)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(changeIdParam);

                    command.Parameters.AddWithValue("@ReleaseId", change.ReleaseId);
                    command.Parameters.AddWithValue("@Description", change.Description);
                    command.Parameters.AddWithValue("@ChangeType", change.ChangeType);

                    var tagIdsParam = change.TagIds != null && change.TagIds.Any()
                        ? string.Join(",", change.TagIds)
                        : (object)DBNull.Value;
                    command.Parameters.AddWithValue("@TagIds", tagIdsParam);

                    // Client tracking fields
                    command.Parameters.AddWithValue("@ClientId", (object)change.ClientId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TicketNumber", (object)change.TicketNumber ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DevOpsNumber", (object)change.DevOpsNumber ?? DBNull.Value);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var createdChange = new Change
                            {
                                ChangeId = reader.GetGuid(reader.GetOrdinal("ChangeId")),
                                ReleaseId = reader.GetGuid(reader.GetOrdinal("ReleaseId")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                ChangeType = reader.GetString(reader.GetOrdinal("ChangeType")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                            };

                            // Client tracking fields
                            var clientIdOrdinal = reader.GetOrdinal("ClientId");
                            if (!reader.IsDBNull(clientIdOrdinal))
                            {
                                createdChange.ClientId = reader.GetGuid(clientIdOrdinal);
                            }

                            var ticketNumberOrdinal = reader.GetOrdinal("TicketNumber");
                            if (!reader.IsDBNull(ticketNumberOrdinal))
                            {
                                createdChange.TicketNumber = reader.GetString(ticketNumberOrdinal);
                            }

                            var devOpsNumberOrdinal = reader.GetOrdinal("DevOpsNumber");
                            if (!reader.IsDBNull(devOpsNumberOrdinal))
                            {
                                createdChange.DevOpsNumber = reader.GetString(devOpsNumberOrdinal);
                            }

                            var tagIdsOrdinal = reader.GetOrdinal("TagIds");
                            if (!reader.IsDBNull(tagIdsOrdinal))
                            {
                                var tagIds = reader.GetString(tagIdsOrdinal);
                                if (!string.IsNullOrEmpty(tagIds))
                                {
                                    createdChange.TagIds = tagIds.Split(',')
                                        .Where(t => !string.IsNullOrWhiteSpace(t))
                                        .Select(t => Guid.Parse(t.Trim()))
                                        .ToList();
                                }
                            }

                            return createdChange;
                        }
                    }
                }
            }

            return null;
        }

        public async Task<Change> UpdateChangeAsync(Change change)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_UpdateChange", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ChangeId", change.ChangeId);
                    command.Parameters.AddWithValue("@Description", change.Description);
                    command.Parameters.AddWithValue("@ChangeType", change.ChangeType);

                    var tagIdsParam = change.TagIds != null && change.TagIds.Any()
                        ? string.Join(",", change.TagIds)
                        : (object)DBNull.Value;
                    command.Parameters.AddWithValue("@TagIds", tagIdsParam);

                    // Client tracking fields
                    command.Parameters.AddWithValue("@ClientId", (object)change.ClientId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TicketNumber", (object)change.TicketNumber ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DevOpsNumber", (object)change.DevOpsNumber ?? DBNull.Value);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var updatedChange = new Change
                            {
                                ChangeId = reader.GetGuid(reader.GetOrdinal("ChangeId")),
                                ReleaseId = reader.GetGuid(reader.GetOrdinal("ReleaseId")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                ChangeType = reader.GetString(reader.GetOrdinal("ChangeType")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                            };

                            // Client tracking fields
                            var clientIdOrdinal = reader.GetOrdinal("ClientId");
                            if (!reader.IsDBNull(clientIdOrdinal))
                            {
                                updatedChange.ClientId = reader.GetGuid(clientIdOrdinal);
                            }

                            var ticketNumberOrdinal = reader.GetOrdinal("TicketNumber");
                            if (!reader.IsDBNull(ticketNumberOrdinal))
                            {
                                updatedChange.TicketNumber = reader.GetString(ticketNumberOrdinal);
                            }

                            var devOpsNumberOrdinal = reader.GetOrdinal("DevOpsNumber");
                            if (!reader.IsDBNull(devOpsNumberOrdinal))
                            {
                                updatedChange.DevOpsNumber = reader.GetString(devOpsNumberOrdinal);
                            }

                            var tagIdsOrdinal = reader.GetOrdinal("TagIds");
                            if (!reader.IsDBNull(tagIdsOrdinal))
                            {
                                var tagIds = reader.GetString(tagIdsOrdinal);
                                if (!string.IsNullOrEmpty(tagIds))
                                {
                                    updatedChange.TagIds = tagIds.Split(',')
                                        .Where(t => !string.IsNullOrWhiteSpace(t))
                                        .Select(t => Guid.Parse(t.Trim()))
                                        .ToList();
                                }
                            }

                            return updatedChange;
                        }
                    }
                }
            }

            return null;
        }

        public async Task<bool> DeleteChangeAsync(Guid changeId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_DeleteChange", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ChangeId", changeId);

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