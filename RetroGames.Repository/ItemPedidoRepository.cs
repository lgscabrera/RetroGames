using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IItemPedidoRepository
{
    Task InserirAsync(ItemPedido item);
    Task<List<ItemPedido>> ListarPorPedidosAsync(IEnumerable<int> idsPedidos);
}

public class ItemPedidoRepository(RepositoryConfig config) : RepositoryBase(config), IItemPedidoRepository
{
    public async Task InserirAsync(ItemPedido item)
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            "INSERT INTO tb_item_pedido (qt_item, vl_unitario, co_pedido, co_jogo) " +
            "VALUES (@quantidade, @precoUnitario, @idPedido, @idJogo)");
        AdicionarParametro(comando, "@quantidade", item.Quantidade);
        AdicionarParametro(comando, "@precoUnitario", item.PrecoUnitario);
        AdicionarParametro(comando, "@idPedido", item.IdPedido);
        AdicionarParametro(comando, "@idJogo", item.IdJogo);
        await ExecutarGravacaoAsync(comando);
    }

    public async Task<List<ItemPedido>> ListarPorPedidosAsync(IEnumerable<int> idsPedidos)
    {
        var ids = idsPedidos.Distinct().ToList();
        if (ids.Count == 0)
            return [];

        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            "SELECT co_seq_item_pedido, qt_item, vl_unitario, co_pedido, co_jogo " +
            "FROM tb_item_pedido WHERE co_pedido IN (" +
            string.Join(",", ids.Select((_, indice) => "@idPedido" + indice)) + ")");
        for (var indice = 0; indice < ids.Count; indice++)
            AdicionarParametro(comando, "@idPedido" + indice, ids[indice]);

        var itens = new List<ItemPedido>();
        await using var leitor = await comando.ExecuteReaderAsync();
        while (await leitor.ReadAsync())
        {
            itens.Add(new ItemPedido
            {
                Id = leitor.GetInt32("co_seq_item_pedido"),
                Quantidade = leitor.GetInt32("qt_item"),
                PrecoUnitario = leitor.GetDecimal("vl_unitario"),
                IdPedido = leitor.GetInt32("co_pedido"),
                IdJogo = leitor.GetInt32("co_jogo"),
            });
        }
        return itens;
    }
}
