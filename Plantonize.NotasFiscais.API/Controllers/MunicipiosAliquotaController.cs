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
    public class MunicipiosAliquotaController : ControllerBase
    {
        private readonly IMunicipioAliquotaService _municipioAliquotaService;
        private readonly ILogger<MunicipiosAliquotaController> _logger;

        public MunicipiosAliquotaController(
            IMunicipioAliquotaService municipioAliquotaService,
            ILogger<MunicipiosAliquotaController> logger)
        {
            _municipioAliquotaService = municipioAliquotaService;
            _logger = logger;
        }

        /// <summary>
        /// Get all municipios with aliquotas
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MunicipioAliquota>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MunicipioAliquota>>> GetAll()
        {
            try
            {
                var municipios = await _municipioAliquotaService.GetAllAsync();
                return Ok(municipios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all municipios aliquota");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        /// <summary>
        /// Get municipio aliquota by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MunicipioAliquota), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MunicipioAliquota>> GetById(Guid id)
        {
            try
            {
                var municipio = await _municipioAliquotaService.GetByIdAsync(id);
                
                if (municipio == null)
                    return NotFound($"Municipio aliquota with ID {id} not found");

                return Ok(municipio);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting municipio aliquota {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        /// <summary>
        /// Get municipio aliquota by codigo municipio
        /// </summary>
        [HttpGet("codigo/{codigoMunicipio}")]
        [ProducesResponseType(typeof(MunicipioAliquota), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MunicipioAliquota>> GetByCodigoMunicipio(string codigoMunicipio)
        {
            try
            {
                var municipio = await _municipioAliquotaService.GetByCodigoMunicipioAsync(codigoMunicipio);
                
                if (municipio == null)
                    return NotFound($"Municipio aliquota with codigo {codigoMunicipio} not found");

                return Ok(municipio);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting municipio aliquota by codigo {CodigoMunicipio}", codigoMunicipio);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        /// <summary>
        /// Create a new municipio aliquota
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(MunicipioAliquota), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MunicipioAliquota>> Create([FromBody] MunicipioAliquota municipioAliquota)
        {
            try
            {
                var created = await _municipioAliquotaService.CreateAsync(municipioAliquota);
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
                _logger.LogError(ex, "Error creating municipio aliquota");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating data");
            }
        }

        /// <summary>
        /// Update an existing municipio aliquota
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(MunicipioAliquota), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MunicipioAliquota>> Update(Guid id, [FromBody] MunicipioAliquota municipioAliquota)
        {
            try
            {
                if (id != municipioAliquota.Id)
                    return BadRequest("ID mismatch");

                var updated = await _municipioAliquotaService.UpdateAsync(municipioAliquota);
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("not found"))
                    return NotFound(ex.Message);
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating municipio aliquota {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data");
            }
        }

        /// <summary>
        /// Delete a municipio aliquota
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _municipioAliquotaService.DeleteAsync(id);
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
                _logger.LogError(ex, "Error deleting municipio aliquota {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data");
            }
        }

        /// <summary>
        /// Check if municipio aliquota exists by ID
        /// </summary>
        [HttpGet("{id}/exists")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> Exists(Guid id)
        {
            try
            {
                var exists = await _municipioAliquotaService.ExistsAsync(id);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if municipio aliquota {Id} exists", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error checking data");
            }
        }

        /// <summary>
        /// Check if municipio aliquota exists by codigo
        /// </summary>
        [HttpGet("codigo/{codigoMunicipio}/exists")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> ExistsByCodigo(string codigoMunicipio)
        {
            try
            {
                var exists = await _municipioAliquotaService.ExistsByCodigoAsync(codigoMunicipio);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if municipio aliquota with codigo {CodigoMunicipio} exists", codigoMunicipio);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error checking data");
            }
        }
    }
}
