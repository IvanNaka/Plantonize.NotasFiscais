using MediatR;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.GetById;

/// <summary>
/// Handler for getting a Nota Fiscal by ID
/// </summary>
public class GetNotaFiscalHandler : IRequestHandler<GetNotaFiscalQuery, NotaFiscal?>
{
    private readonly INotaFiscalRepository _repository;
    private readonly ILogger<GetNotaFiscalHandler> _logger;

    public GetNotaFiscalHandler(INotaFiscalRepository repository, ILogger<GetNotaFiscalHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<NotaFiscal?> Handle(GetNotaFiscalQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting Nota Fiscal {Id} via Vertical Slice", request.Id);

        var result = await _repository.GetByIdAsync(request.Id);

        if (result == null)
        {
            _logger.LogWarning("Nota Fiscal {Id} not found", request.Id);
        }

        return result;
    }
}
