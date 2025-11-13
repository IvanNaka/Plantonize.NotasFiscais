using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Plantonize.NotasFiscais.API.Features.NotasFiscais.Create;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Enum;
using Plantonize.NotasFiscais.Domain.Interfaces;
using Xunit;

namespace Plantonize.NotasFiscais.UnitTests.Features.NotasFiscais.Create;

/// <summary>
/// Testes unitários para CreateNotaFiscalHandler (Vertical Slice)
/// </summary>
public class CreateNotaFiscalHandlerTests
{
    private readonly Mock<INotaFiscalRepository> _mockRepository;
    private readonly Mock<ILogger<CreateNotaFiscalHandler>> _mockLogger;
    private readonly CreateNotaFiscalHandler _handler;

    public CreateNotaFiscalHandlerTests()
    {
        _mockRepository = new Mock<INotaFiscalRepository>();
        _mockLogger = new Mock<ILogger<CreateNotaFiscalHandler>>();
        _handler = new CreateNotaFiscalHandler(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_NotaFiscal_Successfully()
    {
        // Arrange
        var command = new CreateNotaFiscalCommand(
            NumeroNota: "NF-2024-001",
            DataEmissao: new DateTime(2024, 1, 15),
            ValorTotal: 1500.00m,
            MunicipioPrestacao: "São Paulo",
            IssRetido: false,
            Medico: new MedicoFiscal
            {
                Nome = "Dr. João Silva",
                CpfCnpj = "123.456.789-00",
                Email = "joao@example.com",
                Municipio = "São Paulo"
            },
            Tomador: new TomadorServico
            {
                Nome = "Clínica ABC",
                CpfCnpj = "12.345.678/0001-00",
                Email = "contato@clinica.com",
                Municipio = "São Paulo"
            },
            Servicos: new List<ItemServico>
            {
                new ItemServico
                {
                    Descricao = "Consulta médica",
                    Quantidade = 1,
                    ValorUnitario = 1500.00m,
                    AliquotaIss = 5.0m
                }
            }
        );

        _mockRepository
            .Setup(x => x.CreateAsync(It.IsAny<NotaFiscal>()))
            .ReturnsAsync((NotaFiscal nf) => nf);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.NumeroNota.Should().Be("NF-2024-001");
        result.ValorTotal.Should().Be(1500.00m);
        result.MunicipioPrestacao.Should().Be("São Paulo");
        result.IssRetido.Should().BeFalse();
        result.Status.Should().Be(StatusNFSEEnum.Emitida);
        result.EnviadoEmail.Should().BeFalse();
        result.Medico.Should().NotBeNull();
        result.Medico!.Nome.Should().Be("Dr. João Silva");
        result.Tomador.Should().NotBeNull();
        result.Servicos.Should().HaveCount(1);

        _mockRepository.Verify(
            x => x.CreateAsync(It.IsAny<NotaFiscal>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Set_Status_To_Emitida()
    {
        // Arrange
        var command = new CreateNotaFiscalCommand(
            NumeroNota: "NF-2024-002",
            DataEmissao: DateTime.UtcNow,
            ValorTotal: 2000.00m,
            MunicipioPrestacao: "Rio de Janeiro",
            IssRetido: true,
            Medico: null,
            Tomador: null,
            Servicos: null
        );

        NotaFiscal? capturedNotaFiscal = null;
        _mockRepository
            .Setup(x => x.CreateAsync(It.IsAny<NotaFiscal>()))
            .ReturnsAsync((NotaFiscal nf) =>
            {
                capturedNotaFiscal = nf;
                return nf;
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedNotaFiscal.Should().NotBeNull();
        capturedNotaFiscal!.Status.Should().Be(StatusNFSEEnum.Emitida);
        result.Status.Should().Be(StatusNFSEEnum.Emitida);
    }

    [Fact]
    public async Task Handle_Should_Set_EnviadoEmail_To_False()
    {
        // Arrange
        var command = new CreateNotaFiscalCommand(
            NumeroNota: "NF-2024-003",
            DataEmissao: DateTime.UtcNow,
            ValorTotal: 500.00m,
            MunicipioPrestacao: "Brasília",
            IssRetido: false,
            Medico: null,
            Tomador: null,
            Servicos: null
        );

        _mockRepository
            .Setup(x => x.CreateAsync(It.IsAny<NotaFiscal>()))
            .ReturnsAsync((NotaFiscal nf) => nf);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.EnviadoEmail.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Should_Generate_New_Id()
    {
        // Arrange
        var command = new CreateNotaFiscalCommand(
            NumeroNota: "NF-2024-004",
            DataEmissao: DateTime.UtcNow,
            ValorTotal: 1000.00m,
            MunicipioPrestacao: "Curitiba",
            IssRetido: false,
            Medico: null,
            Tomador: null,
            Servicos: null
        );

        _mockRepository
            .Setup(x => x.CreateAsync(It.IsAny<NotaFiscal>()))
            .ReturnsAsync((NotaFiscal nf) => nf);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_Should_Initialize_Empty_Servicos_When_Null()
    {
        // Arrange
        var command = new CreateNotaFiscalCommand(
            NumeroNota: "NF-2024-005",
            DataEmissao: DateTime.UtcNow,
            ValorTotal: 750.00m,
            MunicipioPrestacao: "Belo Horizonte",
            IssRetido: false,
            Medico: null,
            Tomador: null,
            Servicos: null
        );

        _mockRepository
            .Setup(x => x.CreateAsync(It.IsAny<NotaFiscal>()))
            .ReturnsAsync((NotaFiscal nf) => nf);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Servicos.Should().NotBeNull();
        result.Servicos.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_Log_Information_On_Creation()
    {
        // Arrange
        var command = new CreateNotaFiscalCommand(
            NumeroNota: "NF-2024-006",
            DataEmissao: DateTime.UtcNow,
            ValorTotal: 1200.00m,
            MunicipioPrestacao: "Salvador",
            IssRetido: false,
            Medico: null,
            Tomador: null,
            Servicos: null
        );

        _mockRepository
            .Setup(x => x.CreateAsync(It.IsAny<NotaFiscal>()))
            .ReturnsAsync((NotaFiscal nf) => nf);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Creating Nota Fiscal")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
}
