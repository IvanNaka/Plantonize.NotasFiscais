using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Plantonize.NotasFiscais.API.Features.NotasFiscais.Update;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Enum;
using Plantonize.NotasFiscais.Domain.Interfaces;
using Xunit;

namespace Plantonize.NotasFiscais.UnitTests.Features.NotasFiscais.Update;

/// <summary>
/// Testes unitários para UpdateNotaFiscalHandler (Vertical Slice)
/// </summary>
public class UpdateNotaFiscalHandlerTests
{
    private readonly Mock<INotaFiscalRepository> _mockRepository;
    private readonly Mock<ILogger<UpdateNotaFiscalHandler>> _mockLogger;
    private readonly UpdateNotaFiscalHandler _handler;

    public UpdateNotaFiscalHandlerTests()
    {
        _mockRepository = new Mock<INotaFiscalRepository>();
        _mockLogger = new Mock<ILogger<UpdateNotaFiscalHandler>>();
        _handler = new UpdateNotaFiscalHandler(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Update_NotaFiscal_Successfully()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        var existing = new NotaFiscal
        {
            Id = existingId,
            NumeroNota = "NF-2024-001",
            DataEmissao = new DateTime(2024, 1, 15),
            ValorTotal = 1000.00m,
            MunicipioPrestacao = "São Paulo",
            IssRetido = false,
            Status = StatusNFSEEnum.Emitida
        };

        var command = new UpdateNotaFiscalCommand(
            Id: existingId,
            NumeroNota: "NF-2024-001-UPDATED",
            DataEmissao: new DateTime(2024, 1, 16),
            ValorTotal: 1500.00m,
            MunicipioPrestacao: "Rio de Janeiro",
            IssRetido: true,
            Status: StatusNFSEEnum.Paga,
            Medico: null,
            Tomador: null,
            Servicos: null,
            EnviadoEmail: true,
            DataEnvioEmail: DateTime.UtcNow
        );

        _mockRepository
            .Setup(x => x.GetByIdAsync(existingId))
            .ReturnsAsync(existing);

        _mockRepository
            .Setup(x => x.UpdateAsync(It.IsAny<NotaFiscal>()))
            .ReturnsAsync((NotaFiscal nf) => nf);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.NumeroNota.Should().Be("NF-2024-001-UPDATED");
        result.ValorTotal.Should().Be(1500.00m);
        result.MunicipioPrestacao.Should().Be("Rio de Janeiro");
        result.IssRetido.Should().BeTrue();
        result.Status.Should().Be(StatusNFSEEnum.Paga);
        result.EnviadoEmail.Should().BeTrue();

        _mockRepository.Verify(x => x.GetByIdAsync(existingId), Times.Once);
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<NotaFiscal>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Null_When_NotaFiscal_NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateNotaFiscalCommand(
            Id: nonExistentId,
            NumeroNota: "NF-2024-999",
            DataEmissao: null,
            ValorTotal: null,
            MunicipioPrestacao: null,
            IssRetido: null,
            Status: null,
            Medico: null,
            Tomador: null,
            Servicos: null,
            EnviadoEmail: null,
            DataEnvioEmail: null
        );

        _mockRepository
            .Setup(x => x.GetByIdAsync(nonExistentId))
            .ReturnsAsync((NotaFiscal?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(x => x.GetByIdAsync(nonExistentId), Times.Once);
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<NotaFiscal>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Only_Update_Provided_Fields()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        var existing = new NotaFiscal
        {
            Id = existingId,
            NumeroNota = "NF-2024-001",
            DataEmissao = new DateTime(2024, 1, 15),
            ValorTotal = 1000.00m,
            MunicipioPrestacao = "São Paulo",
            IssRetido = false,
            Status = StatusNFSEEnum.Emitida
        };

        var command = new UpdateNotaFiscalCommand(
            Id: existingId,
            NumeroNota: null, // Not updating
            DataEmissao: null, // Not updating
            ValorTotal: 1500.00m, // Updating only this
            MunicipioPrestacao: null, // Not updating
            IssRetido: null, // Not updating
            Status: null, // Not updating
            Medico: null,
            Tomador: null,
            Servicos: null,
            EnviadoEmail: null,
            DataEnvioEmail: null
        );

        _mockRepository
            .Setup(x => x.GetByIdAsync(existingId))
            .ReturnsAsync(existing);

        _mockRepository
            .Setup(x => x.UpdateAsync(It.IsAny<NotaFiscal>()))
            .ReturnsAsync((NotaFiscal nf) => nf);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.NumeroNota.Should().Be("NF-2024-001"); // Not changed
        result.DataEmissao.Should().Be(new DateTime(2024, 1, 15)); // Not changed
        result.ValorTotal.Should().Be(1500.00m); // Changed
        result.MunicipioPrestacao.Should().Be("São Paulo"); // Not changed
        result.IssRetido.Should().BeFalse(); // Not changed
        result.Status.Should().Be(StatusNFSEEnum.Emitida); // Not changed
    }

    [Fact]
    public async Task Handle_Should_Update_Medico_When_Provided()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        var existing = new NotaFiscal
        {
            Id = existingId,
            NumeroNota = "NF-2024-001",
            DataEmissao = DateTime.UtcNow,
            ValorTotal = 1000.00m,
            Medico = null
        };

        var newMedico = new MedicoFiscal
        {
            Nome = "Dr. Carlos Santos",
            CpfCnpj = "987.654.321-00",
            Email = "carlos@example.com"
        };

        var command = new UpdateNotaFiscalCommand(
            Id: existingId,
            NumeroNota: null,
            DataEmissao: null,
            ValorTotal: null,
            MunicipioPrestacao: null,
            IssRetido: null,
            Status: null,
            Medico: newMedico,
            Tomador: null,
            Servicos: null,
            EnviadoEmail: null,
            DataEnvioEmail: null
        );

        _mockRepository
            .Setup(x => x.GetByIdAsync(existingId))
            .ReturnsAsync(existing);

        _mockRepository
            .Setup(x => x.UpdateAsync(It.IsAny<NotaFiscal>()))
            .ReturnsAsync((NotaFiscal nf) => nf);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Medico.Should().NotBeNull();
        result.Medico!.Nome.Should().Be("Dr. Carlos Santos");
    }

    [Fact]
    public async Task Handle_Should_Log_Warning_When_NotaFiscal_NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateNotaFiscalCommand(
            Id: nonExistentId,
            NumeroNota: null,
            DataEmissao: null,
            ValorTotal: null,
            MunicipioPrestacao: null,
            IssRetido: null,
            Status: null,
            Medico: null,
            Tomador: null,
            Servicos: null,
            EnviadoEmail: null,
            DataEnvioEmail: null
        );

        _mockRepository
            .Setup(x => x.GetByIdAsync(nonExistentId))
            .ReturnsAsync((NotaFiscal?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("not found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
