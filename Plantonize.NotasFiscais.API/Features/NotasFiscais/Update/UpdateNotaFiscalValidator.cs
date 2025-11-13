using FluentValidation;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.Update;

/// <summary>
/// Validator for UpdateNotaFiscalCommand
/// </summary>
public class UpdateNotaFiscalValidator : AbstractValidator<UpdateNotaFiscalCommand>
{
    public UpdateNotaFiscalValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID é obrigatório");

        When(x => !string.IsNullOrEmpty(x.NumeroNota), () =>
        {
            RuleFor(x => x.NumeroNota)
                .MaximumLength(50).WithMessage("Número da nota deve ter no máximo 50 caracteres");
        });

        When(x => x.ValorTotal.HasValue, () =>
        {
            RuleFor(x => x.ValorTotal)
                .GreaterThan(0).WithMessage("Valor total deve ser maior que zero");
        });

        When(x => !string.IsNullOrEmpty(x.MunicipioPrestacao), () =>
        {
            RuleFor(x => x.MunicipioPrestacao)
                .MaximumLength(100).WithMessage("Município de prestação deve ter no máximo 100 caracteres");
        });
    }
}
