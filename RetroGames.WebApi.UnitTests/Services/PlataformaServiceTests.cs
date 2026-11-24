using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public class PlataformaServiceTests
{
    private readonly Mock<IPlataformaRepository> _plataformas = new();
    private readonly Mock<IRepositorySet> _repositorio = new();

    public PlataformaServiceTests()
    {
        _repositorio.SetupGet(r => r.Plataforma).Returns(_plataformas.Object);
    }

    private PlataformaService CriarService() => new(_repositorio.Object);

    [Fact]
    public async Task BuscarPorId_IdInexistente_LancaNaoEncontrada()
    {
        // Arrange
        _plataformas.Setup(r => r.BuscarPorIdAsync(99)).ReturnsAsync((Plataforma?)null);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => CriarService().BuscarPorIdAsync(99));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA, erro.Erro);
    }

    [Fact]
    public async Task Inserir_NomeJaCadastrado_LancaNomeDuplicadoENaoGrava()
    {
        // Arrange: a conferência encontra o nome
        _plataformas.Setup(r => r.ExisteComNomeAsync("SNES", null)).ReturnsAsync(true);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => CriarService().InserirAsync(new Plataforma { Nome = "SNES" }));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NOME_DUPLICADO, erro.Erro);
        _plataformas.Verify(r => r.InserirAsync(It.IsAny<Plataforma>()), Times.Never);
    }

    [Fact]
    public async Task Inserir_BancoRecusaPorDuplicidade_LancaNomeDuplicado()
    {
        // Arrange: a conferência passa, mas o banco recusa o INSERT (erro 1062)
        _plataformas.Setup(r => r.ExisteComNomeAsync("SNES", null)).ReturnsAsync(false);
        _plataformas.Setup(r => r.InserirAsync(It.IsAny<Plataforma>()))
            .ThrowsAsync(new RegistroDuplicadoException("Duplicate entry", new Exception()));

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => CriarService().InserirAsync(new Plataforma { Nome = "SNES" }));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NOME_DUPLICADO, erro.Erro);
    }

    [Fact]
    public async Task Inserir_NomeLivre_DevolvePlataformaComIdGerado()
    {
        // Arrange
        _plataformas.Setup(r => r.ExisteComNomeAsync("SNES", null)).ReturnsAsync(false);
        _plataformas.Setup(r => r.InserirAsync(It.IsAny<Plataforma>())).ReturnsAsync(7);

        // Act
        var criada = await CriarService().InserirAsync(new Plataforma { Nome = " SNES " });

        // Assert
        Assert.Equal(7, criada.Id);
        Assert.Equal("SNES", criada.Nome);
        _plataformas.Verify(r => r.InserirAsync(It.IsAny<Plataforma>()), Times.Once);
    }

    [Fact]
    public async Task Alterar_MantendoProprioNome_GravaAlteracao()
    {
        // Arrange: a plataforma 5 existe e nenhuma outra usa o nome "SNES"
        _plataformas.Setup(r => r.BuscarPorIdAsync(5))
            .ReturnsAsync(new Plataforma { Id = 5, Nome = "SNES" });
        _plataformas.Setup(r => r.ExisteComNomeAsync("SNES", 5)).ReturnsAsync(false);
        _plataformas.Setup(r => r.AlterarAsync(It.IsAny<Plataforma>())).Returns(Task.CompletedTask);

        // Act
        var alterada = await CriarService().AlterarAsync(new Plataforma { Id = 5, Nome = "SNES" });

        // Assert
        Assert.Equal(5, alterada.Id);
        _plataformas.Verify(r => r.AlterarAsync(It.IsAny<Plataforma>()), Times.Once);
    }

    [Fact]
    public async Task Excluir_PlataformaComJogos_LancaPossuiJogosENaoExclui()
    {
        // Arrange: a plataforma 1 existe, mas tem jogos
        _plataformas.Setup(r => r.BuscarPorIdAsync(1))
            .ReturnsAsync(new Plataforma { Id = 1, Nome = "SNES" });
        _plataformas.Setup(r => r.PossuiJogosAsync(1)).ReturnsAsync(true);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => CriarService().ExcluirAsync(1));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_POSSUI_JOGOS, erro.Erro);
        _plataformas.Verify(r => r.ExcluirAsync(It.IsAny<int>()), Times.Never);
    }
}
