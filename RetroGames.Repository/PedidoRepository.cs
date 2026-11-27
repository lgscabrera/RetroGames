using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IPedidoRepository
{
    Task<int> InserirAsync(Pedido pedido);
    Task<List<Pedido>> ListarPorUsuarioAsync(int idUsuario);
}

public class PedidoRepository(RepositoryConfig config) : RepositoryBase(config), IPedidoRepository
{
    private const string Colunas =
        "co_seq_pedido, dt_pedido, vl_total, co_usuario, co_situacao_pedido";

    public async Task<int> InserirAsync(Pedido pedido)
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            "INSERT INTO tb_pedido (dt_pedido, vl_total, co_usuario, co_situacao_pedido) " +
            "VALUES (@data, @total, @idUsuario, @idSituacaoPedido)");
        AdicionarParametro(comando, "@data", pedido.Data);
        AdicionarParametro(comando, "@total", pedido.Total);
        AdicionarParametro(comando, "@idUsuario", pedido.IdUsuario);
        AdicionarParametro(comando, "@idSituacaoPedido", pedido.IdSituacaoPedido);
        await ExecutarGravacaoAsync(comando);
        return (int)comando.LastInsertedId;
    }

    public async Task<List<Pedido>> ListarPorUsuarioAsync(int idUsuario)
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            $"SELECT {Colunas} FROM tb_pedido WHERE co_usuario = @idUsuario " +
            "ORDER BY dt_pedido DESC, co_seq_pedido DESC");
        AdicionarParametro(comando, "@idUsuario", idUsuario);

        var pedidos = new List<Pedido>();
        await using var leitor = await comando.ExecuteReaderAsync();
        while (await leitor.ReadAsync())
        {
            pedidos.Add(new Pedido
            {
                Id = leitor.GetInt32("co_seq_pedido"),
                Data = leitor.GetDateTime("dt_pedido"),
                Total = leitor.GetDecimal("vl_total"),
                IdUsuario = leitor.GetInt32("co_usuario"),
                IdSituacaoPedido = leitor.GetInt32("co_situacao_pedido"),
            });
        }
        return pedidos;
    }
}
