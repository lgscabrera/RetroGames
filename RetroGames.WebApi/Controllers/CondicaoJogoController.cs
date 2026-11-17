using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CondicaoJogoController(ICondicaoJogoService condicaoJogoService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarCondicaoJogoResponseApiModel>>>> Listar()
    {
        var condicoes = await condicaoJogoService.ListarAsync();
        return Ok(new RetroGamesResponseApiModel<List<ListarCondicaoJogoResponseApiModel>>(
            condicoes.Select(c => c.ParaResponse()).ToList()));
    }
}
