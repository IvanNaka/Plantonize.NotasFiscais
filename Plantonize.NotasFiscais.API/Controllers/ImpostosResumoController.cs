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
    public class ImpostosResumoController : ControllerBase
    {
        private readonly IImpostoResumoService _impostoResumoService;
        private readonly ILogger<ImpostosResumoController> _logger;

        public ImpostosResumoController(
            IImpostoResumoService impostoResumoService,
            ILogger<ImpostosResumoController> logger)
        {
            _impostoResumoService = impostoResumoService;
            _logger = logger;
        }

        /// <summary>
        /// Get all impostos resumo
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ImpostoResumo>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ImpostoResumo>>> GetAll()
        {
            try
            {
                var impostos = await _impostoResumoService.GetAllAsync();
                return Ok(impostos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all impostos resumo");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        /// <summary>
        /// Get imposto resumo by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ImpostoResumo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ImpostoResumo>> GetById(Guid id)
        {
            try
            {
                var impostoResumo = await _impostoResumoService.GetByIdAsync(id);
                
                if (impostoResumo == null)
                    return NotFound($"Imposto resumo with ID {id} not found");

                return Ok(impostoResumo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting imposto resumo {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        /// <summary>
        /// Get impostos resumo by medico ID
        /// </summary>
        [HttpGet("medico/{medicoId}")]
        [ProducesResponseType(typeof(IEnumerable<ImpostoResumo>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ImpostoResumo>>> GetByMedicoId(Guid medicoId)
        {
            try
            {
                var impostos = await _impostoResumoService.GetByMedicoIdAsync(medicoId);
                return Ok(impostos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting impostos resumo for medico {MedicoId}", medicoId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        /// <summary>
        /// Get imposto resumo by medico, mes, and ano
        /// </summary>
        [HttpGet("medico/{medicoId}/periodo")]
        [ProducesResponseType(typeof(ImpostoResumo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ImpostoResumo>> GetByMedicoMesAno(
            Guid medicoId, 
            [FromQuery] int mes, 
            [FromQuery] int ano)
        {
            try
            {
                var impostoResumo = await _impostoResumoService.GetByMedicoMesAnoAsync(medicoId, mes, ano);
                
                if (impostoResumo == null)
                    return NotFound($"Imposto resumo for medico {medicoId} in {mes}/{ano} not found");

                return Ok(impostoResumo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting imposto resumo for medico {MedicoId} in {Mes}/{Ano}", medicoId, mes, ano);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        /// <summary>
        /// Create a new imposto resumo
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ImpostoResumo), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ImpostoResumo>> Create([FromBody] ImpostoResumo impostoResumo)
        {
            try
            {
                var created = await _impostoResumoService.CreateAsync(impostoResumo);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating imposto resumo");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating data");
            }
        }

        /// <summary>
        /// Update an existing imposto resumo
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ImpostoResumo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ImpostoResumo>> Update(Guid id, [FromBody] ImpostoResumo impostoResumo)
        {
            try
            {
                if (id != impostoResumo.Id)
                    return BadRequest("ID mismatch");

                var updated = await _impostoResumoService.UpdateAsync(impostoResumo);
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
                _logger.LogError(ex, "Error updating imposto resumo {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data");
            }
        }

        /// <summary>
        /// Delete an imposto resumo
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _impostoResumoService.DeleteAsync(id);
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
                _logger.LogError(ex, "Error deleting imposto resumo {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data");
            }
        }

        /// <summary>
        /// Check if imposto resumo exists
        /// </summary>
        [HttpGet("{id}/exists")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> Exists(Guid id)
        {
            try
            {
                var exists = await _impostoResumoService.ExistsAsync(id);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if imposto resumo {Id} exists", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error checking data");
            }
        }

        /// <summary>
        /// Calculate and save imposto resumo for a specific medico and period
        /// </summary>
        [HttpPost("calcular")]
        [ProducesResponseType(typeof(ImpostoResumo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ImpostoResumo>> CalculateResumo(
            [FromQuery] Guid medicoId, 
            [FromQuery] int mes, 
            [FromQuery] int ano)
        {
            try
            {
                var resumo = await _impostoResumoService.CalculateResumoAsync(medicoId, mes, ano);
                return Ok(resumo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating imposto resumo for medico {MedicoId} in {Mes}/{Ano}", medicoId, mes, ano);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error calculating data");
            }
        }
    }
}
