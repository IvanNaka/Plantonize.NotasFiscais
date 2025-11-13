using FluentAssertions;
using Moq;
using Plantonize.NotasFiscais.Application.Services;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Enum;
using Plantonize.NotasFiscais.Domain.Interfaces;
using Xunit;

namespace Plantonize.NotasFiscais.UnitTests.Application.Services;

/// <summary>
/// Testes unitários para NotaFiscalService (Clean Architecture - Application Layer)
/// </summary>
public class NotaFiscalServiceTests
{
    private readonly Mock<INotaFiscalRepository> _mockRepository;
    private readonly Mock<IServiceBusService> _mockServiceBusService;
    private readonly NotaFiscalService _service;

    public NotaFiscalServiceTests()
    {
        _mockRepository = new Mock<INotaFiscalRepository>();
        _mockServiceBusService = new Mock<IServiceBusService>();
        _service = new NotaFiscalService(_mockRepository.Object, _mockServiceBusService.Object);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_NotasFiscais()
    {
        // Arrange
        var notasFiscais = new List<NotaFiscal>
        {
            new NotaFiscal { Id = Guid.NewGuid(), NumeroNota = "NF-001" },
            new NotaFiscal { Id = Guid.NewGuid(), NumeroNota = "NF-002" }
        };

        _mockRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(notasFiscais);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_NotaFiscal_When_Found()
    {
        // Arrange
        var id = Guid.NewGuid();
        var notaFiscal = new NotaFiscal
        {
            Id = id,
            NumeroNota = "NF-001",
            ValorTotal = 1000.00m
        };

        _mockRepository
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(notaFiscal);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.NumeroNota.Should().Be("NF-001");
        _mockRepository.Verify(x => x.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_NotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockRepository
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync((NotaFiscal?)null);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(x => x.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Create_NotaFiscal()
    {
        // Arrange
        var notaFiscal = new NotaFiscal
        {
            Id = Guid.NewGuid(),
            NumeroNota = "NF-001",
            DataEmissao = DateTime.UtcNow,
            ValorTotal = 1500.00m,
            Status = StatusNFSEEnum.Emitida,
            Medico = new MedicoFiscal { Nome = "Dr. João" },
            Tomador = new TomadorServico { Nome = "Clínica ABC" },
            Servicos = new List<ItemServico>
            {
                new ItemServico { Descricao = "Consulta" }
            }
        };

        _mockRepository
            .Setup(x => x.CreateAsync(It.IsAny<NotaFiscal>()))
            .ReturnsAsync((NotaFiscal nf) => nf);

        _mockServiceBusService
            .Setup(x => x.SendMessageToQueueAsync(It.IsAny<object>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(notaFiscal);

        // Assert
        result.Should().NotBeNull();
        result.NumeroNota.Should().Be("NF-001");
        _mockRepository.Verify(x => x.CreateAsync(It.IsAny<NotaFiscal>()), Times.Once);
        _mockServiceBusService.Verify(x => x.SendMessageToQueueAsync(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_NotaFiscal()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingNotaFiscal = new NotaFiscal
        {
            Id = id,
            NumeroNota = "NF-001",
            ValorTotal = 1000.00m,
            Medico = new MedicoFiscal { Nome = "Dr. João" },
            Tomador = new TomadorServico { Nome = "Clínica ABC" },
            Servicos = new List<ItemServico> { new ItemServico { Descricao = "Consulta" } }
        };

        var updatedNotaFiscal = new NotaFiscal
        {
            Id = id,
            NumeroNota = "NF-001-UPDATED",
            ValorTotal = 2000.00m,
            Medico = new MedicoFiscal { Nome = "Dr. João" },
            Tomador = new TomadorServico { Nome = "Clínica ABC" },
            Servicos = new List<ItemServico> { new ItemServico { Descricao = "Consulta" } }
        };

        _mockRepository
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(existingNotaFiscal);

        _mockRepository
            .Setup(x => x.UpdateAsync(It.IsAny<NotaFiscal>()))
            .ReturnsAsync((NotaFiscal nf) => nf);

        // Act
        var result = await _service.UpdateAsync(updatedNotaFiscal);

        // Assert
        result.Should().NotBeNull();
        result.NumeroNota.Should().Be("NF-001-UPDATED");
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<NotaFiscal>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Call_Repository_DeleteAsync()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingNotaFiscal = new NotaFiscal
        {
            Id = id,
            NumeroNota = "NF-001"
        };

        _mockRepository
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(existingNotaFiscal);

        _mockRepository
            .Setup(x => x.DeleteAsync(id))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(id);

        // Assert
        _mockRepository.Verify(x => x.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetByMedicoIdAsync_Should_Return_NotasFiscais_For_Medico()
    {
        // Arrange
        var medicoId = Guid.NewGuid();
        var notasFiscais = new List<NotaFiscal>
        {
            new NotaFiscal
            {
                Id = Guid.NewGuid(),
                NumeroNota = "NF-001",
                Medico = new MedicoFiscal { Nome = "Dr. João" }
            },
            new NotaFiscal
            {
                Id = Guid.NewGuid(),
                NumeroNota = "NF-002",
                Medico = new MedicoFiscal { Nome = "Dr. João" }
            }
        };

        _mockRepository
            .Setup(x => x.GetByMedicoIdAsync(medicoId))
            .ReturnsAsync(notasFiscais);

        // Act
        var result = await _service.GetByMedicoIdAsync(medicoId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        _mockRepository.Verify(x => x.GetByMedicoIdAsync(medicoId), Times.Once);
    }
}
