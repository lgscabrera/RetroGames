namespace RetroGames.Model.Model;

public class Pedido
{
    public int Id { get; set; }                              // co_seq_pedido
    public DateTime Data { get; set; }                       // dt_pedido
    public decimal Total { get; set; }                       // vl_total
    public int IdUsuario { get; set; }                       // co_usuario
    public int IdSituacaoPedido { get; set; }                // co_situacao_pedido
}
