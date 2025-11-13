using MediatR;
using Plantonize.NotasFiscais.Domain.Entities;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.List;

/// <summary>
/// Query to list all Notas Fiscais
/// </summary>
public record ListNotasFiscaisQuery : IRequest<IEnumerable<NotaFiscal>>;
