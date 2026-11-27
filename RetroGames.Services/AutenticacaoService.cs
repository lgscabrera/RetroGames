using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public interface IAutenticacaoService
{
    Task<Usuario?> AutenticarAsync(string email, string senha);
    Task<Usuario> CadastrarClienteAsync(string nome, string email, string senha);
    Task GarantirAdministradorInicialAsync(string email, string senha);
}

public class AutenticacaoService(IRepositorySet repositorio, IPasswordHasher<Usuario> hasher)
    : IAutenticacaoService
{
    // E-mail inexistente, senha errada e usuário inativo dão o mesmo resultado: nulo.
    public async Task<Usuario?> AutenticarAsync(string email, string senha)
    {
        var usuario = await repositorio.Usuario.BuscarPorEmailAsync(email.Trim());
        if (usuario is null || !usuario.Ativo)
            return null;

        var resultado = hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, senha);
        return resultado == PasswordVerificationResult.Failed ? null : usuario;
    }

    // Chamado na inicialização da API: cria o primeiro administrador só se ainda não houver nenhum.
    public async Task<Usuario> CadastrarClienteAsync(string nome, string email, string senha)
    {
        var usuario = new Usuario
        {
            Nome = nome.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            Ativo = true,
            IdPerfil = PerfilIds.Cliente,
        };

        usuario.SenhaHash = hasher.HashPassword(usuario, senha);

        try
        {
            usuario.Id = await repositorio.Usuario.InserirAsync(usuario);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(BusinessErrors.USUARIO_EMAIL_DUPLICADO);
        }

        return usuario;
    }

    public async Task GarantirAdministradorInicialAsync(string email, string senha)
    {
        if (await repositorio.Usuario.ExisteAdministradorAsync())
            return;

        var administrador = new Usuario
        {
            Nome = Perfis.Administrador,
            Email = email.Trim(),
            Ativo = true,
            IdPerfil = PerfilIds.Administrador,
        };
        administrador.SenhaHash = hasher.HashPassword(administrador, senha);   // só o hash é guardado
        await repositorio.Usuario.InserirAsync(administrador);
    }
}
