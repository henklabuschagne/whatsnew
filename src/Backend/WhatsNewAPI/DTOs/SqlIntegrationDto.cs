using System;

namespace WhatsNewAPI.DTOs
{
    public class SqlConnectionDto
    {
        public Guid ConnectionId { get; set; }
        public string Name { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public bool UseIntegratedSecurity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateSqlConnectionDto
    {
        public string Name { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseIntegratedSecurity { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateSqlConnectionDto
    {
        public string Name { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseIntegratedSecurity { get; set; }
        public bool IsActive { get; set; }
    }

    public class SqlQueryDto
    {
        public Guid QueryId { get; set; }
        public Guid ConnectionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string QueryText { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string ConnectionName { get; set; }
    }

    public class CreateSqlQueryDto
    {
        public Guid ConnectionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string QueryText { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateSqlQueryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string QueryText { get; set; }
        public bool IsActive { get; set; }
    }

    public class TestConnectionDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class ExecuteQueryResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int ReleasesImported { get; set; }
        public int ChangesImported { get; set; }
    }
}
