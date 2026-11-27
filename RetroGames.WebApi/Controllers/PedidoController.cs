using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PedidoController(IPedidoService pedidoService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<RetroGamesResponseApiModel<PedidoResponseApiModel>>> Inserir(
        [FromBody] FinalizarPedidoRequestApiModel? request)
    {
        var entradas = request!.Itens.Select(item => new PedidoItemEntradaServiceModel
        {
            IdJogo = item.IdJogo,
            Quantidade = item.Quantidade,
        }).ToList();

        var pedido = await pedidoService.FinalizarAsync(entradas);
        return StatusCode(StatusCodes.Status201Created,
            new RetroGamesResponseApiModel<PedidoResponseApiModel>(ParaResponse(pedido)));
    }

    [HttpGet("meus")]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<PedidoResponseApiModel>>>> Meus()
    {
        var pedidos = await pedidoService.ListarMeusAsync();
        return Ok(new RetroGamesResponseApiModel<List<PedidoResponseApiModel>>(
            pedidos.Select(ParaResponse).ToList()));
    }

    private static PedidoResponseApiModel ParaResponse(PedidoDetalheServiceModel pedido) => new()
    {
        Id = pedido.Id,
        Data = pedido.Data,
        Total = pedido.Total,
        Situacao = pedido.Situacao,
        Itens = pedido.Itens.Select(item => new ItemPedidoResponseApiModel
        {
            IdJogo = item.IdJogo,
            NomeJogo = item.NomeJogo,
            Quantidade = item.Quantidade,
            PrecoUnitario = item.PrecoUnitario,
        }).ToList(),
    };
}
