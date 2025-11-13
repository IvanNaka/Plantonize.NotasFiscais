using MediatR;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Enum;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.Update;

/// <summary>
/// Command to update an existing Nota Fiscal
/// </summary>
public record UpdateNotaFiscalCommand(
    Guid Id,
    string? NumeroNota,
    DateTime? DataEmissao,
    decimal? ValorTotal,
    string? MunicipioPrestacao,
    bool? IssRetido,
    StatusNFSEEnum? Status,
    MedicoFiscal? Medico,
    TomadorServico? Tomador,
    List<ItemServico>? Servicos,
    bool? EnviadoEmail,
    DateTime? DataEnvioEmail
) : IRequest<NotaFiscal?>;
