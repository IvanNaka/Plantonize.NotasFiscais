using MediatR;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.Update;

/// <summary>
/// Handler for updating a Nota Fiscal
/// </summary>
public class UpdateNotaFiscalHandler : IRequestHandler<UpdateNotaFiscalCommand, NotaFiscal?>
{
    private readonly INotaFiscalRepository _repository;
    private readonly ILogger<UpdateNotaFiscalHandler> _logger;

    public UpdateNotaFiscalHandler(
        INotaFiscalRepository repository,
        ILogger<UpdateNotaFiscalHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<NotaFiscal?> Handle(UpdateNotaFiscalCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating Nota Fiscal {Id} via Vertical Slice", request.Id);

        var existing = await _repository.GetByIdAsync(request.Id);
        
        if (existing == null)
        {
            _logger.LogWarning("Nota Fiscal {Id} not found for update", request.Id);
            return null;
        }

        // Update only provided fields
        if (!string.IsNullOrEmpty(request.NumeroNota))
            existing.NumeroNota = request.NumeroNota;
        
        if (request.DataEmissao.HasValue)
            existing.DataEmissao = request.DataEmissao.Value;
        
        if (request.ValorTotal.HasValue)
            existing.ValorTotal = request.ValorTotal.Value;
        
        if (!string.IsNullOrEmpty(request.MunicipioPrestacao))
            existing.MunicipioPrestacao = request.MunicipioPrestacao;
        
        if (request.IssRetido.HasValue)
            existing.IssRetido = request.IssRetido.Value;
        
        if (request.Status.HasValue)
            existing.Status = request.Status.Value;
        
        if (request.Medico != null)
            existing.Medico = request.Medico;
        
        if (request.Tomador != null)
            existing.Tomador = request.Tomador;
        
        if (request.Servicos != null)
            existing.Servicos = request.Servicos;
        
        if (request.EnviadoEmail.HasValue)
            existing.EnviadoEmail = request.EnviadoEmail.Value;
        
        if (request.DataEnvioEmail.HasValue)
            existing.DataEnvioEmail = request.DataEnvioEmail;

        var result = await _repository.UpdateAsync(existing);
        
        _logger.LogInformation("Nota Fiscal {Id} updated successfully via Vertical Slice", request.Id);

        return result;
    }
}
