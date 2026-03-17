using WhatsNewAPI.Models.DTOs.Changes;
using WhatsNewAPI.Models.Entities;
using WhatsNewAPI.Repositories.Interfaces;
using WhatsNewAPI.Services.Interfaces;

namespace WhatsNewAPI.Services.Implementations;

public class ChangeService : IChangeService
{
    private readonly IChangeRepository _changeRepository;
    private readonly IAuditService _auditService;

    public ChangeService(
        IChangeRepository changeRepository,
        IAuditService auditService)
    {
        _changeRepository = changeRepository;
        _auditService = auditService;
    }

    public async Task<ChangeDto?> GetByIdAsync(int changeId)
    {
        var change = await _changeRepository.GetByIdAsync(changeId);
        
        if (change == null)
        {
            return null;
        }

        return new ChangeDto
        {
            ChangeId = change.ChangeId,
            ReleaseId = change.ReleaseId,
            Description = change.Description,
            ChangeType = change.ChangeType,
            ModuleTags = change.ModuleTags,
            CreatedAt = change.CreatedAt,
            UpdatedAt = change.UpdatedAt
        };
    }

    public async Task<List<ChangeDto>> GetByReleaseIdAsync(int releaseId)
    {
        var changes = await _changeRepository.GetByReleaseIdAsync(releaseId);
        
        return changes.Select(c => new ChangeDto
        {
            ChangeId = c.ChangeId,
            ReleaseId = c.ReleaseId,
            Description = c.Description,
            ChangeType = c.ChangeType,
            ModuleTags = c.ModuleTags,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();
    }

    public async Task<ChangeDto> CreateChangeAsync(CreateChangeDto request, int userId)
    {
        var change = new Change
        {
            ReleaseId = request.ReleaseId,
            Description = request.Description,
            ChangeType = request.ChangeType
        };

        var changeId = await _changeRepository.CreateAsync(change, request.ModuleTags, userId);
        await _auditService.LogActionAsync(userId, "CREATE", "Change", changeId, null, $"Type: {request.ChangeType}");

        var createdChange = await _changeRepository.GetByIdAsync(changeId);
        
        return new ChangeDto
        {
            ChangeId = createdChange!.ChangeId,
            ReleaseId = createdChange.ReleaseId,
            Description = createdChange.Description,
            ChangeType = createdChange.ChangeType,
            ModuleTags = createdChange.ModuleTags,
            CreatedAt = createdChange.CreatedAt,
            UpdatedAt = createdChange.UpdatedAt
        };
    }

    public async Task<bool> UpdateChangeAsync(int changeId, UpdateChangeDto request, int userId)
    {
        var change = new Change
        {
            Description = request.Description,
            ChangeType = request.ChangeType
        };

        var success = await _changeRepository.UpdateAsync(changeId, change, request.ModuleTags, userId);
        
        if (success)
        {
            await _auditService.LogActionAsync(userId, "UPDATE", "Change", changeId, null, $"Type: {request.ChangeType}");
        }

        return success;
    }

    public async Task<bool> DeleteChangeAsync(int changeId, int userId)
    {
        var success = await _changeRepository.DeleteAsync(changeId, userId);
        
        if (success)
        {
            await _auditService.LogActionAsync(userId, "DELETE", "Change", changeId);
        }

        return success;
    }
}
