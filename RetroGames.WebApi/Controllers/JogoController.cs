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

    [HttpPost]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> Inserir(
        [FromBody] InserirJogoRequestApiModel? request)
    {
        var detalhe = await jogoService.InserirAsync(request!.ParaModel(), request.IdsGeneros);
        return CreatedAtAction(nameof(ListarPorId), new { id = detalhe.Jogo.Id },
            new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>(detalhe.ParaResponse()));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> Alterar(
        int id, [FromBody] AlterarJogoRequestApiModel? request)
    {
        var detalhe = await jogoService.AlterarAsync(request!.ParaModel(id), request.IdsGeneros);
        return Ok(new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>(detalhe.ParaResponse()));
    }

    // RN03: o jogo nunca é apagado, só desativado.
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Desativar(int id)
    {
        await jogoService.DesativarAsync(id);
        return Ok(new RetroGamesResponseApiModel());
    }
}
