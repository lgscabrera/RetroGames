namespace RetroGames.WebApi.ApiModels;

public class FinalizarPedidoRequestApiModel
{
    public List<ItemPedidoRequestApiModel> Itens { get; set; } = [];
}

public class ItemPedidoRequestApiModel
{
    public int IdJogo { get; set; }
    public int Quantidade { get; set; }
}
