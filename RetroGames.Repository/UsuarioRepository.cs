using MySqlConnector;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IUsuarioRepository
{
    Task<Usuario?> BuscarPorEmailAsync(string email);
    Task<Usuario?> BuscarPorIdAsync(int id);
    Task<bool> ExisteAdministradorAsync();
    Task<int> InserirAsync(Usuario usuario);
}

public class UsuarioRepository : RepositoryBase, IUsuarioRepository
{
    private const string Colunas =
        "co_seq_usuario, no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil";

    public UsuarioRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<Usuario?> BuscarPorEmailAsync(string email)
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            $"SELECT {Colunas} FROM tb_usuario WHERE ds_email = @email");
        AdicionarParametro(comando, "@email", email);
        await using var leitor = await comando.ExecuteReaderAsync();
        return await leitor.ReadAsync() ? Ler(leitor) : null;
    }

    public async Task<Usuario?> BuscarPorIdAsync(int id)
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            $"SELECT {Colunas} FROM tb_usuario WHERE co_seq_usuario = @id");
        AdicionarParametro(comando, "@id", id);
        await using var leitor = await comando.ExecuteReaderAsync();
        return await leitor.ReadAsync() ? Ler(leitor) : null;
    }

    public async Task<bool> ExisteAdministradorAsync()
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            "SELECT COUNT(*) FROM tb_usuario WHERE co_perfil = @idPerfil");
        AdicionarParametro(comando, "@idPerfil", PerfilIds.Administrador);
        return Convert.ToInt32(await comando.ExecuteScalarAsync()) > 0;
    }

    // dt_cadastro vem do banco, em UTC. ds_email é UNIQUE: o 1062 vira RegistroDuplicadoException.
    public async Task<int> InserirAsync(Usuario usuario)
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao,
            "INSERT INTO tb_usuario (no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil) " +
            "VALUES (@nome, @email, @senhaHash, @ativo, UTC_TIMESTAMP(), @idPerfil)");
        AdicionarParametro(comando, "@nome", usuario.Nome);
        AdicionarParametro(comando, "@email", usuario.Email);
        AdicionarParametro(comando, "@senhaHash", usuario.SenhaHash);
        AdicionarParametro(comando, "@ativo", usuario.Ativo);
        AdicionarParametro(comando, "@idPerfil", usuario.IdPerfil);
        await ExecutarGravacaoAsync(comando);
        return (int)comando.LastInsertedId;
    }

    private static Usuario Ler(MySqlDataReader leitor) => new()
    {
        Id = leitor.GetInt32("co_seq_usuario"),
        Nome = leitor.GetString("no_usuario"),
        Email = leitor.GetString("ds_email"),
        SenhaHash = leitor.GetString("ds_senha_hash"),
        Ativo = leitor.GetBoolean("st_ativo"),
        DataCadastro = leitor.GetDateTime("dt_cadastro"),
        IdPerfil = leitor.GetInt32("co_perfil"),
    };
}
