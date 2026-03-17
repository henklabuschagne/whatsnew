using WhatsNewAPI.Models.Entities;

namespace WhatsNewAPI.Repositories.Interfaces;

public interface IChangeRepository
{
    Task<Change?> GetByIdAsync(int changeId);
    Task<List<Change>> GetByReleaseIdAsync(int releaseId);
    Task<int> CreateAsync(Change change, List<string> moduleTags, int createdBy);
    Task<bool> UpdateAsync(int changeId, Change change, List<string> moduleTags, int updatedBy);
    Task<bool> DeleteAsync(int changeId, int deletedBy);
}
