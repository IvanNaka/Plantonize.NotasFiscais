using FluentValidation;

namespace Plantonize.NotasFiscais.API.Features.NotasFiscais.Create;

/// <summary>
/// Validator for CreateNotaFiscalCommand
/// </summary>
public class CreateNotaFiscalValidator : AbstractValidator<CreateNotaFiscalCommand>
{
    public CreateNotaFiscalValidator()
    {
        RuleFor(x => x.NumeroNota)
            .MaximumLength(50).WithMessage("Número da nota deve ter no máximo 50 caracteres")
            .When(x => !string.IsNullOrEmpty(x.NumeroNota));

        RuleFor(x => x.ValorTotal)
            .GreaterThan(0).WithMessage("Valor total deve ser maior que zero");

        RuleFor(x => x.DataEmissao)
            .NotEmpty().WithMessage("Data de emissão é obrigatória");

        RuleFor(x => x.MunicipioPrestacao)
            .MaximumLength(100).WithMessage("Município de prestação deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.MunicipioPrestacao));
    }
}
