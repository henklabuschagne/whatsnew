using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsNewAPI.DTOs;
using WhatsNewAPI.Models;
using WhatsNewAPI.Repositories;

namespace WhatsNewAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReleasesController : ControllerBase
    {
        private readonly IReleaseRepository _releaseRepository;
        private readonly IChangeRepository _changeRepository;
        private readonly ITagRepository _tagRepository;

        public ReleasesController(
            IReleaseRepository releaseRepository,
            IChangeRepository changeRepository,
            ITagRepository tagRepository)
        {
            _releaseRepository = releaseRepository;
            _changeRepository = changeRepository;
            _tagRepository = tagRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReleases([FromQuery] bool includeChanges = true)
        {
            try
            {
                var releases = await _releaseRepository.GetAllReleasesAsync();
                var releaseDtos = releases.Select(r => new ReleaseDto
                {
                    ReleaseId = r.ReleaseId,
                    Version = r.Version,
                    ReleaseDate = r.ReleaseDate,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                }).ToList();

                if (includeChanges)
                {
                    var allTags = await _tagRepository.GetAllTagsAsync();
                    var tagDict = allTags.ToDictionary(t => t.TagId, t => t);

                    foreach (var releaseDto in releaseDtos)
                    {
                        var changes = await _changeRepository.GetChangesByReleaseIdAsync(releaseDto.ReleaseId);
                        releaseDto.Changes = changes.Select(c => new ChangeDto
                        {
                            ChangeId = c.ChangeId,
                            ReleaseId = c.ReleaseId,
                            Description = c.Description,
                            ChangeType = c.ChangeType,
                            CreatedAt = c.CreatedAt,
                            TagIds = c.TagIds,
                            ModuleTags = c.TagIds
                                .Where(tagId => tagDict.ContainsKey(tagId))
                                .Select(tagId => tagDict[tagId].Value)
                                .ToList()
                        }).ToList();
                    }
                }

                return Ok(releaseDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving releases", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReleaseById(Guid id, [FromQuery] bool includeChanges = true)
        {
            try
            {
                var release = await _releaseRepository.GetReleaseByIdAsync(id);

                if (release == null)
                {
                    return NotFound(new { message = "Release not found" });
                }

                var releaseDto = new ReleaseDto
                {
                    ReleaseId = release.ReleaseId,
                    Version = release.Version,
                    ReleaseDate = release.ReleaseDate,
                    CreatedAt = release.CreatedAt,
                    UpdatedAt = release.UpdatedAt
                };

                if (includeChanges)
                {
                    var changes = await _changeRepository.GetChangesByReleaseIdAsync(release.ReleaseId);
                    var allTags = await _tagRepository.GetAllTagsAsync();
                    var tagDict = allTags.ToDictionary(t => t.TagId, t => t);

                    releaseDto.Changes = changes.Select(c => new ChangeDto
                    {
                        ChangeId = c.ChangeId,
                        ReleaseId = c.ReleaseId,
                        Description = c.Description,
                        ChangeType = c.ChangeType,
                        CreatedAt = c.CreatedAt,
                        TagIds = c.TagIds,
                        ModuleTags = c.TagIds
                            .Where(tagId => tagDict.ContainsKey(tagId))
                            .Select(tagId => tagDict[tagId].Value)
                            .ToList()
                    }).ToList();
                }

                return Ok(releaseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the release", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateRelease([FromBody] CreateReleaseDto createReleaseDto)
        {
            try
            {
                if (string.IsNullOrEmpty(createReleaseDto.Version))
                {
                    return BadRequest(new { message = "Version is required" });
                }

                var release = new Release
                {
                    Version = createReleaseDto.Version,
                    ReleaseDate = createReleaseDto.ReleaseDate
                };

                var createdRelease = await _releaseRepository.CreateReleaseAsync(release);

                if (createdRelease == null)
                {
                    return BadRequest(new { message = "Failed to create release" });
                }

                var releaseDto = new ReleaseDto
                {
                    ReleaseId = createdRelease.ReleaseId,
                    Version = createdRelease.Version,
                    ReleaseDate = createdRelease.ReleaseDate,
                    CreatedAt = createdRelease.CreatedAt,
                    UpdatedAt = createdRelease.UpdatedAt
                };

                return CreatedAtAction(nameof(GetReleaseById), new { id = releaseDto.ReleaseId }, releaseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the release", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateRelease(Guid id, [FromBody] UpdateReleaseDto updateReleaseDto)
        {
            try
            {
                if (string.IsNullOrEmpty(updateReleaseDto.Version))
                {
                    return BadRequest(new { message = "Version is required" });
                }

                var release = new Release
                {
                    ReleaseId = id,
                    Version = updateReleaseDto.Version,
                    ReleaseDate = updateReleaseDto.ReleaseDate
                };

                var updatedRelease = await _releaseRepository.UpdateReleaseAsync(release);

                if (updatedRelease == null)
                {
                    return NotFound(new { message = "Release not found" });
                }

                var releaseDto = new ReleaseDto
                {
                    ReleaseId = updatedRelease.ReleaseId,
                    Version = updatedRelease.Version,
                    ReleaseDate = updatedRelease.ReleaseDate,
                    CreatedAt = updatedRelease.CreatedAt,
                    UpdatedAt = updatedRelease.UpdatedAt
                };

                return Ok(releaseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the release", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteRelease(Guid id)
        {
            try
            {
                var success = await _releaseRepository.DeleteReleaseAsync(id);

                if (!success)
                {
                    return NotFound(new { message = "Release not found" });
                }

                return Ok(new { message = "Release deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the release", error = ex.Message });
            }
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetReleasesWithFilters([FromQuery] ReleaseFilterDto filter)
        {
            try
            {
                var releases = await _releaseRepository.GetReleasesWithFiltersAsync(filter);

                if (Request.Query.ContainsKey("includeChanges") && Request.Query["includeChanges"] == "true")
                {
                    var releaseDtos = new List<ReleaseDto>();

                    foreach (var release in releases)
                    {
                        var changes = await _changeRepository.GetChangesByReleaseIdAsync(release.ReleaseId);
                        releaseDtos.Add(new ReleaseDto
                        {
                            ReleaseId = release.ReleaseId,
                            Version = release.Version,
                            ReleaseDate = release.ReleaseDate,
                            CreatedAt = release.CreatedAt,
                            UpdatedAt = release.UpdatedAt,
                            Changes = changes.Select(c => new ChangeDto
                            {
                                ChangeId = c.ChangeId,
                                ReleaseId = c.ReleaseId,
                                Description = c.Description,
                                ChangeType = c.ChangeType,
                                TagIds = c.TagIds,
                                CreatedAt = c.CreatedAt,
                                UpdatedAt = c.UpdatedAt
                            }).ToList()
                        });
                    }

                    return Ok(releaseDtos);
                }
                else
                {
                    var releaseDtos = releases.Select(r => new ReleaseDto
                    {
                        ReleaseId = r.ReleaseId,
                        Version = r.Version,
                        ReleaseDate = r.ReleaseDate,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt,
                        Changes = new List<ChangeDto>()
                    });

                    return Ok(releaseDtos);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving releases", error = ex.Message });
            }
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var statistics = await _releaseRepository.GetReleaseStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving statistics", error = ex.Message });
            }
        }

        [HttpGet("popular-tags")]
        public async Task<IActionResult> GetPopularTags([FromQuery] int topN = 10)
        {
            try
            {
                var tags = await _releaseRepository.GetPopularTagsAsync(topN);
                return Ok(tags);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving popular tags", error = ex.Message });
            }
        }

        [HttpGet("versions")]
        public async Task<IActionResult> GetVersionList()
        {
            try
            {
                var versions = await _releaseRepository.GetVersionListAsync();
                return Ok(versions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving versions", error = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchChanges([FromQuery] string q)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return BadRequest(new { message = "Search term is required" });
                }

                var results = await _releaseRepository.SearchChangesAsync(q);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching", error = ex.Message });
            }
        }
    }
}