using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WhatsNewAPI.DTOs;

namespace WhatsNewAPI.Repositories
{
    public class ReleaseNoteRepository : IReleaseNoteRepository
    {
        private readonly string _connectionString;

        public ReleaseNoteRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<ReleaseNoteDto>> GetReleaseNotesByChangeIdAsync(Guid changeId)
        {
            var releaseNotes = new List<ReleaseNoteDto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetReleaseNotesByChangeId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ChangeId", changeId);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            releaseNotes.Add(MapToReleaseNoteDto(reader));
                        }
                    }
                }
            }

            return releaseNotes;
        }

        public async Task<ReleaseNoteDownloadDto> GetReleaseNoteByIdAsync(Guid releaseNoteId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetReleaseNoteById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ReleaseNoteId", releaseNoteId);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new ReleaseNoteDownloadDto
                            {
                                ReleaseNoteId = reader.GetGuid(reader.GetOrdinal("ReleaseNoteId")),
                                FileName = reader.GetString(reader.GetOrdinal("FileName")),
                                FileSize = reader.GetInt64(reader.GetOrdinal("FileSize")),
                                FileType = reader.GetString(reader.GetOrdinal("FileType")),
                                FileExtension = reader.GetString(reader.GetOrdinal("FileExtension")),
                                FileData = (byte[])reader["FileData"],
                                UploadedAt = reader.GetDateTime(reader.GetOrdinal("UploadedAt"))
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<ReleaseNoteDto> CreateReleaseNoteAsync(CreateReleaseNoteDto dto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_CreateReleaseNote", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ChangeId", dto.ChangeId);
                    command.Parameters.AddWithValue("@FileName", dto.FileName);
                    command.Parameters.AddWithValue("@FileSize", dto.FileSize);
                    command.Parameters.AddWithValue("@FileType", dto.FileType);
                    command.Parameters.AddWithValue("@FileExtension", dto.FileExtension);
                    command.Parameters.AddWithValue("@FileData", dto.FileData);
                    command.Parameters.AddWithValue("@UploadedBy", 
                        dto.UploadedBy.HasValue ? (object)dto.UploadedBy.Value : DBNull.Value);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapToReleaseNoteDto(reader);
                        }
                    }
                }
            }

            return null;
        }

        public async Task<bool> DeleteReleaseNoteAsync(Guid releaseNoteId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_DeleteReleaseNote", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ReleaseNoteId", releaseNoteId);

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

        public async Task<IEnumerable<ReleaseNoteDto>> GetAllReleaseNotesAsync(int topN = 100)
        {
            var releaseNotes = new List<ReleaseNoteDto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetAllReleaseNotes", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TopN", topN);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            releaseNotes.Add(MapToReleaseNoteDto(reader));
                        }
                    }
                }
            }

            return releaseNotes;
        }

        public async Task<int> GetReleaseNotesCountAsync(Guid changeId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetReleaseNotesCount", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ChangeId", changeId);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return reader.GetInt32(reader.GetOrdinal("NoteCount"));
                        }
                    }
                }
            }

            return 0;
        }

        private ReleaseNoteDto MapToReleaseNoteDto(SqlDataReader reader)
        {
            var dto = new ReleaseNoteDto
            {
                ReleaseNoteId = reader.GetGuid(reader.GetOrdinal("ReleaseNoteId")),
                ChangeId = reader.GetGuid(reader.GetOrdinal("ChangeId")),
                FileName = reader.GetString(reader.GetOrdinal("FileName")),
                FileSize = reader.GetInt64(reader.GetOrdinal("FileSize")),
                FileType = reader.GetString(reader.GetOrdinal("FileType")),
                FileExtension = reader.GetString(reader.GetOrdinal("FileExtension")),
                UploadedAt = reader.GetDateTime(reader.GetOrdinal("UploadedAt")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            };

            // Optional fields
            var uploadedByOrdinal = reader.GetOrdinal("UploadedBy");
            if (!reader.IsDBNull(uploadedByOrdinal))
            {
                dto.UploadedBy = reader.GetGuid(uploadedByOrdinal);
            }

            var uploadedByNameOrdinal = reader.GetOrdinal("UploadedByName");
            if (!reader.IsDBNull(uploadedByNameOrdinal))
            {
                dto.UploadedByName = reader.GetString(uploadedByNameOrdinal);
            }

            return dto;
        }
    }
}
