using MediatR;
using Microsoft.AspNetCore.Mvc;
using Plantonize.NotasFiscais.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.List;

/// <summary>
/// Endpoint for listing all Notas Fiscais using Vertical Slice Architecture
/// </summary>
[ApiController]
[Route("api/v2/notas-fiscais")]
[Produces("application/json")]
[Tags("NotasFiscais V2 (Vertical Slice)")]
public class ListNotasFiscaisEndpoint : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ListNotasFiscaisEndpoint> _logger;

    public ListNotasFiscaisEndpoint(IMediator mediator, ILogger<ListNotasFiscaisEndpoint> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Lists all Notas Fiscais (Vertical Slice implementation)
    /// </summary>
    /// <returns>List of all Notas Fiscais</returns>
    /// <response code="200">List of Notas Fiscais retrieved successfully</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "List all Notas Fiscais",
        Description = "Retrieves all Notas Fiscais using Vertical Slice Architecture with MediatR",
        OperationId = "ListNotasFiscaisV2",
        Tags = new[] { "NotasFiscais V2 (Vertical Slice)" }
    )]
    [ProducesResponseType(typeof(IEnumerable<NotaFiscal>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<NotaFiscal>>> List()
    {
        try
        {
            _logger.LogInformation("V2 API: Listing all Notas Fiscais via Vertical Slice");
            
            var notasFiscais = await _mediator.Send(new ListNotasFiscaisQuery());
            
            return Ok(notasFiscais);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "V2 API: Error getting all notas fiscais");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
        }
    }
}
