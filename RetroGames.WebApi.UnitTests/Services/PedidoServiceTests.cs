using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public class PedidoServiceTests
{
    private readonly Mock<IRepositorySet> _repositorio = new();
    private readonly Mock<IJogoRepository> _jogos = new();
    private readonly Mock<IPedidoRepository> _pedidos = new();
    private readonly Mock<IItemPedidoRepository> _itens = new();
    private readonly Mock<ISituacaoPedidoRepository> _situacoes = new();
    private readonly Mock<IUsuarioAtualService> _usuarioAtual = new();

    public PedidoServiceTests()
    {
        _repositorio.SetupGet(r => r.Jogo).Returns(_jogos.Object);
        _repositorio.SetupGet(r => r.Pedido).Returns(_pedidos.Object);
        _repositorio.SetupGet(r => r.ItemPedido).Returns(_itens.Object);
        _repositorio.SetupGet(r => r.SituacaoPedido).Returns(_situacoes.Object);
        _usuarioAtual.SetupGet(u => u.Id).Returns(20);
    }

    private PedidoService CriarService() => new(_repositorio.Object, _usuarioAtual.Object);

    [Fact]
    public async Task Finalizar_JogoAtivo_ComEstoque_CalculaPrecoDoBanco()
    {
        _jogos.Setup(r => r.ListarPorIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync([new Jogo { Id = 5, Nome = "Chrono Trigger", Preco = 149.90m, Estoque = 3, Ativo = true }]);
        _situacoes.Setup(r => r.ListarTodosAsync()).ReturnsAsync([
            new SituacaoPedido { Id = 1, Nome = "Pendente" }
        ]);
        _pedidos.Setup(r => r.InserirAsync(It.IsAny<Pedido>())).ReturnsAsync(7);
        _jogos.Setup(r => r.BaixarEstoqueAsync(5, 2)).ReturnsAsync(true);

        var pedido = await CriarService().FinalizarAsync([
            new PedidoItemEntradaServiceModel { IdJogo = 5, Quantidade = 2 }
        ]);

        Assert.Equal(299.80m, pedido.Total);
        Assert.Equal("Pendente", pedido.Situacao);
        Assert.Equal("Chrono Trigger", Assert.Single(pedido.Itens).NomeJogo);
        _itens.Verify(r => r.InserirAsync(It.Is<ItemPedido>(item =>
            item.PrecoUnitario == 149.90m && item.Quantidade == 2)), Times.Once);
    }

    [Fact]
    public async Task Finalizar_JogoInativo_LancaJogoIndisponivel()
    {
        _jogos.Setup(r => r.ListarPorIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync([new Jogo { Id = 5, Nome = "Jogo", Preco = 10m, Estoque = 3, Ativo = false }]);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => CriarService().FinalizarAsync([
                new PedidoItemEntradaServiceModel { IdJogo = 5, Quantidade = 1 }
            ]));

        Assert.Equal(BusinessErrors.JOGO_INDISPONIVEL, erro.Erro);
    }

    [Fact]
    public async Task Finalizar_EstoqueInsuficiente_LancaEstoqueInsuficiente()
    {
        _jogos.Setup(r => r.ListarPorIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync([new Jogo { Id = 5, Nome = "Jogo", Preco = 10m, Estoque = 1, Ativo = true }]);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => CriarService().FinalizarAsync([
                new PedidoItemEntradaServiceModel { IdJogo = 5, Quantidade = 2 }
            ]));

        Assert.Equal(BusinessErrors.PEDIDO_ESTOQUE_INSUFICIENTE, erro.Erro);
    }
}
