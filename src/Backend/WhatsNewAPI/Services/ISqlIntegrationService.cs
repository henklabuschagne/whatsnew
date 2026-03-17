using System;
using System.Threading.Tasks;
using WhatsNewAPI.DTOs;
using WhatsNewAPI.Models;

namespace WhatsNewAPI.Services
{
    public interface ISqlIntegrationService
    {
        Task<TestConnectionDto> TestConnectionAsync(SqlConnection connection);
        Task<ExecuteQueryResultDto> ExecuteQueryAsync(Guid queryId);
    }
}
