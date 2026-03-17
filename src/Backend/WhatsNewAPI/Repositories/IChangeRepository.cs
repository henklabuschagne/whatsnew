using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhatsNewAPI.Models;

namespace WhatsNewAPI.Repositories
{
    public interface IChangeRepository
    {
        Task<IEnumerable<Change>> GetChangesByReleaseIdAsync(Guid releaseId);
        Task<Change> GetChangeByIdAsync(Guid changeId);
        Task<Change> CreateChangeAsync(Change change);
        Task<Change> UpdateChangeAsync(Change change);
        Task<bool> DeleteChangeAsync(Guid changeId);
    }
}
