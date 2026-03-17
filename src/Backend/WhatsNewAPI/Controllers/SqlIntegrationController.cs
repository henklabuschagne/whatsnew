using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsNewAPI.DTOs;
using WhatsNewAPI.Models;
using WhatsNewAPI.Repositories;
using WhatsNewAPI.Services;

namespace WhatsNewAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class SqlIntegrationController : ControllerBase
    {
        private readonly ISqlIntegrationRepository _repository;
        private readonly ISqlIntegrationService _service;

        public SqlIntegrationController(
            ISqlIntegrationRepository repository,
            ISqlIntegrationService service)
        {
            _repository = repository;
            _service = service;
        }

        // SQL Connections
        [HttpGet("connections")]
        public async Task<IActionResult> GetAllConnections()
        {
            try
            {
                var connections = await _repository.GetAllConnectionsAsync();
                var connectionDtos = connections.Select(c => new SqlConnectionDto
                {
                    ConnectionId = c.ConnectionId,
                    Name = c.Name,
                    Server = c.Server,
                    Database = c.Database,
                    Username = c.Username,
                    UseIntegratedSecurity = c.UseIntegratedSecurity,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                });

                return Ok(connectionDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving connections", error = ex.Message });
            }
        }

        [HttpGet("connections/{id}")]
        public async Task<IActionResult> GetConnectionById(Guid id)
        {
            try
            {
                var connection = await _repository.GetConnectionByIdAsync(id);

                if (connection == null)
                {
                    return NotFound(new { message = "Connection not found" });
                }

                var connectionDto = new SqlConnectionDto
                {
                    ConnectionId = connection.ConnectionId,
                    Name = connection.Name,
                    Server = connection.Server,
                    Database = connection.Database,
                    Username = connection.Username,
                    UseIntegratedSecurity = connection.UseIntegratedSecurity,
                    IsActive = connection.IsActive,
                    CreatedAt = connection.CreatedAt,
                    UpdatedAt = connection.UpdatedAt
                };

                return Ok(connectionDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the connection", error = ex.Message });
            }
        }

        [HttpPost("connections")]
        public async Task<IActionResult> CreateConnection([FromBody] CreateSqlConnectionDto createDto)
        {
            try
            {
                if (string.IsNullOrEmpty(createDto.Name) || string.IsNullOrEmpty(createDto.Server) || string.IsNullOrEmpty(createDto.Database))
                {
                    return BadRequest(new { message = "Name, Server, and Database are required" });
                }

                var connection = new SqlConnection
                {
                    Name = createDto.Name,
                    Server = createDto.Server,
                    Database = createDto.Database,
                    Username = createDto.Username,
                    Password = createDto.Password,
                    UseIntegratedSecurity = createDto.UseIntegratedSecurity,
                    IsActive = createDto.IsActive
                };

                var createdConnection = await _repository.CreateConnectionAsync(connection);

                if (createdConnection == null)
                {
                    return BadRequest(new { message = "Failed to create connection" });
                }

                var connectionDto = new SqlConnectionDto
                {
                    ConnectionId = createdConnection.ConnectionId,
                    Name = createdConnection.Name,
                    Server = createdConnection.Server,
                    Database = createdConnection.Database,
                    Username = createdConnection.Username,
                    UseIntegratedSecurity = createdConnection.UseIntegratedSecurity,
                    IsActive = createdConnection.IsActive,
                    CreatedAt = createdConnection.CreatedAt,
                    UpdatedAt = createdConnection.UpdatedAt
                };

                return CreatedAtAction(nameof(GetConnectionById), new { id = connectionDto.ConnectionId }, connectionDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the connection", error = ex.Message });
            }
        }

        [HttpPut("connections/{id}")]
        public async Task<IActionResult> UpdateConnection(Guid id, [FromBody] UpdateSqlConnectionDto updateDto)
        {
            try
            {
                if (string.IsNullOrEmpty(updateDto.Name) || string.IsNullOrEmpty(updateDto.Server) || string.IsNullOrEmpty(updateDto.Database))
                {
                    return BadRequest(new { message = "Name, Server, and Database are required" });
                }

                var connection = new SqlConnection
                {
                    ConnectionId = id,
                    Name = updateDto.Name,
                    Server = updateDto.Server,
                    Database = updateDto.Database,
                    Username = updateDto.Username,
                    Password = updateDto.Password,
                    UseIntegratedSecurity = updateDto.UseIntegratedSecurity,
                    IsActive = updateDto.IsActive
                };

                var updatedConnection = await _repository.UpdateConnectionAsync(connection);

                if (updatedConnection == null)
                {
                    return NotFound(new { message = "Connection not found" });
                }

                var connectionDto = new SqlConnectionDto
                {
                    ConnectionId = updatedConnection.ConnectionId,
                    Name = updatedConnection.Name,
                    Server = updatedConnection.Server,
                    Database = updatedConnection.Database,
                    Username = updatedConnection.Username,
                    UseIntegratedSecurity = updatedConnection.UseIntegratedSecurity,
                    IsActive = updatedConnection.IsActive,
                    CreatedAt = updatedConnection.CreatedAt,
                    UpdatedAt = updatedConnection.UpdatedAt
                };

                return Ok(connectionDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the connection", error = ex.Message });
            }
        }

        [HttpDelete("connections/{id}")]
        public async Task<IActionResult> DeleteConnection(Guid id)
        {
            try
            {
                var success = await _repository.DeleteConnectionAsync(id);

                if (!success)
                {
                    return NotFound(new { message = "Connection not found" });
                }

                return Ok(new { message = "Connection deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the connection", error = ex.Message });
            }
        }

        [HttpPost("connections/test")]
        public async Task<IActionResult> TestConnection([FromBody] CreateSqlConnectionDto connectionDto)
        {
            try
            {
                var connection = new SqlConnection
                {
                    Server = connectionDto.Server,
                    Database = connectionDto.Database,
                    Username = connectionDto.Username,
                    Password = connectionDto.Password,
                    UseIntegratedSecurity = connectionDto.UseIntegratedSecurity
                };

                var result = await _service.TestConnectionAsync(connection);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while testing the connection", error = ex.Message });
            }
        }

        // SQL Queries
        [HttpGet("queries")]
        public async Task<IActionResult> GetAllQueries()
        {
            try
            {
                var queries = await _repository.GetAllQueriesAsync();
                var queryDtos = queries.Select(q => new SqlQueryDto
                {
                    QueryId = q.QueryId,
                    ConnectionId = q.ConnectionId,
                    Name = q.Name,
                    Description = q.Description,
                    QueryText = q.QueryText,
                    IsActive = q.IsActive,
                    CreatedAt = q.CreatedAt,
                    UpdatedAt = q.UpdatedAt,
                    ConnectionName = q.ConnectionName
                });

                return Ok(queryDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving queries", error = ex.Message });
            }
        }

        [HttpGet("queries/{id}")]
        public async Task<IActionResult> GetQueryById(Guid id)
        {
            try
            {
                var query = await _repository.GetQueryByIdAsync(id);

                if (query == null)
                {
                    return NotFound(new { message = "Query not found" });
                }

                var queryDto = new SqlQueryDto
                {
                    QueryId = query.QueryId,
                    ConnectionId = query.ConnectionId,
                    Name = query.Name,
                    Description = query.Description,
                    QueryText = query.QueryText,
                    IsActive = query.IsActive,
                    CreatedAt = query.CreatedAt,
                    UpdatedAt = query.UpdatedAt,
                    ConnectionName = query.ConnectionName
                };

                return Ok(queryDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the query", error = ex.Message });
            }
        }

        [HttpPost("queries")]
        public async Task<IActionResult> CreateQuery([FromBody] CreateSqlQueryDto createDto)
        {
            try
            {
                if (string.IsNullOrEmpty(createDto.Name) || string.IsNullOrEmpty(createDto.QueryText))
                {
                    return BadRequest(new { message = "Name and QueryText are required" });
                }

                var query = new SqlQuery
                {
                    ConnectionId = createDto.ConnectionId,
                    Name = createDto.Name,
                    Description = createDto.Description,
                    QueryText = createDto.QueryText,
                    IsActive = createDto.IsActive
                };

                var createdQuery = await _repository.CreateQueryAsync(query);

                if (createdQuery == null)
                {
                    return BadRequest(new { message = "Failed to create query" });
                }

                var queryDto = new SqlQueryDto
                {
                    QueryId = createdQuery.QueryId,
                    ConnectionId = createdQuery.ConnectionId,
                    Name = createdQuery.Name,
                    Description = createdQuery.Description,
                    QueryText = createdQuery.QueryText,
                    IsActive = createdQuery.IsActive,
                    CreatedAt = createdQuery.CreatedAt,
                    UpdatedAt = createdQuery.UpdatedAt,
                    ConnectionName = createdQuery.ConnectionName
                };

                return CreatedAtAction(nameof(GetQueryById), new { id = queryDto.QueryId }, queryDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the query", error = ex.Message });
            }
        }

        [HttpPut("queries/{id}")]
        public async Task<IActionResult> UpdateQuery(Guid id, [FromBody] UpdateSqlQueryDto updateDto)
        {
            try
            {
                if (string.IsNullOrEmpty(updateDto.Name) || string.IsNullOrEmpty(updateDto.QueryText))
                {
                    return BadRequest(new { message = "Name and QueryText are required" });
                }

                var query = new SqlQuery
                {
                    QueryId = id,
                    Name = updateDto.Name,
                    Description = updateDto.Description,
                    QueryText = updateDto.QueryText,
                    IsActive = updateDto.IsActive
                };

                var updatedQuery = await _repository.UpdateQueryAsync(query);

                if (updatedQuery == null)
                {
                    return NotFound(new { message = "Query not found" });
                }

                var queryDto = new SqlQueryDto
                {
                    QueryId = updatedQuery.QueryId,
                    ConnectionId = updatedQuery.ConnectionId,
                    Name = updatedQuery.Name,
                    Description = updatedQuery.Description,
                    QueryText = updatedQuery.QueryText,
                    IsActive = updatedQuery.IsActive,
                    CreatedAt = updatedQuery.CreatedAt,
                    UpdatedAt = updatedQuery.UpdatedAt,
                    ConnectionName = updatedQuery.ConnectionName
                };

                return Ok(queryDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the query", error = ex.Message });
            }
        }

        [HttpDelete("queries/{id}")]
        public async Task<IActionResult> DeleteQuery(Guid id)
        {
            try
            {
                var success = await _repository.DeleteQueryAsync(id);

                if (!success)
                {
                    return NotFound(new { message = "Query not found" });
                }

                return Ok(new { message = "Query deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the query", error = ex.Message });
            }
        }

        [HttpPost("queries/{id}/execute")]
        public async Task<IActionResult> ExecuteQuery(Guid id)
        {
            try
            {
                var result = await _service.ExecuteQueryAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while executing the query", error = ex.Message });
            }
        }
    }
}
