namespace RetroGames.Model.Consultas;

public class JogoFiltro
{
    public string? Busca { get; set; }                  // parte do nome do jogo
    public int? IdPlataforma { get; set; }
    public List<int>? IdsJogos { get; set; }            // ids já resolvidos pelo Service (filtro por gênero)
    public bool IncluirInativos { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 12;
}
