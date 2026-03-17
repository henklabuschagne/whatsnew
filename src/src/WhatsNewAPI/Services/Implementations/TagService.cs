using WhatsNewAPI.Models.DTOs.Tags;
using WhatsNewAPI.Models.Entities;
using WhatsNewAPI.Repositories.Interfaces;
using WhatsNewAPI.Services.Interfaces;

namespace WhatsNewAPI.Services.Implementations;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;
    private readonly IAuditService _auditService;

    public TagService(
        ITagRepository tagRepository,
        IAuditService auditService)
    {
        _tagRepository = tagRepository;
        _auditService = auditService;
    }

    public async Task<List<TagDto>> GetAllTagsAsync(bool activeOnly = true)
    {
        var tags = await _tagRepository.GetAllAsync(activeOnly);
        
        return tags.Select(t => new TagDto
        {
            TagId = t.TagId,
            Value = t.TagValue,
            Label = t.TagLabel,
            Type = t.TagType,
            IsActive = t.IsActive
        }).ToList();
    }

    public async Task<TagDto?> GetByIdAsync(int tagId)
    {
        var tag = await _tagRepository.GetByIdAsync(tagId);
        
        if (tag == null)
        {
            return null;
        }

        return new TagDto
        {
            TagId = tag.TagId,
            Value = tag.TagValue,
            Label = tag.TagLabel,
            Type = tag.TagType,
            IsActive = tag.IsActive
        };
    }

    public async Task<TagDto> CreateTagAsync(CreateTagDto request, int userId)
    {
        var tag = new Tag
        {
            TagValue = request.Value,
            TagLabel = request.Label,
            TagType = request.Type
        };

        var tagId = await _tagRepository.CreateAsync(tag, userId);
        await _auditService.LogActionAsync(userId, "CREATE", "Tag", tagId, null, $"Value: {request.Value}");

        var createdTag = await _tagRepository.GetByIdAsync(tagId);
        
        return new TagDto
        {
            TagId = createdTag!.TagId,
            Value = createdTag.TagValue,
            Label = createdTag.TagLabel,
            Type = createdTag.TagType,
            IsActive = createdTag.IsActive
        };
    }

    public async Task<bool> UpdateTagAsync(int tagId, UpdateTagDto request, int userId)
    {
        var tag = new Tag
        {
            TagLabel = request.Label,
            IsActive = request.IsActive
        };

        var success = await _tagRepository.UpdateAsync(tagId, tag, userId);
        
        if (success)
        {
            await _auditService.LogActionAsync(userId, "UPDATE", "Tag", tagId, null, $"Label: {request.Label}");
        }

        return success;
    }

    public async Task<bool> DeleteTagAsync(int tagId, int userId)
    {
        var success = await _tagRepository.DeleteAsync(tagId, userId);
        
        if (success)
        {
            await _auditService.LogActionAsync(userId, "DELETE", "Tag", tagId);
        }

        return success;
    }
}
