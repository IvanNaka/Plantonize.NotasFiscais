using FluentAssertions;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Enum;
using Xunit;

namespace Plantonize.NotasFiscais.UnitTests.Domain.Entities;

/// <summary>
/// Testes unitários para a entidade NotaFiscal (Domain Layer)
/// </summary>
public class NotaFiscalTests
{
    [Fact]
    public void NotaFiscal_Should_Be_Created_With_Default_Values()
    {
        // Act
        var notaFiscal = new NotaFiscal();

        // Assert
        notaFiscal.Id.Should().BeEmpty();
        notaFiscal.Status.Should().Be(StatusNFSEEnum.Autorizado);
        notaFiscal.EnviadoEmail.Should().BeFalse();
        notaFiscal.IssRetido.Should().BeFalse();
    }

    [Fact]
    public void NotaFiscal_Should_Allow_Setting_Properties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dataEmissao = new DateTime(2024, 1, 15);

        // Act
        var notaFiscal = new NotaFiscal
        {
            Id = id,
            NumeroNota = "NF-2024-001",
            DataEmissao = dataEmissao,
            ValorTotal = 1500.00m,
            Status = StatusNFSEEnum.Emitida,
            MunicipioPrestacao = "São Paulo",
            IssRetido = false
        };

        // Assert
        notaFiscal.Id.Should().Be(id);
        notaFiscal.NumeroNota.Should().Be("NF-2024-001");
        notaFiscal.DataEmissao.Should().Be(dataEmissao);
        notaFiscal.ValorTotal.Should().Be(1500.00m);
        notaFiscal.Status.Should().Be(StatusNFSEEnum.Emitida);
        notaFiscal.MunicipioPrestacao.Should().Be("São Paulo");
        notaFiscal.IssRetido.Should().BeFalse();
    }

    [Fact]
    public void NotaFiscal_Should_Allow_Setting_Medico()
    {
        // Arrange
        var medico = new MedicoFiscal
        {
            Nome = "Dr. João Silva",
            CpfCnpj = "123.456.789-00",
            Email = "joao@example.com",
            Municipio = "São Paulo"
        };

        // Act
        var notaFiscal = new NotaFiscal
        {
            Medico = medico
        };

        // Assert
        notaFiscal.Medico.Should().NotBeNull();
        notaFiscal.Medico!.Nome.Should().Be("Dr. João Silva");
        notaFiscal.Medico.CpfCnpj.Should().Be("123.456.789-00");
    }

    [Fact]
    public void NotaFiscal_Should_Allow_Setting_Tomador()
    {
        // Arrange
        var tomador = new TomadorServico
        {
            Nome = "Clínica ABC",
            CpfCnpj = "12.345.678/0001-00",
            Email = "contato@clinica.com",
            Municipio = "Rio de Janeiro"
        };

        // Act
        var notaFiscal = new NotaFiscal
        {
            Tomador = tomador
        };

        // Assert
        notaFiscal.Tomador.Should().NotBeNull();
        notaFiscal.Tomador!.Nome.Should().Be("Clínica ABC");
        notaFiscal.Tomador.CpfCnpj.Should().Be("12.345.678/0001-00");
    }

    [Fact]
    public void NotaFiscal_Should_Allow_Setting_Multiple_Servicos()
    {
        // Arrange
        var servicos = new List<ItemServico>
        {
            new ItemServico
            {
                Descricao = "Consulta médica",
                Quantidade = 1,
                ValorUnitario = 500.00m,
                AliquotaIss = 5.0m
            },
            new ItemServico
            {
                Descricao = "Exame",
                Quantidade = 2,
                ValorUnitario = 250.00m,
                AliquotaIss = 5.0m
            }
        };

        // Act
        var notaFiscal = new NotaFiscal
        {
            Servicos = servicos
        };

        // Assert
        notaFiscal.Servicos.Should().NotBeNull();
        notaFiscal.Servicos.Should().HaveCount(2);
        notaFiscal.Servicos![0].Descricao.Should().Be("Consulta médica");
        notaFiscal.Servicos[1].Descricao.Should().Be("Exame");
    }

    [Fact]
    public void NotaFiscal_Should_Allow_Setting_EnviadoEmail()
    {
        // Act
        var notaFiscal = new NotaFiscal
        {
            EnviadoEmail = true,
            DataEnvioEmail = DateTime.UtcNow
        };

        // Assert
        notaFiscal.EnviadoEmail.Should().BeTrue();
        notaFiscal.DataEnvioEmail.Should().NotBeNull();
    }

    [Theory]
    [InlineData(StatusNFSEEnum.Autorizado)]
    [InlineData(StatusNFSEEnum.Emitida)]
    [InlineData(StatusNFSEEnum.Cancelado)]
    [InlineData(StatusNFSEEnum.Paga)]
    [InlineData(StatusNFSEEnum.Rejeitado)]
    [InlineData(StatusNFSEEnum.Enviada)]
    public void NotaFiscal_Should_Accept_Valid_Status_Values(StatusNFSEEnum status)
    {
        // Act
        var notaFiscal = new NotaFiscal { Status = status };

        // Assert
        notaFiscal.Status.Should().Be(status);
    }
}
