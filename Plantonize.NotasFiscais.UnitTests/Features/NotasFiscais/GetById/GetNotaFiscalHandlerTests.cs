using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Plantonize.NotasFiscais.API.Features.NotasFiscais.GetById;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Enum;
using Plantonize.NotasFiscais.Domain.Interfaces;
using Xunit;

namespace Plantonize.NotasFiscais.UnitTests.Features.NotasFiscais.GetById;

/// <summary>
/// Testes unitários para GetNotaFiscalHandler (Vertical Slice)
/// </summary>
public class GetNotaFiscalHandlerTests
{
    private readonly Mock<INotaFiscalRepository> _mockRepository;
    private readonly Mock<ILogger<GetNotaFiscalHandler>> _mockLogger;
    private readonly GetNotaFiscalHandler _handler;

    public GetNotaFiscalHandlerTests()
    {
        _mockRepository = new Mock<INotaFiscalRepository>();
        _mockLogger = new Mock<ILogger<GetNotaFiscalHandler>>();
        _handler = new GetNotaFiscalHandler(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_NotaFiscal_When_Found()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        var notaFiscal = new NotaFiscal
        {
            Id = existingId,
            NumeroNota = "NF-2024-001",
            DataEmissao = new DateTime(2024, 1, 15),
            ValorTotal = 1500.00m,
            MunicipioPrestacao = "São Paulo",
            IssRetido = false,
            Status = StatusNFSEEnum.Emitida
        };

        var query = new GetNotaFiscalQuery(existingId);

        _mockRepository
            .Setup(x => x.GetByIdAsync(existingId))
            .ReturnsAsync(notaFiscal);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(existingId);
        result.NumeroNota.Should().Be("NF-2024-001");
        result.ValorTotal.Should().Be(1500.00m);

        _mockRepository.Verify(x => x.GetByIdAsync(existingId), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Null_When_NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var query = new GetNotaFiscalQuery(nonExistentId);

        _mockRepository
            .Setup(x => x.GetByIdAsync(nonExistentId))
            .ReturnsAsync((NotaFiscal?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(x => x.GetByIdAsync(nonExistentId), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Information_When_Getting()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        var notaFiscal = new NotaFiscal { Id = existingId };
        var query = new GetNotaFiscalQuery(existingId);

        _mockRepository
            .Setup(x => x.GetByIdAsync(existingId))
            .ReturnsAsync(notaFiscal);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Getting Nota Fiscal")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Warning_When_NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var query = new GetNotaFiscalQuery(nonExistentId);

        _mockRepository
            .Setup(x => x.GetByIdAsync(nonExistentId))
            .ReturnsAsync((NotaFiscal?)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

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
