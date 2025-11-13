using FluentAssertions;
using FluentValidation.TestHelper;
using Plantonize.NotasFiscais.API.Features.NotasFiscais.Create;
using Xunit;

namespace Plantonize.NotasFiscais.UnitTests.Features.NotasFiscais.Create;

/// <summary>
/// Testes unitários para CreateNotaFiscalValidator
/// </summary>
public class CreateNotaFiscalValidatorTests
{
    private readonly CreateNotaFiscalValidator _validator;

    public CreateNotaFiscalValidatorTests()
    {
        _validator = new CreateNotaFiscalValidator();
    }

    [Fact]
    public void Should_Have_Error_When_ValorTotal_Is_Zero()
    {
        // Arrange
        var command = new CreateNotaFiscalCommand(
            NumeroNota: "NF-001",
            DataEmissao: DateTime.UtcNow,
            ValorTotal: 0,
            MunicipioPrestacao: "São Paulo",
            IssRetido: false,
            Medico: null,
            Tomador: null,
            Servicos: null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ValorTotal)
            .WithErrorMessage("Valor total deve ser maior que zero");
    }

    [Fact]
    public void Should_Have_Error_When_ValorTotal_Is_Negative()
    {
        // Arrange
        var command = new CreateNotaFiscalCommand(
            NumeroNota: "NF-001",
            DataEmissao: DateTime.UtcNow,
            ValorTotal: -100,
            MunicipioPrestacao: "São Paulo",
            IssRetido: false,
            Medico: null,
            Tomador: null,
            Servicos: null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ValorTotal);
    }

    [Fact]
    public void Should_Have_Error_When_DataEmissao_Is_In_Future()
    {
        // Arrange
        var command = new CreateNotaFiscalCommand(
            NumeroNota: "NF-001",
            DataEmissao: DateTime.Now.AddDays(1),
            ValorTotal: 1000,
            MunicipioPrestacao: "São Paulo",
            IssRetido: false,
            Medico: null,
            Tomador: null,
            Servicos: null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DataEmissao)
            .WithErrorMessage("Data de emissão não pode ser futura");
    }

    [Fact]
    public void Should_Have_Error_When_NumeroNota_Exceeds_MaxLength()
    {
        // Arrange
        var command = new CreateNotaFiscalCommand(
            NumeroNota: new string('A', 51), // 51 caracteres
            DataEmissao: DateTime.UtcNow,
            ValorTotal: 1000,
            MunicipioPrestacao: "São Paulo",
            IssRetido: false,
            Medico: null,
            Tomador: null,
            Servicos: null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NumeroNota)
            .WithErrorMessage("Número da nota deve ter no máximo 50 caracteres");
    }

    [Fact]
    public void Should_Have_Error_When_MunicipioPrestacao_Exceeds_MaxLength()
    {
        // Arrange
        var command = new CreateNotaFiscalCommand(
            NumeroNota: "NF-001",
            DataEmissao: DateTime.UtcNow,
            ValorTotal: 1000,
            MunicipioPrestacao: new string('A', 101), // 101 caracteres
            IssRetido: false,
            Medico: null,
            Tomador: null,
            Servicos: null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MunicipioPrestacao)
            .WithErrorMessage("Município de prestação deve ter no máximo 100 caracteres");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var command = new CreateNotaFiscalCommand(
            NumeroNota: "NF-2024-001",
            DataEmissao: new DateTime(2024, 1, 15, 10, 0, 0), // Data fixa no passado
            ValorTotal: 1500.00m,
            MunicipioPrestacao: "São Paulo",
            IssRetido: false,
            Medico: null,
            Tomador: null,
            Servicos: null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_NumeroNota_Is_Null()
    {
        // Arrange
        var command = new CreateNotaFiscalCommand(
            NumeroNota: null,
            DataEmissao: DateTime.UtcNow,
            ValorTotal: 1000,
            MunicipioPrestacao: "São Paulo",
            IssRetido: false,
            Medico: null,
            Tomador: null,
            Servicos: null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.NumeroNota);
    }

    [Fact]
    public void Should_Not_Have_Error_When_MunicipioPrestacao_Is_Null()
    {
        // Arrange
        var command = new CreateNotaFiscalCommand(
            NumeroNota: "NF-001",
            DataEmissao: DateTime.UtcNow,
            ValorTotal: 1000,
            MunicipioPrestacao: null,
            IssRetido: false,
            Medico: null,
            Tomador: null,
            Servicos: null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.MunicipioPrestacao);
    }
}
