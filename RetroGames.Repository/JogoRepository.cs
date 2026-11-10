using MySqlConnector;
using RetroGames.Model.Consultas;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IJogoRepository
{
    Task<ResultadoPaginado<Jogo>> ListarAsync(JogoFiltro filtro);
    Task<Jogo?> BuscarPorIdAsync(int id);
    Task<int> InserirAsync(Jogo jogo);
    Task AlterarAsync(Jogo jogo);
    Task DesativarAsync(int id);
}

public class JogoRepository : RepositoryBase, IJogoRepository
{
    private const string Colunas =
        "co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, " +
        "st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo";

    public JogoRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<ResultadoPaginado<Jogo>> ListarAsync(JogoFiltro filtro)
    {
        var resultado = new ResultadoPaginado<Jogo>();
        if (filtro.IdsJogos is { Count: 0 })
            return resultado; // gênero sem jogos: nem precisa ir ao banco

        var pagina = Math.Max(filtro.Pagina, 1);
        var tamanho = Math.Max(filtro.TamanhoPagina, 1);

        await using var conexao = await CriarConexaoAsync();

        await using (var comandoTotal = CriarComando(conexao, "SELECT COUNT(*) FROM tb_jogo"))
        {
            comandoTotal.CommandText += MontarFiltro(filtro, comandoTotal);
            resultado.Total = Convert.ToInt32(await comandoTotal.ExecuteScalarAsync());
        }

        await using (var comandoPagina = CriarComando(conexao, $"SELECT {Colunas} FROM tb_jogo"))
        {
            comandoPagina.CommandText += MontarFiltro(filtro, comandoPagina) +
                " ORDER BY no_jogo, co_seq_jogo LIMIT @tamanho OFFSET @pular";
            AdicionarParametro(comandoPagina, "@tamanho", tamanho);
            AdicionarParametro(comandoPagina, "@pular", (pagina - 1) * tamanho);

            await using var leitor = await comandoPagina.ExecuteReaderAsync();
            while (await leitor.ReadAsync())
                resultado.Itens.Add(Ler(leitor));
        }

        return resultado;
    }

    public async Task<Jogo?> BuscarPorIdAsync(int id)
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            $"SELECT {Colunas} FROM tb_jogo WHERE co_seq_jogo = @id");
        AdicionarParametro(comando, "@id", id);
        await using var leitor = await comando.ExecuteReaderAsync();
        return await leitor.ReadAsync() ? Ler(leitor) : null;
    }

    // dt_cadastro vem do banco, em UTC (Espec. 4.1).
    public async Task<int> InserirAsync(Jogo jogo)
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            "INSERT INTO tb_jogo (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, " +
            "st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo) " +
            "VALUES (@nome, @descricao, @ano, @preco, @estoque, @ativo, UTC_TIMESTAMP(), " +
            "@idPlataforma, @idCondicao)");
        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@ano", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicao", jogo.IdCondicaoJogo);
        await ExecutarGravacaoAsync(comando);
        return (int)comando.LastInsertedId;
    }

    // Não altera st_ativo (só DesativarAsync) nem dt_cadastro.
    public async Task AlterarAsync(Jogo jogo)
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            "UPDATE tb_jogo SET no_jogo = @nome, ds_jogo = @descricao, nu_ano_lancamento = @ano, " +
            "vl_preco = @preco, qt_estoque = @estoque, co_plataforma = @idPlataforma, " +
            "co_condicao_jogo = @idCondicao WHERE co_seq_jogo = @id");
        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@ano", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicao", jogo.IdCondicaoJogo);
        AdicionarParametro(comando, "@id", jogo.Id);
        await ExecutarGravacaoAsync(comando);
    }

    // Exclusão lógica (RN03): o registro e os vínculos continuam no banco.
    public async Task DesativarAsync(int id)
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            "UPDATE tb_jogo SET st_ativo = FALSE WHERE co_seq_jogo = @id");
        AdicionarParametro(comando, "@id", id);
        await ExecutarGravacaoAsync(comando);
    }

    // Só os NOMES dos parâmetros entram no texto do SQL; os valores vão por AdicionarParametro.
    private string MontarFiltro(JogoFiltro filtro, MySqlCommand comando)
    {
        var sql = " WHERE (@busca IS NULL OR no_jogo LIKE CONCAT('%', @busca, '%'))" +
                  " AND (@idPlataforma IS NULL OR co_plataforma = @idPlataforma)" +
                  " AND (@incluirInativos = 1 OR st_ativo = 1)";
        AdicionarParametro(comando, "@busca",
            string.IsNullOrWhiteSpace(filtro.Busca) ? null : filtro.Busca.Trim());
        AdicionarParametro(comando, "@idPlataforma", filtro.IdPlataforma);
        AdicionarParametro(comando, "@incluirInativos", filtro.IncluirInativos);

        if (filtro.IdsJogos is { Count: > 0 })
        {
            var nomes = new List<string>();
            for (var i = 0; i < filtro.IdsJogos.Count; i++)
            {
                nomes.Add("@idJogo" + i);
                AdicionarParametro(comando, "@idJogo" + i, filtro.IdsJogos[i]);
            }
            sql += " AND co_seq_jogo IN (" + string.Join(", ", nomes) + ")";
        }
        return sql;
    }

    private static Jogo Ler(MySqlDataReader leitor) => new()
    {
        Id = leitor.GetInt32("co_seq_jogo"),
        Nome = leitor.GetString("no_jogo"),
        Descricao = leitor.IsDBNull(leitor.GetOrdinal("ds_jogo")) ? null : leitor.GetString("ds_jogo"),
        AnoLancamento = leitor.GetInt32("nu_ano_lancamento"),
        Preco = leitor.GetDecimal("vl_preco"),
        Estoque = leitor.GetInt32("qt_estoque"),
        Ativo = leitor.GetBoolean("st_ativo"),
        DataCadastro = leitor.GetDateTime("dt_cadastro"),
        IdPlataforma = leitor.GetInt32("co_plataforma"),
        IdCondicaoJogo = leitor.GetInt32("co_condicao_jogo"),
    };
}
