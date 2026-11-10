namespace RetroGames.Model.Consultas;

public class ResultadoPaginado<T>
{
    public List<T> Itens { get; set; } = new();
    public int Total { get; set; }
}
