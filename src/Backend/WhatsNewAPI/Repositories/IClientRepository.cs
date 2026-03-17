using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhatsNewAPI.DTOs;
using WhatsNewAPI.Models;

namespace WhatsNewAPI.Repositories
{
    public interface IClientRepository
    {
        Task<IEnumerable<ClientDto>> GetAllClientsAsync(bool includeInactive = false);
        Task<ClientDto?> GetClientByIdAsync(Guid clientId);
        Task<ClientDto?> GetClientByCodeAsync(string code);
        Task<ClientDto> CreateClientAsync(ClientCreateDto createDto);
        Task<ClientDto> UpdateClientAsync(Guid clientId, ClientUpdateDto updateDto);
        Task DeleteClientAsync(Guid clientId);
        Task<ClientStatisticsDto?> GetClientStatisticsAsync(Guid clientId);
    }

    public interface ITimeToActionRepository
    {
        Task<TimeToActionDto?> GetTimeToActionByChangeAsync(Guid changeId);
        Task<TimeToActionDto> UpdateTimeToActionAsync(TimeToActionUpdateDto updateDto);
    }
}
