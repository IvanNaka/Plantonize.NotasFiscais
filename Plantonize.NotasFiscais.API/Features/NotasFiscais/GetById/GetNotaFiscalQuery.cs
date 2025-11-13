using MediatR;
using Plantonize.NotasFiscais.Domain.Entities;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.GetById;

/// <summary>
/// Query to get a Nota Fiscal by ID
/// </summary>
public record GetNotaFiscalQuery(Guid Id) : IRequest<NotaFiscal?>;
