using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Plantonize.NotasFiscais.API.Features.NotasFiscais.Delete;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Interfaces;
using Xunit;

namespace Plantonize.NotasFiscais.UnitTests.Features.NotasFiscais.Delete;

/// <summary>
/// Testes unitários para DeleteNotaFiscalHandler (Vertical Slice)
/// </summary>
public class DeleteNotaFiscalHandlerTests
{
    private readonly Mock<INotaFiscalRepository> _mockRepository;
    private readonly Mock<ILogger<DeleteNotaFiscalHandler>> _mockLogger;
    private readonly DeleteNotaFiscalHandler _handler;

    public DeleteNotaFiscalHandlerTests()
    {
        _mockRepository = new Mock<INotaFiscalRepository>();
        _mockLogger = new Mock<ILogger<DeleteNotaFiscalHandler>>();
        _handler = new DeleteNotaFiscalHandler(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Delete_NotaFiscal_Successfully()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        var existing = new NotaFiscal
        {
            Id = existingId,
            NumeroNota = "NF-2024-001",
            DataEmissao = DateTime.UtcNow,
            ValorTotal = 1000.00m
        };

        var command = new DeleteNotaFiscalCommand(existingId);

        _mockRepository
            .Setup(x => x.GetByIdAsync(existingId))
            .ReturnsAsync(existing);

        _mockRepository
            .Setup(x => x.DeleteAsync(existingId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(x => x.GetByIdAsync(existingId), Times.Once);
        _mockRepository.Verify(x => x.DeleteAsync(existingId), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_False_When_NotaFiscal_NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new DeleteNotaFiscalCommand(nonExistentId);

        _mockRepository
            .Setup(x => x.GetByIdAsync(nonExistentId))
            .ReturnsAsync((NotaFiscal?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(x => x.GetByIdAsync(nonExistentId), Times.Once);
        _mockRepository.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Log_Information_On_Successful_Delete()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        var existing = new NotaFiscal
        {
            Id = existingId,
            NumeroNota = "NF-2024-001",
            DataEmissao = DateTime.UtcNow,
            ValorTotal = 1000.00m
        };

        var command = new DeleteNotaFiscalCommand(existingId);

        _mockRepository
            .Setup(x => x.GetByIdAsync(existingId))
            .ReturnsAsync(existing);

        _mockRepository
            .Setup(x => x.DeleteAsync(existingId))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("deleted successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Log_Warning_When_NotaFiscal_NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new DeleteNotaFiscalCommand(nonExistentId);

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
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("not found for deletion")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
