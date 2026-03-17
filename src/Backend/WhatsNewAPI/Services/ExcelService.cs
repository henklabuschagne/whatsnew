using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using WhatsNewAPI.DTOs;
using WhatsNewAPI.Models;
using WhatsNewAPI.Repositories;

namespace WhatsNewAPI.Services
{
    public class ExcelService : IExcelService
    {
        private readonly IReleaseRepository _releaseRepository;
        private readonly IChangeRepository _changeRepository;
        private readonly ITagRepository _tagRepository;

        public ExcelService(
            IReleaseRepository releaseRepository,
            IChangeRepository changeRepository,
            ITagRepository tagRepository)
        {
            _releaseRepository = releaseRepository;
            _changeRepository = changeRepository;
            _tagRepository = tagRepository;
            
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<ExcelImportResultDto> ImportFromExcelAsync(Stream excelStream)
        {
            var result = new ExcelImportResultDto { Success = true, Message = "Import completed successfully" };

            try
            {
                // Load all tags first
                var allTags = await _tagRepository.GetAllTagsAsync();
                var tagDict = allTags.ToDictionary(t => t.Value.ToLower(), t => t);

                using (var package = new ExcelPackage(excelStream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension?.Rows ?? 0;

                    if (rowCount < 2)
                    {
                        result.Success = false;
                        result.Message = "Excel file is empty or has no data rows";
                        return result;
                    }

                    // Group rows by version
                    var releaseGroups = new Dictionary<string, List<ExcelReleaseRow>>();

                    // Start from row 2 (skip header)
                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            var version = worksheet.Cells[row, 1].Value?.ToString();
                            var releaseDateStr = worksheet.Cells[row, 2].Value?.ToString();
                            var changeType = worksheet.Cells[row, 3].Value?.ToString();
                            var description = worksheet.Cells[row, 4].Value?.ToString();
                            var moduleTags = worksheet.Cells[row, 5].Value?.ToString();

                            if (string.IsNullOrWhiteSpace(version) || string.IsNullOrWhiteSpace(description))
                            {
                                result.Errors.Add($"Row {row}: Missing required fields (Version or Description)");
                                continue;
                            }

                            DateTime releaseDate;
                            if (!DateTime.TryParse(releaseDateStr, out releaseDate))
                            {
                                releaseDate = DateTime.UtcNow;
                            }

                            var excelRow = new ExcelReleaseRow
                            {
                                Version = version,
                                ReleaseDate = releaseDate,
                                ChangeType = changeType ?? "new-feature",
                                Description = description,
                                ModuleTags = moduleTags ?? ""
                            };

                            if (!releaseGroups.ContainsKey(version))
                            {
                                releaseGroups[version] = new List<ExcelReleaseRow>();
                            }
                            releaseGroups[version].Add(excelRow);
                        }
                        catch (Exception ex)
                        {
                            result.Errors.Add($"Row {row}: {ex.Message}");
                        }
                    }

                    // Create releases and changes
                    foreach (var group in releaseGroups)
                    {
                        try
                        {
                            var firstRow = group.Value.First();
                            
                            // Create release
                            var release = new Release
                            {
                                Version = group.Key,
                                ReleaseDate = firstRow.ReleaseDate
                            };

                            var createdRelease = await _releaseRepository.CreateReleaseAsync(release);
                            if (createdRelease != null)
                            {
                                result.ReleasesImported++;

                                // Create changes for this release
                                foreach (var row in group.Value)
                                {
                                    try
                                    {
                                        // Parse module tags
                                        var tagIds = new List<Guid>();
                                        if (!string.IsNullOrWhiteSpace(row.ModuleTags))
                                        {
                                            var tagNames = row.ModuleTags.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                                            foreach (var tagName in tagNames)
                                            {
                                                var normalizedTag = tagName.Trim().ToLower();
                                                if (tagDict.ContainsKey(normalizedTag))
                                                {
                                                    tagIds.Add(tagDict[normalizedTag].TagId);
                                                }
                                            }
                                        }

                                        var change = new Change
                                        {
                                            ReleaseId = createdRelease.ReleaseId,
                                            Description = row.Description,
                                            ChangeType = NormalizeChangeType(row.ChangeType),
                                            TagIds = tagIds
                                        };

                                        var createdChange = await _changeRepository.CreateChangeAsync(change);
                                        if (createdChange != null)
                                        {
                                            result.ChangesImported++;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        result.Errors.Add($"Failed to create change for version {group.Key}: {ex.Message}");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            result.Errors.Add($"Failed to create release {group.Key}: {ex.Message}");
                        }
                    }

                    if (result.Errors.Count > 0)
                    {
                        result.Success = false;
                        result.Message = $"Import completed with {result.Errors.Count} errors";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Failed to import Excel file: {ex.Message}";
                result.Errors.Add(ex.Message);
            }

            return result;
        }

        public async Task<byte[]> ExportToExcelAsync()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Releases");

                // Add headers
                worksheet.Cells[1, 1].Value = "Version";
                worksheet.Cells[1, 2].Value = "Release Date";
                worksheet.Cells[1, 3].Value = "Change Type";
                worksheet.Cells[1, 4].Value = "Description";
                worksheet.Cells[1, 5].Value = "Module Tags";

                // Style headers
                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Get all releases with changes
                var releases = await _releaseRepository.GetAllReleasesAsync();
                var allTags = await _tagRepository.GetAllTagsAsync();
                var tagDict = allTags.ToDictionary(t => t.TagId, t => t);

                int row = 2;
                foreach (var release in releases)
                {
                    var changes = await _changeRepository.GetChangesByReleaseIdAsync(release.ReleaseId);

                    if (!changes.Any())
                    {
                        // Add release without changes
                        worksheet.Cells[row, 1].Value = release.Version;
                        worksheet.Cells[row, 2].Value = release.ReleaseDate.ToString("yyyy-MM-dd");
                        worksheet.Cells[row, 3].Value = "";
                        worksheet.Cells[row, 4].Value = "";
                        worksheet.Cells[row, 5].Value = "";
                        row++;
                    }
                    else
                    {
                        foreach (var change in changes)
                        {
                            worksheet.Cells[row, 1].Value = release.Version;
                            worksheet.Cells[row, 2].Value = release.ReleaseDate.ToString("yyyy-MM-dd");
                            worksheet.Cells[row, 3].Value = GetChangeTypeLabel(change.ChangeType);
                            worksheet.Cells[row, 4].Value = change.Description;

                            // Get tag values
                            var tagValues = change.TagIds
                                .Where(tagId => tagDict.ContainsKey(tagId))
                                .Select(tagId => tagDict[tagId].Value);
                            worksheet.Cells[row, 5].Value = string.Join(", ", tagValues);

                            row++;
                        }
                    }
                }

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                return package.GetAsByteArray();
            }
        }

        private string NormalizeChangeType(string changeType)
        {
            if (string.IsNullOrWhiteSpace(changeType))
                return "new-feature";

            var normalized = changeType.ToLower().Trim();
            
            if (normalized.Contains("bug") || normalized.Contains("fix"))
                return "bug-fix";
            if (normalized.Contains("enhance") || normalized.Contains("improve"))
                return "enhancement";
            if (normalized.Contains("new") || normalized.Contains("feature"))
                return "new-feature";

            return "new-feature";
        }

        private string GetChangeTypeLabel(string changeType)
        {
            switch (changeType)
            {
                case "bug-fix": return "Bug Fix";
                case "new-feature": return "New Feature";
                case "enhancement": return "Enhancement";
                default: return changeType;
            }
        }
    }
}
