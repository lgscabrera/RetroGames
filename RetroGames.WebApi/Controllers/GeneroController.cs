using Microsoft.AspNetCore.Authorization;
using RetroGames.WebApi.Configuration;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneroController(IGeneroService generoService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarGeneroResponseApiModel>>>> Listar()
    {
        var generos = await generoService.ListarAsync();
        return Ok(new RetroGamesResponseApiModel<List<ListarGeneroResponseApiModel>>(
            generos.Select(g => g.ParaResponse()).ToList()));
    }
}
