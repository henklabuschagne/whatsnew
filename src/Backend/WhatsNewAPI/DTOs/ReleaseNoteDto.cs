using System;

namespace WhatsNewAPI.DTOs
{
    /// <summary>
    /// DTO for Release Note file metadata (without file data)
    /// Used for listing release notes
    /// </summary>
    public class ReleaseNoteDto
    {
        public Guid ReleaseNoteId { get; set; }
        public Guid ChangeId { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public string FileExtension { get; set; }
        public Guid? UploadedBy { get; set; }
        public string UploadedByName { get; set; }
        public DateTime UploadedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for downloading a release note (includes file data)
    /// </summary>
    public class ReleaseNoteDownloadDto
    {
        public Guid ReleaseNoteId { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public string FileExtension { get; set; }
        public byte[] FileData { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    /// <summary>
    /// DTO for creating/uploading a release note
    /// </summary>
    public class CreateReleaseNoteDto
    {
        public Guid ChangeId { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public string FileExtension { get; set; }
        public byte[] FileData { get; set; }
        public Guid? UploadedBy { get; set; }
    }

    /// <summary>
    /// DTO for upload response
    /// </summary>
    public class ReleaseNoteUploadResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ReleaseNoteDto ReleaseNote { get; set; }
    }
}
