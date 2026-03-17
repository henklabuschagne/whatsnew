using WhatsNewAPI.Models.DTOs.Releases;
using WhatsNewAPI.Models.DTOs.Changes;
using WhatsNewAPI.Models.DTOs.Common;
using WhatsNewAPI.Models.Entities;
using WhatsNewAPI.Repositories.Interfaces;
using WhatsNewAPI.Services.Interfaces;

namespace WhatsNewAPI.Services.Implementations;

public class ReleaseService : IReleaseService
{
    private readonly IReleaseRepository _releaseRepository;
    private readonly IAuditService _auditService;

    public ReleaseService(
        IReleaseRepository releaseRepository,
        IAuditService auditService)
    {
        _releaseRepository = releaseRepository;
        _auditService = auditService;
    }

    public async Task<List<ReleaseDto>> GetAllReleasesAsync(bool includeUnpublished = false)
    {
        var releases = await _releaseRepository.GetAllAsync(includeUnpublished);
        
        return releases.Select(r => new ReleaseDto
        {
            ReleaseId = r.ReleaseId,
            Version = r.Version,
            ReleaseDate = r.ReleaseDate,
            Description = r.Description,
            IsPublished = r.IsPublished,
            ChangeCount = r.ChangeCount,
            CreatedByUsername = r.CreatedByUsername,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        }).ToList();
    }

    public async Task<ReleaseDetailDto?> GetReleaseByIdAsync(int releaseId)
    {
        var release = await _releaseRepository.GetByIdAsync(releaseId);
        
        if (release == null)
        {
            return null;
        }

        return new ReleaseDetailDto
        {
            ReleaseId = release.ReleaseId,
            Version = release.Version,
            ReleaseDate = release.ReleaseDate,
            Description = release.Description,
            IsPublished = release.IsPublished,
            Changes = release.Changes.Select(c => new ChangeDto
            {
                ChangeId = c.ChangeId,
                ReleaseId = c.ReleaseId,
                Description = c.Description,
                ChangeType = c.ChangeType,
                ModuleTags = c.ModuleTags,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList(),
            CreatedByUsername = release.CreatedByUsername,
            CreatedAt = release.CreatedAt,
            UpdatedAt = release.UpdatedAt
        };
    }

    public async Task<ReleaseDto> CreateReleaseAsync(CreateReleaseDto request, int userId)
    {
        var release = new Release
        {
            Version = request.Version,
            ReleaseDate = request.ReleaseDate,
            Description = request.Description,
            IsPublished = request.IsPublished
        };

        var releaseId = await _releaseRepository.CreateAsync(release, userId);
        await _auditService.LogActionAsync(userId, "CREATE", "Release", releaseId, null, $"Version: {request.Version}");

        var createdRelease = await _releaseRepository.GetByIdAsync(releaseId);
        
        return new ReleaseDto
        {
            ReleaseId = createdRelease!.ReleaseId,
            Version = createdRelease.Version,
            ReleaseDate = createdRelease.ReleaseDate,
            Description = createdRelease.Description,
            IsPublished = createdRelease.IsPublished,
            ChangeCount = 0,
            CreatedByUsername = createdRelease.CreatedByUsername,
            CreatedAt = createdRelease.CreatedAt,
            UpdatedAt = createdRelease.UpdatedAt
        };
    }

    public async Task<bool> UpdateReleaseAsync(int releaseId, UpdateReleaseDto request, int userId)
    {
        var release = new Release
        {
            Version = request.Version,
            ReleaseDate = request.ReleaseDate,
            Description = request.Description,
            IsPublished = request.IsPublished
        };

        var success = await _releaseRepository.UpdateAsync(releaseId, release, userId);
        
        if (success)
        {
            await _auditService.LogActionAsync(userId, "UPDATE", "Release", releaseId, null, $"Version: {request.Version}");
        }

        return success;
    }

    public async Task<bool> DeleteReleaseAsync(int releaseId, int userId)
    {
        var success = await _releaseRepository.DeleteAsync(releaseId, userId);
        
        if (success)
        {
            await _auditService.LogActionAsync(userId, "DELETE", "Release", releaseId);
        }

        return success;
    }

    public async Task<StatisticsDto> GetStatisticsAsync()
    {
        var (totalReleases, publishedReleases, totalChanges, bugFixes, newFeatures, enhancements) = 
            await _releaseRepository.GetStatisticsAsync();

        return new StatisticsDto
        {
            TotalReleases = totalReleases,
            PublishedReleases = publishedReleases,
            TotalChanges = totalChanges,
            BugFixes = bugFixes,
            NewFeatures = newFeatures,
            Enhancements = enhancements,
            ModuleStats = new List<ModuleStatDto>()
        };
    }
}
