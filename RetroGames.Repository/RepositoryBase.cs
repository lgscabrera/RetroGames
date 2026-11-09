using MySqlConnector;

namespace RetroGames.Repository;

public abstract class RepositoryBase
{
    private readonly RepositoryConfig _config;

    protected RepositoryBase(RepositoryConfig config)
    {
        _config = config;
    }

    // Abre uma conexão. Quem chama usa "await using" para fechá-la no fim.
    protected async Task<MySqlConnection> CriarConexaoAsync()
    {
        var conexao = new MySqlConnection(_config.ConnectionString);
        try
        {
            await conexao.OpenAsync();
        }
        catch
        {
            await conexao.DisposeAsync();
            throw;
        }
        return conexao;
    }

    // Cria um comando com o texto SQL, ligado à conexão.
    protected MySqlCommand CriarComando(MySqlConnection conexao, string sql)
    {
        return new MySqlCommand(sql, conexao);
    }

    // Adiciona um parâmetro (@nome e valor). O valor viaja separado do SQL.
    protected void AdicionarParametro(MySqlCommand comando, string nome, object? valor)
    {
        comando.Parameters.AddWithValue(nome, valor ?? DBNull.Value);
    }

    // Executa INSERT ou UPDATE. O erro 1062 (valor repetido em coluna UNIQUE)
    // vira RegistroDuplicadoException; é o único lugar que faz essa conversão.
    protected async Task<int> ExecutarGravacaoAsync(MySqlCommand comando)
    {
        try
        {
            return await comando.ExecuteNonQueryAsync();
        }
        catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
        {
            throw new RegistroDuplicadoException(ex.Message, ex);
        }
    }
}
