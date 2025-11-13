using MediatR;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.List;

/// <summary>
/// Handler for listing all Notas Fiscais
/// </summary>
public class ListNotasFiscaisHandler : IRequestHandler<ListNotasFiscaisQuery, IEnumerable<NotaFiscal>>
{
    private readonly INotaFiscalRepository _repository;
    private readonly ILogger<ListNotasFiscaisHandler> _logger;

    public ListNotasFiscaisHandler(INotaFiscalRepository repository, ILogger<ListNotasFiscaisHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<NotaFiscal>> Handle(ListNotasFiscaisQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listing all Notas Fiscais via Vertical Slice");

        var result = await _repository.GetAllAsync();

        _logger.LogInformation("Found {Count} Notas Fiscais", result.Count());

        return result;
    }
}
