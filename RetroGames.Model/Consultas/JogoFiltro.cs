namespace RetroGames.Model.Consultas;

public class JogoFiltro
{
    public string? Busca { get; set; }                  // parte do nome do jogo
    public int? IdPlataforma { get; set; }
    public int? IdGenero { get; set; }                  // só o Service usa; o Repository ignora
    public List<int>? IdsJogos { get; set; }            // ids dos jogos do gênero, resolvidos pelo Service
    public bool IncluirInativos { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 12;
}
