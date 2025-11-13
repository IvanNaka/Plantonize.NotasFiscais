using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Plantonize.NotasFiscais.API.Features.NotasFiscais.List;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Enum;
using Plantonize.NotasFiscais.Domain.Interfaces;
using Xunit;

namespace Plantonize.NotasFiscais.UnitTests.Features.NotasFiscais.List;

/// <summary>
/// Testes unitários para ListNotasFiscaisHandler (Vertical Slice)
/// </summary>
public class ListNotasFiscaisHandlerTests
{
    private readonly Mock<INotaFiscalRepository> _mockRepository;
    private readonly Mock<ILogger<ListNotasFiscaisHandler>> _mockLogger;
    private readonly ListNotasFiscaisHandler _handler;

    public ListNotasFiscaisHandlerTests()
    {
        _mockRepository = new Mock<INotaFiscalRepository>();
        _mockLogger = new Mock<ILogger<ListNotasFiscaisHandler>>();
        _handler = new ListNotasFiscaisHandler(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_All_NotasFiscais()
    {
        // Arrange
        var notasFiscais = new List<NotaFiscal>
        {
            new NotaFiscal
            {
                Id = Guid.NewGuid(),
                NumeroNota = "NF-2024-001",
                DataEmissao = new DateTime(2024, 1, 15),
                ValorTotal = 1000.00m,
                Status = StatusNFSEEnum.Emitida
            },
            new NotaFiscal
            {
                Id = Guid.NewGuid(),
                NumeroNota = "NF-2024-002",
                DataEmissao = new DateTime(2024, 1, 16),
                ValorTotal = 1500.00m,
                Status = StatusNFSEEnum.Paga
            },
            new NotaFiscal
            {
                Id = Guid.NewGuid(),
                NumeroNota = "NF-2024-003",
                DataEmissao = new DateTime(2024, 1, 17),
                ValorTotal = 2000.00m,
                Status = StatusNFSEEnum.Cancelado
            }
        };

        var query = new ListNotasFiscaisQuery();

        _mockRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(notasFiscais);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(nf => nf.NumeroNota == "NF-2024-001");
        result.Should().Contain(nf => nf.NumeroNota == "NF-2024-002");
        result.Should().Contain(nf => nf.NumeroNota == "NF-2024-003");

        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_No_NotasFiscais()
    {
        // Arrange
        var emptyList = new List<NotaFiscal>();
        var query = new ListNotasFiscaisQuery();

        _mockRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(emptyList);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Information_With_Count()
    {
        // Arrange
        var notasFiscais = new List<NotaFiscal>
        {
            new NotaFiscal { Id = Guid.NewGuid(), NumeroNota = "NF-001" },
            new NotaFiscal { Id = Guid.NewGuid(), NumeroNota = "NF-002" }
        };

        var query = new ListNotasFiscaisQuery();

        _mockRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(notasFiscais);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Found 2 Notas Fiscais")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Listing_Information()
    {
        // Arrange
        var notasFiscais = new List<NotaFiscal>();
        var query = new ListNotasFiscaisQuery();

        _mockRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(notasFiscais);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Listing all Notas Fiscais")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
