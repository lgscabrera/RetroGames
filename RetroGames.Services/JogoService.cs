using System.Transactions;
using RetroGames.Model.Consultas;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public interface IJogoService
{
    Task<ResultadoPaginado<JogoDetalheServiceModel>> ListarAsync(JogoFiltro filtro);
    Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id);
    Task<JogoDetalheServiceModel> InserirAsync(Jogo jogo, IReadOnlyList<int> idsGeneros);
    Task<JogoDetalheServiceModel> AlterarAsync(Jogo jogo, IReadOnlyList<int> idsGeneros);
    Task DesativarAsync(int id);
}

public class JogoService(IRepositorySet repositorio) : IJogoService
{
    public async Task<ResultadoPaginado<JogoDetalheServiceModel>> ListarAsync(JogoFiltro filtro)
    {
        // Filtro por gênero: resolvido aqui com LINQ; o Repository recebe só a lista de ids.
        if (filtro.IdGenero.HasValue)
        {
            var vinculos = await repositorio.JogoGenero.ListarTodosAsync();
            filtro.IdsJogos = vinculos
                .Where(v => v.IdGenero == filtro.IdGenero.Value)
                .Select(v => v.IdJogo)
                .Distinct()
                .ToList();
        }

        var pagina = await repositorio.Jogo.ListarAsync(filtro);   // filtro, paginação e total no SQL
        return new ResultadoPaginado<JogoDetalheServiceModel>
        {
            Itens = await MontarDetalhesAsync(pagina.Itens),
            Total = pagina.Total,
        };
    }

    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id)
    {
        var jogo = await repositorio.Jogo.BuscarPorIdAsync(id)
                   ?? throw new BusinessRuleException(BusinessErrors.JOGO_NAO_ENCONTRADO);
        return (await MontarDetalhesAsync(new List<Jogo> { jogo })).Single();
    }

    public async Task<JogoDetalheServiceModel> InserirAsync(Jogo jogo, IReadOnlyList<int> idsGeneros)
    {
        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = true;
        var ids = idsGeneros.Distinct().ToList();
        await ValidarReferenciasAsync(jogo, ids);                                   // RN05

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };
        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                   transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            jogo.Id = await repositorio.Jogo.InserirAsync(jogo);
            await repositorio.JogoGenero.InserirAsync(jogo.Id, ids);
            transactionScope.Complete();
        }

        return await BuscarPorIdAsync(jogo.Id);
    }

    public async Task<JogoDetalheServiceModel> AlterarAsync(Jogo jogo, IReadOnlyList<int> idsGeneros)
    {
        if (await repositorio.Jogo.BuscarPorIdAsync(jogo.Id) is null)
            throw new BusinessRuleException(BusinessErrors.JOGO_NAO_ENCONTRADO);

        jogo.Nome = jogo.Nome.Trim();
        var ids = idsGeneros.Distinct().ToList();
        await ValidarReferenciasAsync(jogo, ids);                                   // RN05

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };
        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                   transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            await repositorio.Jogo.AlterarAsync(jogo);
            await repositorio.JogoGenero.RemoverPorJogoAsync(jogo.Id);
            await repositorio.JogoGenero.InserirAsync(jogo.Id, ids);
            transactionScope.Complete();
        }

        return await BuscarPorIdAsync(jogo.Id);
    }

    // RN03: o jogo nunca é apagado, só desativado.
    public async Task DesativarAsync(int id)
    {
        if (await repositorio.Jogo.BuscarPorIdAsync(id) is null)
            throw new BusinessRuleException(BusinessErrors.JOGO_NAO_ENCONTRADO);
        await repositorio.Jogo.DesativarAsync(id);
    }

    private async Task ValidarReferenciasAsync(Jogo jogo, IReadOnlyList<int> idsGeneros)
    {
        if (await repositorio.Plataforma.BuscarPorIdAsync(jogo.IdPlataforma) is null)
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);

        var condicoes = await repositorio.CondicaoJogo.ListarAsync();
        if (condicoes.All(c => c.Id != jogo.IdCondicaoJogo))
            throw new BusinessRuleException(BusinessErrors.CONDICAO_NAO_ENCONTRADA);

        var generos = await repositorio.Genero.ListarAsync();
        if (idsGeneros.Any(id => generos.All(g => g.Id != id)))
            throw new BusinessRuleException(BusinessErrors.GENERO_NAO_ENCONTRADO);
    }

    // Número fixo de consultas (plataformas, condições, gêneros e vínculos), qualquer que seja a página.
    private async Task<List<JogoDetalheServiceModel>> MontarDetalhesAsync(List<Jogo> jogos)
    {
        if (jogos.Count == 0)
            return new List<JogoDetalheServiceModel>();

        var plataformas = await repositorio.Plataforma.ListarAsync();
        var condicoes = await repositorio.CondicaoJogo.ListarAsync();
        var generos = await repositorio.Genero.ListarAsync();
        var vinculos = await repositorio.JogoGenero.ListarPorJogosAsync(jogos.Select(j => j.Id).ToList());

        return (from jogo in jogos
                join plataforma in plataformas on jogo.IdPlataforma equals plataforma.Id
                join condicao in condicoes on jogo.IdCondicaoJogo equals condicao.Id
                select new JogoDetalheServiceModel
                {
                    Jogo = jogo,
                    NomePlataforma = plataforma.Nome,
                    DescricaoCondicao = condicao.Descricao,
                    Generos = vinculos
                        .Where(v => v.IdJogo == jogo.Id)
                        .Join(generos, v => v.IdGenero, g => g.Id, (v, g) => g)
                        .OrderBy(g => g.Nome)
                        .ToList(),
                }).ToList();
    }
}
