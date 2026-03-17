using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhatsNewAPI.DTOs;

namespace WhatsNewAPI.Repositories
{
    public interface IReleaseNoteRepository
    {
        Task<IEnumerable<ReleaseNoteDto>> GetReleaseNotesByChangeIdAsync(Guid changeId);
        Task<ReleaseNoteDownloadDto> GetReleaseNoteByIdAsync(Guid releaseNoteId);
        Task<ReleaseNoteDto> CreateReleaseNoteAsync(CreateReleaseNoteDto dto);
        Task<bool> DeleteReleaseNoteAsync(Guid releaseNoteId);
        Task<IEnumerable<ReleaseNoteDto>> GetAllReleaseNotesAsync(int topN = 100);
        Task<int> GetReleaseNotesCountAsync(Guid changeId);
    }
}
