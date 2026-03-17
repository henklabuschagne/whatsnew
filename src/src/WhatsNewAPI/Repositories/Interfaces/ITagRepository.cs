using WhatsNewAPI.Models.Entities;

namespace WhatsNewAPI.Repositories.Interfaces;

public interface ITagRepository
{
    Task<List<Tag>> GetAllAsync(bool activeOnly = true);
    Task<Tag?> GetByIdAsync(int tagId);
    Task<Tag?> GetByValueAsync(string tagValue);
    Task<int> CreateAsync(Tag tag, int createdBy);
    Task<bool> UpdateAsync(int tagId, Tag tag, int updatedBy);
    Task<bool> DeleteAsync(int tagId, int deletedBy);
}
