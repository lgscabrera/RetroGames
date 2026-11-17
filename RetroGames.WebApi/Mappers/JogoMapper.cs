using RetroGames.Model.Consultas;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class JogoMapper
{
    // O IncluirInativos fica falso: só o administrador verá jogos inativos (aula 6).
    public static JogoFiltro ParaFiltro(this ListarJogoRequestApiModel request) => new()
    {
        Busca = request.Busca,
        IdPlataforma = request.IdPlataforma,
        IdGenero = request.IdGenero,
        IncluirInativos = false,
        Pagina = Math.Max(request.Pagina, 1),
        TamanhoPagina = Math.Max(request.TamanhoPagina, 1),
    };

    public static ListarJogoResponseApiModel ParaListarResponse(this JogoDetalheServiceModel detalhe) => new()
    {
        Id = detalhe.Jogo.Id,
        Nome = detalhe.Jogo.Nome,
        AnoLancamento = detalhe.Jogo.AnoLancamento,
        Preco = detalhe.Jogo.Preco,
        Estoque = detalhe.Jogo.Estoque,
        Ativo = detalhe.Jogo.Ativo,
        NomePlataforma = detalhe.NomePlataforma,
        DescricaoCondicao = detalhe.DescricaoCondicao,
        Generos = detalhe.Generos.Select(g => g.Nome).ToList(),
    };

    public static ListarPorIdJogoResponseApiModel ParaResponse(this JogoDetalheServiceModel detalhe) => new()
    {
        Id = detalhe.Jogo.Id,
        Nome = detalhe.Jogo.Nome,
        Descricao = detalhe.Jogo.Descricao,
        AnoLancamento = detalhe.Jogo.AnoLancamento,
        Preco = detalhe.Jogo.Preco,
        Estoque = detalhe.Jogo.Estoque,
        Ativo = detalhe.Jogo.Ativo,
        IdPlataforma = detalhe.Jogo.IdPlataforma,
        NomePlataforma = detalhe.NomePlataforma,
        IdCondicaoJogo = detalhe.Jogo.IdCondicaoJogo,
        DescricaoCondicao = detalhe.DescricaoCondicao,
        Generos = detalhe.Generos.Select(g => g.ParaResponse()).ToList(),
    };

    public static PaginaResponseApiModel<ListarJogoResponseApiModel> ParaPaginaResponse(
        this ResultadoPaginado<JogoDetalheServiceModel> resultado, JogoFiltro filtro) => new()
    {
        Itens = resultado.Itens.Select(i => i.ParaListarResponse()).ToList(),
        Pagina = filtro.Pagina,
        TamanhoPagina = filtro.TamanhoPagina,
        TotalItens = resultado.Total,
        TotalPaginas = (int)Math.Ceiling(resultado.Total / (double)filtro.TamanhoPagina),
    };
}
