using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhatsNewAPI.Models;

namespace WhatsNewAPI.Repositories
{
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> GetAllTagsAsync();
        Task<Tag> GetTagByIdAsync(Guid tagId);
        Task<IEnumerable<Tag>> GetTagsByTypeAsync(string type);
        Task<Tag> CreateTagAsync(Tag tag);
        Task<Tag> UpdateTagAsync(Tag tag);
        Task<bool> DeleteTagAsync(Guid tagId);
    }
}
