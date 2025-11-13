using MediatR;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.Delete;

/// <summary>
/// Command to delete a Nota Fiscal
/// </summary>
public record DeleteNotaFiscalCommand(Guid Id) : IRequest<bool>;
