using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public class GeneroServiceTests
{
    private readonly Mock<IGeneroRepository> _generos = new();
    private readonly Mock<IRepositorySet> _repositorio = new();

    public GeneroServiceTests()
    {
        _repositorio.SetupGet(r => r.Genero).Returns(_generos.Object);
    }

    [Fact]
    public async Task Listar_RepositorioComGeneros_DevolveOsGenerosEntregues()
    {
        // Arrange
        var esperados = new List<Genero>
        {
            new() { Id = 1, Nome = "Ação" },
            new() { Id = 2, Nome = "RPG" },
        };
        _generos.Setup(r => r.ListarAsync()).ReturnsAsync(esperados);

        // Act
        var resultado = await new GeneroService(_repositorio.Object).ListarAsync();

        // Assert
        Assert.Equal(esperados, resultado);
    }
}
