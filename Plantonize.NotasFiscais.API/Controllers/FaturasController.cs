using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plantonize.NotasFiscais.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class FaturasController : ControllerBase
    {
        private readonly IFaturaService _faturaService;
        private readonly ILogger<FaturasController> _logger;

        public FaturasController(
            IFaturaService faturaService,
            ILogger<FaturasController> logger)
        {
            _faturaService = faturaService;
            _logger = logger;
        }

        /// <summary>
        /// Get all faturas
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Fatura>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Fatura>>> GetAll()
        {
            try
            {
                var faturas = await _faturaService.GetAllAsync();
                return Ok(faturas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all faturas");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        /// <summary>
        /// Get fatura by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Fatura), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Fatura>> GetById(Guid id)
        {
            try
            {
                var fatura = await _faturaService.GetByIdAsync(id);
                
                if (fatura == null)
                    return NotFound($"Fatura with ID {id} not found");

                return Ok(fatura);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting fatura {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        /// <summary>
        /// Get faturas by medico ID
        /// </summary>
        [HttpGet("medico/{medicoId}")]
        [ProducesResponseType(typeof(IEnumerable<Fatura>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Fatura>>> GetByMedicoId(Guid medicoId)
        {
            try
            {
                var faturas = await _faturaService.GetByMedicoIdAsync(medicoId);
                return Ok(faturas);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting faturas for medico {MedicoId}", medicoId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        /// <summary>
        /// Create a new fatura
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Fatura), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Fatura>> Create([FromBody] Fatura fatura)
        {
            try
            {
                var created = await _faturaService.CreateAsync(fatura);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating fatura");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating data");
            }
        }

        /// <summary>
        /// Update an existing fatura
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Fatura), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Fatura>> Update(Guid id, [FromBody] Fatura fatura)
        {
            try
            {
                if (id != fatura.Id)
                    return BadRequest("ID mismatch");

                var updated = await _faturaService.UpdateAsync(fatura);
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating fatura {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data");
            }
        }

        /// <summary>
        /// Delete a fatura
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _faturaService.DeleteAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting fatura {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data");
            }
        }

        /// <summary>
        /// Check if fatura exists
        /// </summary>
        [HttpGet("{id}/exists")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> Exists(Guid id)
        {
            try
            {
                var exists = await _faturaService.ExistsAsync(id);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if fatura {Id} exists", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error checking data");
            }
        }

        /// <summary>
        /// Calculate total value of a fatura
        /// </summary>
        [HttpGet("{id}/total")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<decimal>> CalculateTotalValue(Guid id)
        {
            try
            {
                var total = await _faturaService.CalculateTotalValueAsync(id);
                return Ok(total);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total value for fatura {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error calculating data");
            }
        }
    }
}
