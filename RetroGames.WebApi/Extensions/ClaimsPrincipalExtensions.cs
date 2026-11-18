using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;

namespace RetroGames.WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    // As claims vão criptografadas no cookie: id, nome, e-mail e perfil. Nunca a senha nem o hash.
    public static ClaimsPrincipal ParaClaimsPrincipal(this Usuario usuario)
    {
        var perfil = usuario.IdPerfil == PerfilIds.Administrador ? Perfis.Administrador : Perfis.Cliente;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, perfil),
        };
        return new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
    }

    public static int? ObterIdUsuario(this ClaimsPrincipal principal)
        => int.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
}
