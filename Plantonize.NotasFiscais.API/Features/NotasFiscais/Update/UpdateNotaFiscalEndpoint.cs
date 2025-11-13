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
    /// <param name="command">Updated Nota Fiscal data</param>
    /// <returns>The updated Nota Fiscal</returns>
    /// <response code="200">Nota Fiscal updated successfully</response>
    /// <response code="404">Nota Fiscal not found</response>
    /// <response code="400">Invalid request data</response>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(
        Summary = "Update a Nota Fiscal",
        Description = "Updates an existing Nota Fiscal using Vertical Slice Architecture with MediatR",
        OperationId = "UpdateNotaFiscalV2",
        Tags = new[] { "NotasFiscais V2 (Vertical Slice)" }
    )]
    [ProducesResponseType(typeof(NotaFiscal), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNotaFiscalCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { error = "ID mismatch between route and body" });
        }

        try
        {
            _logger.LogInformation("V2 API: Updating Nota Fiscal {Id} via Vertical Slice", id);
            
            var result = await _mediator.Send(command);
            
            if (result == null)
            {
                return NotFound(new { message = $"Nota Fiscal with ID {id} not found" });
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Nota Fiscal {Id} via V2 API", id);
            return BadRequest(new { error = ex.Message });
        }
    }
}
