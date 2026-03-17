using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhatsNewAPI.DTOs;
using WhatsNewAPI.Repositories;

namespace WhatsNewAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(IClientRepository clientRepository, ILogger<ClientsController> logger)
        {
            _clientRepository = clientRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get all clients
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetAllClients([FromQuery] bool includeInactive = false)
        {
            try
            {
                var clients = await _clientRepository.GetAllClientsAsync(includeInactive);
                return Ok(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clients");
                return StatusCode(500, new { message = "An error occurred while retrieving clients" });
            }
        }

        /// <summary>
        /// Get client by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDto>> GetClientById(Guid id)
        {
            try
            {
                var client = await _clientRepository.GetClientByIdAsync(id);
                
                if (client == null)
                    return NotFound(new { message = "Client not found" });

                return Ok(client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving client {ClientId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the client" });
            }
        }

        /// <summary>
        /// Get client by code
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<ActionResult<ClientDto>> GetClientByCode(string code)
        {
            try
            {
                var client = await _clientRepository.GetClientByCodeAsync(code);
                
                if (client == null)
                    return NotFound(new { message = "Client not found" });

                return Ok(client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving client by code {Code}", code);
                return StatusCode(500, new { message = "An error occurred while retrieving the client" });
            }
        }

        /// <summary>
        /// Create a new client
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ClientDto>> CreateClient([FromBody] ClientCreateDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var client = await _clientRepository.CreateClientAsync(createDto);
                return CreatedAtAction(nameof(GetClientById), new { id = client.ClientId }, client);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Failed to create client");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating client");
                return StatusCode(500, new { message = "An error occurred while creating the client" });
            }
        }

        /// <summary>
        /// Update an existing client
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ClientDto>> UpdateClient(Guid id, [FromBody] ClientUpdateDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var client = await _clientRepository.UpdateClientAsync(id, updateDto);
                return Ok(client);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Failed to update client {ClientId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating client {ClientId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the client" });
            }
        }

        /// <summary>
        /// Delete a client
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteClient(Guid id)
        {
            try
            {
                await _clientRepository.DeleteClientAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Failed to delete client {ClientId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting client {ClientId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the client" });
            }
        }

        /// <summary>
        /// Get client statistics
        /// </summary>
        [HttpGet("{id}/statistics")]
        public async Task<ActionResult<ClientStatisticsDto>> GetClientStatistics(Guid id)
        {
            try
            {
                var statistics = await _clientRepository.GetClientStatisticsAsync(id);
                
                if (statistics == null)
                    return NotFound(new { message = "Client not found" });

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving client statistics for {ClientId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving client statistics" });
            }
        }
    }

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TimeToActionController : ControllerBase
    {
        private readonly ITimeToActionRepository _timeToActionRepository;
        private readonly ILogger<TimeToActionController> _logger;

        public TimeToActionController(ITimeToActionRepository timeToActionRepository, ILogger<TimeToActionController> logger)
        {
            _timeToActionRepository = timeToActionRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get time to action for a specific change
        /// </summary>
        [HttpGet("change/{changeId}")]
        public async Task<ActionResult<TimeToActionDto>> GetTimeToActionByChange(Guid changeId)
        {
            try
            {
                var timeToAction = await _timeToActionRepository.GetTimeToActionByChangeAsync(changeId);
                
                if (timeToAction == null)
                    return NotFound(new { message = "Time to action record not found" });

                return Ok(timeToAction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving time to action for change {ChangeId}", changeId);
                return StatusCode(500, new { message = "An error occurred while retrieving time to action data" });
            }
        }

        /// <summary>
        /// Update time to action dates
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult<TimeToActionDto>> UpdateTimeToAction([FromBody] TimeToActionUpdateDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var timeToAction = await _timeToActionRepository.UpdateTimeToActionAsync(updateDto);
                return Ok(timeToAction);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Failed to update time to action");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating time to action");
                return StatusCode(500, new { message = "An error occurred while updating time to action data" });
            }
        }
    }
}
