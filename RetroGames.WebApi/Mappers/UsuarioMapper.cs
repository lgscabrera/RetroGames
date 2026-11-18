using System.Security.Claims;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Mappers;

public static class UsuarioMapper
{
    public static UsuarioLogadoResponseApiModel ParaLogadoResponse(this Usuario usuario) => new()
    {
        Id = usuario.Id,
        Nome = usuario.Nome,
        Email = usuario.Email,
        Perfil = usuario.IdPerfil == PerfilIds.Administrador ? Perfis.Administrador : Perfis.Cliente,
    };

    // Para o GET eu: lê do cookie, sem ir ao banco.
    public static UsuarioLogadoResponseApiModel ParaLogadoResponse(this ClaimsPrincipal principal) => new()
    {
        Id = principal.ObterIdUsuario() ?? 0,
        Nome = principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
        Email = principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
        Perfil = principal.FindFirstValue(ClaimTypes.Role) ?? string.Empty,
    };
}
