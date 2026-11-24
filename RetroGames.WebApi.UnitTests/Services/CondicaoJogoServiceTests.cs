using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public class CondicaoJogoServiceTests
{
    private readonly Mock<ICondicaoJogoRepository> _condicoes = new();
    private readonly Mock<IRepositorySet> _repositorio = new();

    public CondicaoJogoServiceTests()
    {
        _repositorio.SetupGet(r => r.CondicaoJogo).Returns(_condicoes.Object);
    }

    [Fact]
    public async Task Listar_RepositorioComCondicoes_DevolveAsCondicoesEntregues()
    {
        // Arrange
        var esperadas = new List<CondicaoJogo>
        {
            new() { Id = 1, Descricao = "Novo" },
            new() { Id = 2, Descricao = "Usado" },
        };
        _condicoes.Setup(r => r.ListarAsync()).ReturnsAsync(esperadas);

        // Act
        var resultado = await new CondicaoJogoService(_repositorio.Object).ListarAsync();

        // Assert
        Assert.Equal(esperadas, resultado);
    }
}
