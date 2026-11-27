using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public interface IPedidoService
{
    Task<PedidoDetalheServiceModel> FinalizarAsync(List<PedidoItemEntradaServiceModel> itens);
    Task<List<PedidoDetalheServiceModel>> ListarMeusAsync();
}

public class PedidoService(IRepositorySet repositorio, IUsuarioAtualService usuarioAtual)
    : IPedidoService
{
    private const int SituacaoPendente = 1;

    public async Task<PedidoDetalheServiceModel> FinalizarAsync(List<PedidoItemEntradaServiceModel> itens)
    {
        if (usuarioAtual.Id is null)
            throw new BusinessRuleException(BusinessErrors.USUARIO_NAO_AUTENTICADO);

        if (itens.Count == 0)
            throw new BusinessRuleException(BusinessErrors.PEDIDO_SEM_ITENS);

        var pedidosPorJogo = itens
            .GroupBy(item => item.IdJogo)
            .Select(grupo => new PedidoItemEntradaServiceModel
            {
                IdJogo = grupo.Key,
                Quantidade = grupo.Sum(item => item.Quantidade),
            })
            .ToList();

        if (pedidosPorJogo.Any(item => item.Quantidade < 1 || item.Quantidade > 10))
            throw new BusinessRuleException(BusinessErrors.PEDIDO_QUANTIDADE_INVALIDA);

        var jogos = await repositorio.Jogo.ListarPorIdsAsync(pedidosPorJogo.Select(item => item.IdJogo));
        foreach (var item in pedidosPorJogo)
        {
            var jogo = jogos.SingleOrDefault(j => j.Id == item.IdJogo);
            if (jogo is null || !jogo.Ativo)
                throw new BusinessRuleException(BusinessErrors.JOGO_INDISPONIVEL);
            if (jogo.Estoque < item.Quantidade)
                throw new BusinessRuleException(BusinessErrors.PEDIDO_ESTOQUE_INSUFICIENTE);
        }

        var pedido = new Pedido
        {
            Data = DateTime.UtcNow,
            IdUsuario = usuarioAtual.Id.Value,
            IdSituacaoPedido = SituacaoPendente,
            Total = pedidosPorJogo.Sum(item =>
                jogos.Single(jogo => jogo.Id == item.IdJogo).Preco * item.Quantidade),
        };

        var situacao = (await repositorio.SituacaoPedido.ListarTodosAsync())
            .Single(s => s.Id == SituacaoPendente);

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };
        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                   transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            pedido.Id = await repositorio.Pedido.InserirAsync(pedido);

            foreach (var item in pedidosPorJogo)
            {
                var jogo = jogos.Single(j => j.Id == item.IdJogo);
                if (!await repositorio.Jogo.BaixarEstoqueAsync(item.IdJogo, item.Quantidade))
                    throw new BusinessRuleException(BusinessErrors.PEDIDO_ESTOQUE_INSUFICIENTE);

                await repositorio.ItemPedido.InserirAsync(new ItemPedido
                {
                    Quantidade = item.Quantidade,
                    PrecoUnitario = jogo.Preco,
                    IdPedido = pedido.Id,
                    IdJogo = item.IdJogo,
                });
            }

            transactionScope.Complete();
        }

        return MontarPedido(pedido, situacao.Nome, pedidosPorJogo, jogos);
    }

    public async Task<List<PedidoDetalheServiceModel>> ListarMeusAsync()
    {
        if (usuarioAtual.Id is null)
            throw new BusinessRuleException(BusinessErrors.USUARIO_NAO_AUTENTICADO);

        var pedidos = await repositorio.Pedido.ListarPorUsuarioAsync(usuarioAtual.Id.Value);
        if (pedidos.Count == 0)
            return [];

        var idsPedidos = pedidos.Select(pedido => pedido.Id).ToList();
        var itens = await repositorio.ItemPedido.ListarPorPedidosAsync(idsPedidos);
        var situacoes = await repositorio.SituacaoPedido.ListarTodosAsync();
        var jogos = await repositorio.Jogo.ListarPorIdsAsync(itens.Select(item => item.IdJogo));

        return pedidos.Select(pedido =>
        {
            var situacao = situacoes.Single(s => s.Id == pedido.IdSituacaoPedido);
            var itensPedido = itens
                .Where(item => item.IdPedido == pedido.Id)
                .Select(item =>
                {
                    var jogo = jogos.Single(j => j.Id == item.IdJogo);
                    return new ItemPedidoServiceModel
                    {
                        IdJogo = item.IdJogo,
                        NomeJogo = jogo.Nome,
                        Quantidade = item.Quantidade,
                        PrecoUnitario = item.PrecoUnitario,
                    };
                })
                .ToList();

            return new PedidoDetalheServiceModel
            {
                Id = pedido.Id,
                Data = pedido.Data,
                Total = pedido.Total,
                Situacao = situacao.Nome,
                Itens = itensPedido,
            };
        }).ToList();
    }

    private static PedidoDetalheServiceModel MontarPedido(
        Pedido pedido, string situacao,
        List<PedidoItemEntradaServiceModel> itens,
        List<Jogo> jogos) => new()
    {
        Id = pedido.Id,
        Data = pedido.Data,
        Total = pedido.Total,
        Situacao = situacao,
        Itens = itens.Select(item =>
        {
            var jogo = jogos.Single(j => j.Id == item.IdJogo);
            return new ItemPedidoServiceModel
            {
                IdJogo = item.IdJogo,
                NomeJogo = jogo.Nome,
                Quantidade = item.Quantidade,
                PrecoUnitario = jogo.Preco,
            };
        }).ToList(),
    };
}
