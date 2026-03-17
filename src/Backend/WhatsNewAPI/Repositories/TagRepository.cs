using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WhatsNewAPI.Models;

namespace WhatsNewAPI.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly string _connectionString;

        public TagRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            var tags = new List<Tag>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetAllTags", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tags.Add(new Tag
                            {
                                TagId = reader.GetGuid(reader.GetOrdinal("TagId")),
                                Label = reader.GetString(reader.GetOrdinal("Label")),
                                Value = reader.GetString(reader.GetOrdinal("Value")),
                                Type = reader.GetString(reader.GetOrdinal("Type")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                            });
                        }
                    }
                }
            }

            return tags;
        }

        public async Task<Tag> GetTagByIdAsync(Guid tagId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetTagById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TagId", tagId);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Tag
                            {
                                TagId = reader.GetGuid(reader.GetOrdinal("TagId")),
                                Label = reader.GetString(reader.GetOrdinal("Label")),
                                Value = reader.GetString(reader.GetOrdinal("Value")),
                                Type = reader.GetString(reader.GetOrdinal("Type")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<IEnumerable<Tag>> GetTagsByTypeAsync(string type)
        {
            var tags = new List<Tag>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetTagsByType", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Type", type);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tags.Add(new Tag
                            {
                                TagId = reader.GetGuid(reader.GetOrdinal("TagId")),
                                Label = reader.GetString(reader.GetOrdinal("Label")),
                                Value = reader.GetString(reader.GetOrdinal("Value")),
                                Type = reader.GetString(reader.GetOrdinal("Type")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                            });
                        }
                    }
                }
            }

            return tags;
        }

        public async Task<Tag> CreateTagAsync(Tag tag)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_CreateTag", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    var tagIdParam = new SqlParameter("@TagId", SqlDbType.UniqueIdentifier)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(tagIdParam);

                    command.Parameters.AddWithValue("@Label", tag.Label);
                    command.Parameters.AddWithValue("@Value", tag.Value);
                    command.Parameters.AddWithValue("@Type", tag.Type);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Tag
                            {
                                TagId = reader.GetGuid(reader.GetOrdinal("TagId")),
                                Label = reader.GetString(reader.GetOrdinal("Label")),
                                Value = reader.GetString(reader.GetOrdinal("Value")),
                                Type = reader.GetString(reader.GetOrdinal("Type")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<Tag> UpdateTagAsync(Tag tag)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_UpdateTag", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TagId", tag.TagId);
                    command.Parameters.AddWithValue("@Label", tag.Label);
                    command.Parameters.AddWithValue("@Value", tag.Value);
                    command.Parameters.AddWithValue("@Type", tag.Type);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Tag
                            {
                                TagId = reader.GetGuid(reader.GetOrdinal("TagId")),
                                Label = reader.GetString(reader.GetOrdinal("Label")),
                                Value = reader.GetString(reader.GetOrdinal("Value")),
                                Type = reader.GetString(reader.GetOrdinal("Type")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<bool> DeleteTagAsync(Guid tagId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_DeleteTag", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TagId", tagId);

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
