using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface ISituacaoPedidoRepository
{
    Task<List<SituacaoPedido>> ListarTodosAsync();
}

public class SituacaoPedidoRepository(RepositoryConfig config) : RepositoryBase(config), ISituacaoPedidoRepository
{
    public async Task<List<SituacaoPedido>> ListarTodosAsync()
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            "SELECT co_situacao_pedido, ds_situacao_pedido FROM tb_situacao_pedido");

        var situacoes = new List<SituacaoPedido>();
        await using var leitor = await comando.ExecuteReaderAsync();
        while (await leitor.ReadAsync())
        {
            situacoes.Add(new SituacaoPedido
            {
                Id = leitor.GetInt32("co_situacao_pedido"),
                Nome = leitor.GetString("ds_situacao_pedido"),
            });
        }
        return situacoes;
    }
}
