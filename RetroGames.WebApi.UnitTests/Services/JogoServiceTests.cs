using Moq;
using RetroGames.Model.Consultas;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public class JogoServiceTests
{
    private readonly Mock<IJogoRepository> _jogos = new();
    private readonly Mock<IPlataformaRepository> _plataformas = new();
    private readonly Mock<ICondicaoJogoRepository> _condicoes = new();
    private readonly Mock<IGeneroRepository> _generos = new();
    private readonly Mock<IJogoGeneroRepository> _jogoGeneros = new();
    private readonly Mock<IRepositorySet> _repositorio = new();
    private readonly Mock<IUsuarioAtualService> _usuarioAtual = new();

    public JogoServiceTests()
    {
        _repositorio.SetupGet(r => r.Jogo).Returns(_jogos.Object);
        _repositorio.SetupGet(r => r.Plataforma).Returns(_plataformas.Object);
        _repositorio.SetupGet(r => r.CondicaoJogo).Returns(_condicoes.Object);
        _repositorio.SetupGet(r => r.Genero).Returns(_generos.Object);
        _repositorio.SetupGet(r => r.JogoGenero).Returns(_jogoGeneros.Object);
    }

    private JogoService CriarService() => new(_repositorio.Object, _usuarioAtual.Object);

    [Fact]
    public async Task Inserir_PlataformaInexistente_LancaPlataformaNaoEncontradaENaoGrava()
    {
        // Arrange
        _plataformas.Setup(r => r.BuscarPorIdAsync(99)).ReturnsAsync((Plataforma?)null);
        var jogo = new Jogo { Nome = "Teste", IdPlataforma = 99, IdCondicaoJogo = 1 };

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => CriarService().InserirAsync(jogo, new List<int> { 1 }));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA, erro.Erro);
        _jogos.Verify(r => r.InserirAsync(It.IsAny<Jogo>()), Times.Never);
    }

    [Fact]
    public async Task Inserir_CondicaoInexistente_LancaCondicaoNaoEncontradaENaoGrava()
    {
        // Arrange: a plataforma existe, mas só a condição 1 está cadastrada
        _plataformas.Setup(r => r.BuscarPorIdAsync(2)).ReturnsAsync(new Plataforma { Id = 2, Nome = "SNES" });
        _condicoes.Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<CondicaoJogo> { new() { Id = 1, Descricao = "Novo" } });
        var jogo = new Jogo { Nome = "Teste", IdPlataforma = 2, IdCondicaoJogo = 9 };

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => CriarService().InserirAsync(jogo, new List<int> { 1 }));

        // Assert
        Assert.Equal(BusinessErrors.CONDICAO_NAO_ENCONTRADA, erro.Erro);
        _jogos.Verify(r => r.InserirAsync(It.IsAny<Jogo>()), Times.Never);
    }

    [Fact]
    public async Task Inserir_GeneroInexistente_LancaGeneroNaoEncontradoENaoGrava()
    {
        // Arrange: plataforma e condição existem, mas o gênero 5 não
        _plataformas.Setup(r => r.BuscarPorIdAsync(2)).ReturnsAsync(new Plataforma { Id = 2, Nome = "SNES" });
        _condicoes.Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<CondicaoJogo> { new() { Id = 1, Descricao = "Novo" } });
        _generos.Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<Genero> { new() { Id = 1, Nome = "RPG" } });
        var jogo = new Jogo { Nome = "Teste", IdPlataforma = 2, IdCondicaoJogo = 1 };

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => CriarService().InserirAsync(jogo, new List<int> { 5 }));

        // Assert
        Assert.Equal(BusinessErrors.GENERO_NAO_ENCONTRADO, erro.Erro);
        _jogos.Verify(r => r.InserirAsync(It.IsAny<Jogo>()), Times.Never);
    }

    [Fact]
    public async Task Listar_FiltrandoPorGenero_EnviaAoRepositorioSoOsIdsDoGenero()
    {
        // Arrange: os jogos 10 e 30 são do gênero 2; o jogo 20 é de outro
        _jogoGeneros.Setup(r => r.ListarTodosAsync()).ReturnsAsync(new List<JogoGenero>
        {
            new() { IdJogo = 10, IdGenero = 2 },
            new() { IdJogo = 20, IdGenero = 3 },
            new() { IdJogo = 30, IdGenero = 2 },
        });
        _jogos.Setup(r => r.ListarAsync(It.IsAny<JogoFiltro>()))
            .ReturnsAsync(new ResultadoPaginado<Jogo>());

        // Act
        await CriarService().ListarAsync(new JogoFiltro { IdGenero = 2 });

        // Assert
        _jogos.Verify(r => r.ListarAsync(It.Is<JogoFiltro>(f =>
            f.IdsJogos != null && f.IdsJogos.SequenceEqual(new[] { 10, 30 }))), Times.Once);
    }

    [Fact]
    public async Task Listar_SemGenero_NaoConsultaVinculosENaoRestringeIds()
    {
        // Arrange
        _jogos.Setup(r => r.ListarAsync(It.IsAny<JogoFiltro>()))
            .ReturnsAsync(new ResultadoPaginado<Jogo>());

        // Act
        await CriarService().ListarAsync(new JogoFiltro());

        // Assert
        _jogoGeneros.Verify(r => r.ListarTodosAsync(), Times.Never);
        _jogos.Verify(r => r.ListarAsync(It.Is<JogoFiltro>(f => f.IdsJogos == null)), Times.Once);
    }

    [Fact]
    public async Task Listar_PaginaComJogos_MontaNomeDaPlataformaEDescricaoDaCondicao()
    {
        // Arrange
        var jogo = new Jogo { Id = 1, Nome = "Chrono Trigger", IdPlataforma = 2, IdCondicaoJogo = 3, Ativo = true };
        _jogos.Setup(r => r.ListarAsync(It.IsAny<JogoFiltro>()))
            .ReturnsAsync(new ResultadoPaginado<Jogo> { Itens = new List<Jogo> { jogo }, Total = 1 });
        _plataformas.Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<Plataforma> { new() { Id = 2, Nome = "Super Nintendo" } });
        _condicoes.Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<CondicaoJogo> { new() { Id = 3, Descricao = "Recondicionado" } });
        _generos.Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<Genero> { new() { Id = 4, Nome = "RPG" } });
        _jogoGeneros.Setup(r => r.ListarPorJogosAsync(It.IsAny<IReadOnlyList<int>>()))
            .ReturnsAsync(new List<JogoGenero> { new() { IdJogo = 1, IdGenero = 4 } });

        // Act
        var resultado = await CriarService().ListarAsync(new JogoFiltro());

        // Assert
        var item = Assert.Single(resultado.Itens);
        Assert.Equal("Super Nintendo", item.NomePlataforma);
        Assert.Equal("Recondicionado", item.DescricaoCondicao);
        Assert.Equal("RPG", Assert.Single(item.Generos).Nome);
        Assert.Equal(1, resultado.Total);
    }

    [Fact]
    public async Task BuscarPorId_IdInexistente_LancaJogoNaoEncontrado()
    {
        // Arrange
        _jogos.Setup(r => r.BuscarPorIdAsync(99)).ReturnsAsync((Jogo?)null);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => CriarService().BuscarPorIdAsync(99));

        // Assert
        Assert.Equal(BusinessErrors.JOGO_NAO_ENCONTRADO, erro.Erro);
    }

    [Fact]
    public async Task Desativar_IdInexistente_LancaJogoNaoEncontradoENaoDesativa()
    {
        // Arrange
        _jogos.Setup(r => r.BuscarPorIdAsync(99)).ReturnsAsync((Jogo?)null);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => CriarService().DesativarAsync(99));

        // Assert
        Assert.Equal(BusinessErrors.JOGO_NAO_ENCONTRADO, erro.Erro);
        _jogos.Verify(r => r.DesativarAsync(It.IsAny<int>()), Times.Never);
    }
}
