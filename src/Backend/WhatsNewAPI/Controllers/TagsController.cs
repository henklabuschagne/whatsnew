using System;
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
    public class TagsController : ControllerBase
    {
        private readonly ITagRepository _tagRepository;

        public TagsController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTags([FromQuery] string type = null)
        {
            try
            {
                var tags = string.IsNullOrEmpty(type)
                    ? await _tagRepository.GetAllTagsAsync()
                    : await _tagRepository.GetTagsByTypeAsync(type);

                var tagDtos = tags.Select(t => new TagDto
                {
                    TagId = t.TagId,
                    Label = t.Label,
                    Value = t.Value,
                    Type = t.Type,
                    CreatedAt = t.CreatedAt
                });

                return Ok(tagDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving tags", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTagById(Guid id)
        {
            try
            {
                var tag = await _tagRepository.GetTagByIdAsync(id);

                if (tag == null)
                {
                    return NotFound(new { message = "Tag not found" });
                }

                var tagDto = new TagDto
                {
                    TagId = tag.TagId,
                    Label = tag.Label,
                    Value = tag.Value,
                    Type = tag.Type,
                    CreatedAt = tag.CreatedAt
                };

                return Ok(tagDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the tag", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagDto createTagDto)
        {
            try
            {
                if (string.IsNullOrEmpty(createTagDto.Label) || string.IsNullOrEmpty(createTagDto.Value))
                {
                    return BadRequest(new { message = "Label and Value are required" });
                }

                var tag = new Tag
                {
                    Label = createTagDto.Label,
                    Value = createTagDto.Value,
                    Type = createTagDto.Type ?? "module"
                };

                var createdTag = await _tagRepository.CreateTagAsync(tag);

                if (createdTag == null)
                {
                    return BadRequest(new { message = "Failed to create tag" });
                }

                var tagDto = new TagDto
                {
                    TagId = createdTag.TagId,
                    Label = createdTag.Label,
                    Value = createdTag.Value,
                    Type = createdTag.Type,
                    CreatedAt = createdTag.CreatedAt
                };

                return CreatedAtAction(nameof(GetTagById), new { id = tagDto.TagId }, tagDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the tag", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateTag(Guid id, [FromBody] UpdateTagDto updateTagDto)
        {
            try
            {
                if (string.IsNullOrEmpty(updateTagDto.Label) || string.IsNullOrEmpty(updateTagDto.Value))
                {
                    return BadRequest(new { message = "Label and Value are required" });
                }

                var tag = new Tag
                {
                    TagId = id,
                    Label = updateTagDto.Label,
                    Value = updateTagDto.Value,
                    Type = updateTagDto.Type ?? "module"
                };

                var updatedTag = await _tagRepository.UpdateTagAsync(tag);

                if (updatedTag == null)
                {
                    return NotFound(new { message = "Tag not found" });
                }

                var tagDto = new TagDto
                {
                    TagId = updatedTag.TagId,
                    Label = updatedTag.Label,
                    Value = updatedTag.Value,
                    Type = updatedTag.Type,
                    CreatedAt = updatedTag.CreatedAt
                };

                return Ok(tagDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the tag", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteTag(Guid id)
        {
            try
            {
                var success = await _tagRepository.DeleteTagAsync(id);

                if (!success)
                {
                    return NotFound(new { message = "Tag not found or cannot be deleted" });
                }

                return Ok(new { message = "Tag deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the tag", error = ex.Message });
            }
        }
    }
}
