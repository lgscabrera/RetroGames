namespace RetroGames.Services;

public class PedidoDetalheServiceModel
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public decimal Total { get; set; }
    public string Situacao { get; set; } = string.Empty;
    public List<ItemPedidoServiceModel> Itens { get; set; } = [];
}

public class ItemPedidoServiceModel
{
    public int IdJogo { get; set; }
    public string NomeJogo { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}

public class PedidoItemEntradaServiceModel
{
    public int IdJogo { get; set; }
    public int Quantidade { get; set; }
}
