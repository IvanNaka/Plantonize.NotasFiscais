using MediatR;
using Plantonize.NotasFiscais.Domain.Interfaces;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.Delete;

/// <summary>
/// Handler for deleting a Nota Fiscal
/// </summary>
public class DeleteNotaFiscalHandler : IRequestHandler<DeleteNotaFiscalCommand, bool>
{
    private readonly INotaFiscalRepository _repository;
    private readonly ILogger<DeleteNotaFiscalHandler> _logger;

    public DeleteNotaFiscalHandler(
        INotaFiscalRepository repository,
        ILogger<DeleteNotaFiscalHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteNotaFiscalCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting Nota Fiscal {Id} via Vertical Slice", request.Id);

        var existing = await _repository.GetByIdAsync(request.Id);
        
        if (existing == null)
        {
            _logger.LogWarning("Nota Fiscal {Id} not found for deletion", request.Id);
            return false;
        }

        await _repository.DeleteAsync(request.Id);
        
        _logger.LogInformation("Nota Fiscal {Id} deleted successfully via Vertical Slice", request.Id);

        return true;
    }
}
