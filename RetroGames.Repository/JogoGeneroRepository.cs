using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IJogoGeneroRepository
{
    Task<List<JogoGenero>> ListarTodosAsync();
    Task<List<JogoGenero>> ListarPorJogosAsync(IReadOnlyList<int> idsJogos);
    Task InserirAsync(int idJogo, IEnumerable<int> idsGeneros);
    Task RemoverPorJogoAsync(int idJogo);
}

public class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(RepositoryConfig config) : base(config)
    {
    }

    // Todos os vínculos: o Service resolve o filtro por gênero com LINQ.
    public async Task<List<JogoGenero>> ListarTodosAsync()
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            "SELECT co_jogo, co_genero FROM tb_jogo_genero");
        return await LerTodosAsync(comando);
    }

    // Uma única consulta com IN para todos os jogos da página (evita N+1).
    public async Task<List<JogoGenero>> ListarPorJogosAsync(IReadOnlyList<int> idsJogos)
    {
        if (idsJogos.Count == 0)
            return new List<JogoGenero>();

        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao, "");
        var nomes = new List<string>();
        for (var i = 0; i < idsJogos.Count; i++)
        {
            nomes.Add("@idJogo" + i);
            AdicionarParametro(comando, "@idJogo" + i, idsJogos[i]);
        }
        comando.CommandText = "SELECT co_jogo, co_genero FROM tb_jogo_genero " +
                              "WHERE co_jogo IN (" + string.Join(", ", nomes) + ")";
        return await LerTodosAsync(comando);
    }

    public async Task InserirAsync(int idJogo, IEnumerable<int> idsGeneros)
    {
        await using var conexao = await CriarConexaoAsync();
        foreach (var idGenero in idsGeneros)
        {
            await using var comando = CriarComando(conexao,
                "INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES (@idJogo, @idGenero)");
            AdicionarParametro(comando, "@idJogo", idJogo);
            AdicionarParametro(comando, "@idGenero", idGenero);
            await ExecutarGravacaoAsync(comando);
        }
    }

    public async Task RemoverPorJogoAsync(int idJogo)
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            "DELETE FROM tb_jogo_genero WHERE co_jogo = @idJogo");
        AdicionarParametro(comando, "@idJogo", idJogo);
        await comando.ExecuteNonQueryAsync();
    }

    private static async Task<List<JogoGenero>> LerTodosAsync(MySqlCommand comando)
    {
        await using var leitor = await comando.ExecuteReaderAsync();
        var vinculos = new List<JogoGenero>();
        while (await leitor.ReadAsync())
        {
            vinculos.Add(new JogoGenero
            {
                IdJogo = leitor.GetInt32("co_jogo"),
                IdGenero = leitor.GetInt32("co_genero"),
            });
        }
        return vinculos;
    }
}
