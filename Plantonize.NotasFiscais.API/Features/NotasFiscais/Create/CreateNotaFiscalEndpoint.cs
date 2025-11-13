using MediatR;
using Microsoft.AspNetCore.Mvc;
using Plantonize.NotasFiscais.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.Create;

/// <summary>
/// Endpoint for creating Nota Fiscal using Vertical Slice Architecture
/// </summary>
[ApiController]
[Route("api/v2/notas-fiscais")]
[Produces("application/json")]
[Tags("NotasFiscais V2 (Vertical Slice)")]
public class CreateNotaFiscalEndpoint : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateNotaFiscalEndpoint> _logger;

    public CreateNotaFiscalEndpoint(IMediator mediator, ILogger<CreateNotaFiscalEndpoint> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new Nota Fiscal (Vertical Slice implementation)
    /// </summary>
    /// <param name="command">Nota Fiscal data</param>
    /// <returns>The created Nota Fiscal</returns>
    /// <response code="201">Nota Fiscal created successfully</response>
    /// <response code="400">Invalid request data</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new Nota Fiscal",
        Description = "Creates a new Nota Fiscal using Vertical Slice Architecture with MediatR",
        OperationId = "CreateNotaFiscalV2",
        Tags = new[] { "NotasFiscais V2 (Vertical Slice)" }
    )]
    [ProducesResponseType(typeof(NotaFiscal), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateNotaFiscalCommand command)
    {
        try
        {
            _logger.LogInformation("V2 API: Creating Nota Fiscal via Vertical Slice");
            
            var result = await _mediator.Send(command);
            
            return CreatedAtAction(
                actionName: "GetById",
                controllerName: "GetNotaFiscal",
                routeValues: new { id = result.Id },
                value: result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Nota Fiscal via V2 API");
            return BadRequest(new { error = ex.Message });
        }
    }
}
