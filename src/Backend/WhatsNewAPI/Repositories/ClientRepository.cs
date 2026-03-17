using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using WhatsNewAPI.DTOs;
using WhatsNewAPI.Models;

namespace WhatsNewAPI.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly string _connectionString;

        public ClientRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<IEnumerable<ClientDto>> GetAllClientsAsync(bool includeInactive = false)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new { IncludeInactive = includeInactive };
            
            return await connection.QueryAsync<ClientDto>(
                "sp_GetAllClients",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<ClientDto?> GetClientByIdAsync(Guid clientId)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new { ClientId = clientId };
            
            return await connection.QueryFirstOrDefaultAsync<ClientDto>(
                "sp_GetClientById",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<ClientDto?> GetClientByCodeAsync(string code)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new { Code = code };
            
            return await connection.QueryFirstOrDefaultAsync<ClientDto>(
                "sp_GetClientByCode",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<ClientDto> CreateClientAsync(ClientCreateDto createDto)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new
            {
                Name = createDto.Name,
                Code = createDto.Code,
                ContactEmail = createDto.ContactEmail,
                ContactPhone = createDto.ContactPhone,
                IsActive = createDto.IsActive
            };
            
            var result = await connection.QueryFirstOrDefaultAsync<ClientDto>(
                "sp_CreateClient",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result ?? throw new InvalidOperationException("Failed to create client");
        }

        public async Task<ClientDto> UpdateClientAsync(Guid clientId, ClientUpdateDto updateDto)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new
            {
                ClientId = clientId,
                Name = updateDto.Name,
                Code = updateDto.Code,
                ContactEmail = updateDto.ContactEmail,
                ContactPhone = updateDto.ContactPhone,
                IsActive = updateDto.IsActive
            };
            
            var result = await connection.QueryFirstOrDefaultAsync<ClientDto>(
                "sp_UpdateClient",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result ?? throw new InvalidOperationException("Failed to update client");
        }

        public async Task DeleteClientAsync(Guid clientId)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new { ClientId = clientId };
            
            await connection.ExecuteAsync(
                "sp_DeleteClient",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<ClientStatisticsDto?> GetClientStatisticsAsync(Guid clientId)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new { ClientId = clientId };
            
            return await connection.QueryFirstOrDefaultAsync<ClientStatisticsDto>(
                "sp_GetClientStatistics",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }

    public class TimeToActionRepository : ITimeToActionRepository
    {
        private readonly string _connectionString;

        public TimeToActionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<TimeToActionDto?> GetTimeToActionByChangeAsync(Guid changeId)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new { ChangeId = changeId };
            
            return await connection.QueryFirstOrDefaultAsync<TimeToActionDto>(
                "sp_GetTimeToActionByChange",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<TimeToActionDto> UpdateTimeToActionAsync(TimeToActionUpdateDto updateDto)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new
            {
                ChangeId = updateDto.ChangeId,
                SubmittedDate = updateDto.SubmittedDate,
                DevelopedDate = updateDto.DevelopedDate,
                TestedDate = updateDto.TestedDate,
                ReleasedDate = updateDto.ReleasedDate
            };
            
            var result = await connection.QueryFirstOrDefaultAsync<TimeToActionDto>(
                "sp_UpdateTimeToAction",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result ?? throw new InvalidOperationException("Failed to update time to action");
        }
    }
}
