using System.Security.Claims;
using RetroGames.Model.Constantes;
using RetroGames.Services;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Services;

public class UsuarioAtualService(IHttpContextAccessor acessor) : IUsuarioAtualService
{
    private ClaimsPrincipal? Principal => acessor.HttpContext?.User;

    public bool EstaLogado => Principal?.Identity?.IsAuthenticated == true;

    public int? Id => Principal?.ObterIdUsuario();

    public bool EhAdministrador => EstaLogado && Principal!.IsInRole(Perfis.Administrador);
}
