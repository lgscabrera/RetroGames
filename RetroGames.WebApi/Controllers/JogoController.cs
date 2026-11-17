using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogoController(IJogoService jogoService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>>>
        Listar([FromQuery] ListarJogoRequestApiModel request)
    {
        var filtro = request.ParaFiltro();
        var resultado = await jogoService.ListarAsync(filtro);
        return Ok(new RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>(
            resultado.ParaPaginaResponse(filtro)));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> ListarPorId(int id)
    {
        var detalhe = await jogoService.BuscarPorIdAsync(id);
        return Ok(new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>(detalhe.ParaResponse()));
    }
}
