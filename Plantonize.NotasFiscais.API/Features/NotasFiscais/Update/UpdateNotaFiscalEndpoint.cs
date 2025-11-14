using MediatR;
using Microsoft.AspNetCore.Mvc;
using Plantonize.NotasFiscais.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.Update;

/// <summary>
/// Endpoint for updating Nota Fiscal using Vertical Slice Architecture
/// </summary>
[ApiController]
[Route("api/v2/notas-fiscais")]
[Produces("application/json")]
[Tags("NotasFiscais V2 (Vertical Slice)")]
public class UpdateNotaFiscalEndpoint : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UpdateNotaFiscalEndpoint> _logger;

    public UpdateNotaFiscalEndpoint(IMediator mediator, ILogger<UpdateNotaFiscalEndpoint> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Updates an existing Nota Fiscal (Vertical Slice implementation)
    /// </summary>
    /// <param name="id">Nota Fiscal ID</param>
    /// <param name="notaFiscal">Updated Nota Fiscal data</param>
    /// <returns>The updated Nota Fiscal</returns>
    /// <response code="200">Nota Fiscal updated successfully</response>
    /// <response code="404">Nota Fiscal not found</response>
    /// <response code="400">Invalid request data</response>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(
        Summary = "Update a Nota Fiscal",
        Description = "Updates an existing Nota Fiscal using Vertical Slice Architecture with MediatR and FluentValidation",
        OperationId = "UpdateNotaFiscalV2",
        Tags = new[] { "NotasFiscais V2 (Vertical Slice)" }
    )]
    [ProducesResponseType(typeof(NotaFiscal), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<NotaFiscal>> Update(Guid id, [FromBody] NotaFiscal notaFiscal)
    {
        try
        {
            if (id != notaFiscal.Id)
            {
                _logger.LogWarning("V2 API: ID mismatch - Route: {RouteId}, Body: {BodyId}", id, notaFiscal.Id);
                return BadRequest("ID mismatch");
            }

            // Validar ModelState
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("V2 API: Invalid model state when updating nota fiscal {Id}", id);
                
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e => new
                    {
                        Field = x.Key,
                        Error = e.ErrorMessage
                    }))
                    .ToList();

                return BadRequest(new
                {
                    message = "Dados inválidos",
                    errors
                });
            }

            _logger.LogInformation("V2 API: Updating Nota Fiscal {Id} via Vertical Slice", id);
            
            // Converter NotaFiscal para Command
            var command = new UpdateNotaFiscalCommand(
                notaFiscal.Id,
                notaFiscal.NumeroNota,
                notaFiscal.DataEmissao,
                notaFiscal.ValorTotal,
                notaFiscal.MunicipioPrestacao,
                notaFiscal.IssRetido,
                notaFiscal.Status,
                notaFiscal.Medico,
                notaFiscal.Tomador,
                notaFiscal.Servicos,
                notaFiscal.EnviadoEmail,
                notaFiscal.DataEnvioEmail
            );
            
            var updated = await _mediator.Send(command);
            
            if (updated == null)
            {
                _logger.LogWarning("V2 API: Nota Fiscal {Id} not found", id);
                return NotFound($"Nota fiscal with ID {id} not found");
            }
            
            _logger.LogInformation("V2 API: Nota Fiscal {Id} updated successfully", id);
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "V2 API: Invalid argument when updating nota fiscal {Id}", id);
            return BadRequest(ex.Message);
        }
        catch (FluentValidation.ValidationException validationEx)
        {
            _logger.LogWarning(validationEx, "V2 API: Validation failed when updating nota fiscal {Id}", id);
            var errors = validationEx.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "V2 API: Nota fiscal {Id} not found", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "V2 API: Error updating nota fiscal {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data");
        }
    }
}
