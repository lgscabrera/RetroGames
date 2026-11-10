using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IPlataformaRepository
{
    Task<List<Plataforma>> ListarAsync();
    Task<Plataforma?> BuscarPorIdAsync(int id);
    Task<bool> ExisteComNomeAsync(string nome, int? idIgnorar = null);
    Task<bool> PossuiJogosAsync(int id);
    Task<int> InserirAsync(Plataforma plataforma);
    Task AlterarAsync(Plataforma plataforma);
    Task ExcluirAsync(int id);
}

public class PlataformaRepository : RepositoryBase, IPlataformaRepository
{
    private const string Colunas = "co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento";

    public PlataformaRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<List<Plataforma>> ListarAsync()
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            $"SELECT {Colunas} FROM tb_plataforma ORDER BY no_plataforma");
        await using var leitor = await comando.ExecuteReaderAsync();

        var plataformas = new List<Plataforma>();
        while (await leitor.ReadAsync())
            plataformas.Add(Ler(leitor));
        return plataformas;
    }

    public async Task<Plataforma?> BuscarPorIdAsync(int id)
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            $"SELECT {Colunas} FROM tb_plataforma WHERE co_seq_plataforma = @id");
        AdicionarParametro(comando, "@id", id);
        await using var leitor = await comando.ExecuteReaderAsync();
        return await leitor.ReadAsync() ? Ler(leitor) : null;
    }

    // idIgnorar: usado na alteração, para o próprio registro não contar como duplicidade.
    public async Task<bool> ExisteComNomeAsync(string nome, int? idIgnorar = null)
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            "SELECT COUNT(*) FROM tb_plataforma " +
            "WHERE no_plataforma = @nome AND (@idIgnorar IS NULL OR co_seq_plataforma <> @idIgnorar)");
        AdicionarParametro(comando, "@nome", nome);
        AdicionarParametro(comando, "@idIgnorar", idIgnorar);
        return Convert.ToInt32(await comando.ExecuteScalarAsync()) > 0;
    }

    // Consulta só tb_jogo: se algum jogo usa a plataforma (a regra RN02 fica no Service).
    public async Task<bool> PossuiJogosAsync(int id)
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            "SELECT COUNT(*) FROM tb_jogo WHERE co_plataforma = @id");
        AdicionarParametro(comando, "@id", id);
        return Convert.ToInt32(await comando.ExecuteScalarAsync()) > 0;
    }

    public async Task<int> InserirAsync(Plataforma plataforma)
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            "INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento) " +
            "VALUES (@nome, @fabricante, @ano)");
        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@ano", plataforma.AnoLancamento);
        await ExecutarGravacaoAsync(comando);
        return (int)comando.LastInsertedId;
    }

    public async Task AlterarAsync(Plataforma plataforma)
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            "UPDATE tb_plataforma SET no_plataforma = @nome, no_fabricante = @fabricante, " +
            "nu_ano_lancamento = @ano WHERE co_seq_plataforma = @id");
        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@ano", plataforma.AnoLancamento);
        AdicionarParametro(comando, "@id", plataforma.Id);
        await ExecutarGravacaoAsync(comando);
    }

    public async Task ExcluirAsync(int id)
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            "DELETE FROM tb_plataforma WHERE co_seq_plataforma = @id");
        AdicionarParametro(comando, "@id", id);
        await comando.ExecuteNonQueryAsync();
    }

    private static Plataforma Ler(MySqlDataReader leitor) => new()
    {
        Id = leitor.GetInt32("co_seq_plataforma"),
        Nome = leitor.GetString("no_plataforma"),
        Fabricante = leitor.GetString("no_fabricante"),
        AnoLancamento = leitor.GetInt32("nu_ano_lancamento"),
    };
}
