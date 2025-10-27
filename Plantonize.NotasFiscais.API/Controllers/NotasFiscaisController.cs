using Microsoft.AspNetCore.Mvc;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plantonize.NotasFiscais.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class NotasFiscaisController : ControllerBase
    {
        private readonly INotaFiscalService _notaFiscalService;
        private readonly ILogger<NotasFiscaisController> _logger;

        public NotasFiscaisController(
            INotaFiscalService notaFiscalService,
            ILogger<NotasFiscaisController> logger)
        {
            _notaFiscalService = notaFiscalService;
            _logger = logger;
        }

        /// <summary>
        /// Get all notas fiscais
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NotaFiscal>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<NotaFiscal>>> GetAll()
        {
            try
            {
                var notasFiscais = await _notaFiscalService.GetAllAsync();
                return Ok(notasFiscais);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all notas fiscais");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        /// <summary>
        /// Get nota fiscal by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NotaFiscal), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NotaFiscal>> GetById(Guid id)
        {
            try
            {
                var notaFiscal = await _notaFiscalService.GetByIdAsync(id);
                
                if (notaFiscal == null)
                    return NotFound($"Nota fiscal with ID {id} not found");

                return Ok(notaFiscal);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting nota fiscal {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        /// <summary>
        /// Get notas fiscais by medico ID
        /// </summary>
        [HttpGet("medico/{medicoId}")]
        [ProducesResponseType(typeof(IEnumerable<NotaFiscal>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<NotaFiscal>>> GetByMedicoId(Guid medicoId)
        {
            try
            {
                var notasFiscais = await _notaFiscalService.GetByMedicoIdAsync(medicoId);
                return Ok(notasFiscais);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notas fiscais for medico {MedicoId}", medicoId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        /// <summary>
        /// Create a new nota fiscal
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(NotaFiscal), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NotaFiscal>> Create([FromBody] NotaFiscal notaFiscal)
        {
            try
            {
                var created = await _notaFiscalService.CreateAsync(notaFiscal);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating nota fiscal");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating data");
            }
        }

        /// <summary>
        /// Update an existing nota fiscal
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(NotaFiscal), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NotaFiscal>> Update(Guid id, [FromBody] NotaFiscal notaFiscal)
        {
            try
            {
                if (id != notaFiscal.Id)
                    return BadRequest("ID mismatch");

                var updated = await _notaFiscalService.UpdateAsync(notaFiscal);
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
                _logger.LogError(ex, "Error updating nota fiscal {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data");
            }
        }

        /// <summary>
        /// Delete a nota fiscal
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _notaFiscalService.DeleteAsync(id);
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
                _logger.LogError(ex, "Error deleting nota fiscal {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data");
            }
        }

        /// <summary>
        /// Check if nota fiscal exists
        /// </summary>
        [HttpGet("{id}/exists")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> Exists(Guid id)
        {
            try
            {
                var exists = await _notaFiscalService.ExistsAsync(id);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if nota fiscal {Id} exists", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error checking data");
            }
        }
    }
}
