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
    /// <param name="notaFiscal">Nota Fiscal data</param>
    /// <returns>The created Nota Fiscal</returns>
    /// <response code="201">Nota Fiscal created successfully</response>
    /// <response code="400">Invalid request data</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new Nota Fiscal",
        Description = "Creates a new Nota Fiscal using Vertical Slice Architecture with MediatR and FluentValidation",
        OperationId = "CreateNotaFiscalV2",
        Tags = new[] { "NotasFiscais V2 (Vertical Slice)" }
    )]
    [ProducesResponseType(typeof(NotaFiscal), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<NotaFiscal>> Create([FromBody] NotaFiscal notaFiscal)
    {
        try
        {
            _logger.LogInformation("========== V2 API: Starting nota fiscal creation ==========");
            _logger.LogInformation("Received NotaFiscal: NumeroNota={NumeroNota}, ValorTotal={ValorTotal}", 
                notaFiscal?.NumeroNota, notaFiscal?.ValorTotal);

            // Validar ModelState (JSON malformado, tipos incompatíveis, etc)
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("V2 API: Invalid model state when creating nota fiscal");
                
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e => new
                    {
                        Field = x.Key,
                        Error = e.ErrorMessage
                    }))
                    .ToList();

                _logger.LogWarning("ModelState errors: {@Errors}", errors);

                return BadRequest(new
                {
                    message = "Dados inválidos",
                    errors
                });
            }

            _logger.LogInformation("V2 API: ModelState is valid");
            _logger.LogInformation("V2 API: Creating command from NotaFiscal");
            
            // Converter NotaFiscal para Command
            var command = new CreateNotaFiscalCommand(
                notaFiscal.NumeroNota,
                notaFiscal.DataEmissao,
                notaFiscal.ValorTotal,
                notaFiscal.MunicipioPrestacao,
                notaFiscal.IssRetido,
                notaFiscal.Medico,
                notaFiscal.Tomador,
                notaFiscal.Servicos
            );
            
            _logger.LogInformation("V2 API: Command created successfully");
            _logger.LogInformation("V2 API: Sending command to MediatR");
            
            var created = await _mediator.Send(command);
            
            _logger.LogInformation("V2 API: Nota Fiscal created successfully with ID {Id}", created.Id);
            
            // Retornar 201 Created com a nota fiscal criada
            return StatusCode(StatusCodes.Status201Created, created);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "V2 API: Invalid argument when creating nota fiscal");
            return BadRequest(new { message = ex.Message, type = "ArgumentException" });
        }
        catch (FluentValidation.ValidationException validationEx)
        {
            _logger.LogWarning(validationEx, "V2 API: Validation failed when creating nota fiscal");
            var errors = validationEx.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
            _logger.LogWarning("Validation errors: {@Errors}", errors);
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "V2 API: EXCEPTION DETAILS - Type: {Type}, Message: {Message}, StackTrace: {StackTrace}", 
                ex.GetType().Name, ex.Message, ex.StackTrace);
            
            if (ex.InnerException != null)
            {
                _logger.LogError("V2 API: INNER EXCEPTION - Type: {Type}, Message: {Message}", 
                    ex.InnerException.GetType().Name, ex.InnerException.Message);
            }
            
            return StatusCode(StatusCodes.Status500InternalServerError, new 
            { 
                message = "Error creating data",
                error = ex.Message,
                type = ex.GetType().Name,
                innerError = ex.InnerException?.Message
            });
        }
    }
}
