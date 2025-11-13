using MediatR;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;
using Plantonize.NotasFiscais.Domain.Enum;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.Create;

/// <summary>
/// Handler for creating a new Nota Fiscal
/// </summary>
public class CreateNotaFiscalHandler : IRequestHandler<CreateNotaFiscalCommand, NotaFiscal>
{
    private readonly INotaFiscalRepository _repository;
    private readonly ILogger<CreateNotaFiscalHandler> _logger;

    public CreateNotaFiscalHandler(
        INotaFiscalRepository repository,
        ILogger<CreateNotaFiscalHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<NotaFiscal> Handle(CreateNotaFiscalCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating Nota Fiscal {NumeroNota} via Vertical Slice", request.NumeroNota);

        var notaFiscal = new NotaFiscal
        {
            Id = Guid.NewGuid(),
            NumeroNota = request.NumeroNota,
            DataEmissao = request.DataEmissao,
            ValorTotal = request.ValorTotal,
            MunicipioPrestacao = request.MunicipioPrestacao,
            IssRetido = request.IssRetido,
            Medico = request.Medico,
            Tomador = request.Tomador,
            Servicos = request.Servicos ?? new List<ItemServico>(),
            Status = StatusNFSEEnum.Emitida,
            EnviadoEmail = false
        };

        var result = await _repository.CreateAsync(notaFiscal);
        
        _logger.LogInformation(
            "Nota Fiscal {Id} created successfully via Vertical Slice. NumeroNota: {NumeroNota}", 
            result.Id, 
            result.NumeroNota);

        return result;
    }
}
