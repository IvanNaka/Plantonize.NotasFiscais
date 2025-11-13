using MediatR;
using Plantonize.NotasFiscais.Domain.Entities;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.Create;

/// <summary>
/// Command to create a new Nota Fiscal
/// </summary>
public record CreateNotaFiscalCommand(
    string? NumeroNota,
    DateTime DataEmissao,
    decimal ValorTotal,
    string? MunicipioPrestacao,
    bool IssRetido,
    MedicoFiscal? Medico,
    TomadorServico? Tomador,
    List<ItemServico>? Servicos
) : IRequest<NotaFiscal>;
