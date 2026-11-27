namespace RetroGames.WebApi.ApiModels;

public class PedidoResponseApiModel
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public decimal Total { get; set; }
    public string Situacao { get; set; } = string.Empty;
    public List<ItemPedidoResponseApiModel> Itens { get; set; } = [];
}

public class ItemPedidoResponseApiModel
{
    public int IdJogo { get; set; }
    public string NomeJogo { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}
