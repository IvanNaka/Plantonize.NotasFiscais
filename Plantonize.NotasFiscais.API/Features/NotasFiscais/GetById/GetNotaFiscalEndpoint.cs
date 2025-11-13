using MediatR;
using Microsoft.AspNetCore.Mvc;
using Plantonize.NotasFiscais.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.GetById;

/// <summary>
/// Endpoint for getting Nota Fiscal by ID using Vertical Slice Architecture
/// </summary>
[ApiController]
[Route("api/v2/notas-fiscais")]
[Produces("application/json")]
[Tags("NotasFiscais V2 (Vertical Slice)")]
public class GetNotaFiscalEndpoint : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<GetNotaFiscalEndpoint> _logger;

    public GetNotaFiscalEndpoint(IMediator mediator, ILogger<GetNotaFiscalEndpoint> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets a Nota Fiscal by ID (Vertical Slice implementation)
    /// </summary>
    /// <param name="id">Nota Fiscal ID</param>
    /// <returns>The Nota Fiscal if found</returns>
    /// <response code="200">Nota Fiscal found</response>
    /// <response code="404">Nota Fiscal not found</response>
    [HttpGet("{id:guid}", Name = "GetById")]
    [SwaggerOperation(
        Summary = "Get Nota Fiscal by ID",
        Description = "Retrieves a Nota Fiscal using Vertical Slice Architecture with MediatR",
        OperationId = "GetNotaFiscalByIdV2",
        Tags = new[] { "NotasFiscais V2 (Vertical Slice)" }
    )]
    [ProducesResponseType(typeof(NotaFiscal), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        _logger.LogInformation("V2 API: Getting Nota Fiscal {Id} via Vertical Slice", id);
        
        var result = await _mediator.Send(new GetNotaFiscalQuery(id));
        
        if (result == null)
        {
            return NotFound(new { message = $"Nota Fiscal with ID {id} not found" });
        }
            
        return Ok(result);
    }
}
