using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.Delete;

/// <summary>
/// Endpoint for deleting Nota Fiscal using Vertical Slice Architecture
/// </summary>
[ApiController]
[Route("api/v2/notas-fiscais")]
[Produces("application/json")]
[Tags("NotasFiscais V2 (Vertical Slice)")]
public class DeleteNotaFiscalEndpoint : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DeleteNotaFiscalEndpoint> _logger;

    public DeleteNotaFiscalEndpoint(IMediator mediator, ILogger<DeleteNotaFiscalEndpoint> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Deletes a Nota Fiscal (Vertical Slice implementation)
    /// </summary>
    /// <param name="id">Nota Fiscal ID</param>
    /// <returns>Success status</returns>
    /// <response code="204">Nota Fiscal deleted successfully</response>
    /// <response code="404">Nota Fiscal not found</response>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "Delete a Nota Fiscal",
        Description = "Deletes a Nota Fiscal using Vertical Slice Architecture with MediatR",
        OperationId = "DeleteNotaFiscalV2",
        Tags = new[] { "NotasFiscais V2 (Vertical Slice)" }
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation("V2 API: Deleting Nota Fiscal {Id} via Vertical Slice", id);
        
        var result = await _mediator.Send(new DeleteNotaFiscalCommand(id));
        
        if (!result)
        {
            return NotFound(new { message = $"Nota Fiscal with ID {id} not found" });
        }
        
        return NoContent();
    }
}
