using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Extensions;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController(IAutenticacaoService autenticacaoService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("cadastrar")]
    [HttpPost("cadastrar-cliente")]
    public async Task<ActionResult<RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>>> CadastrarCliente(
        [FromBody] CadastrarClienteRequestApiModel? request)
    {
        var usuario = await autenticacaoService.CadastrarClienteAsync(request!.Nome, request.Email, request.Senha);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, usuario.ParaClaimsPrincipal());
        return StatusCode(StatusCodes.Status201Created,
            new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>(usuario.ParaLogadoResponse()));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>>> Login(
        [FromBody] LoginRequestApiModel? request)
    {
        var usuario = await autenticacaoService.AutenticarAsync(request!.Email, request.Senha);
        if (usuario is null)   // e-mail inexistente, senha errada e usuário inativo: a mesma resposta
        {
            return Unauthorized(new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.CREDENCIAIS_INVALIDAS.Code,
                ErrorMessage = ResponseCodes.CREDENCIAIS_INVALIDAS.DefaultMessage,
            });
        }

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, usuario.ParaClaimsPrincipal());
        return Ok(new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>(usuario.ParaLogadoResponse()));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new RetroGamesResponseApiModel());
    }

    [Authorize]
    [HttpGet("eu")]
    public ActionResult<RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>> Eu()
        => Ok(new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>(User.ParaLogadoResponse()));
}
