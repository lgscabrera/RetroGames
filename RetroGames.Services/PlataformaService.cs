using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public interface IPlataformaService
{
    Task<List<Plataforma>> ListarAsync();
    Task<Plataforma> BuscarPorIdAsync(int id);
    Task<Plataforma> InserirAsync(Plataforma plataforma);
    Task<Plataforma> AlterarAsync(Plataforma plataforma);
    Task ExcluirAsync(int id);
}

public class PlataformaService(IRepositorySet repositorio) : IPlataformaService
{
    public Task<List<Plataforma>> ListarAsync()
        => repositorio.Plataforma.ListarAsync();

    public async Task<Plataforma> BuscarPorIdAsync(int id)
    {
        var plataforma = await repositorio.Plataforma.BuscarPorIdAsync(id);
        return plataforma ?? throw new BusinessRuleException(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);
    }

    public async Task<Plataforma> InserirAsync(Plataforma plataforma)
    {
        plataforma.Nome = plataforma.Nome.Trim();
        if (await repositorio.Plataforma.ExisteComNomeAsync(plataforma.Nome))      // RN01
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
        try
        {
            plataforma.Id = await repositorio.Plataforma.InserirAsync(plataforma);
        }
        catch (RegistroDuplicadoException)   // outra gravação chegou antes
        {
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
        }
        return plataforma;
    }

    public async Task<Plataforma> AlterarAsync(Plataforma plataforma)
    {
        await BuscarPorIdAsync(plataforma.Id);                                      // existe?
        plataforma.Nome = plataforma.Nome.Trim();
        if (await repositorio.Plataforma.ExisteComNomeAsync(plataforma.Nome, plataforma.Id))   // ignora a própria
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
        try
        {
            await repositorio.Plataforma.AlterarAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
        }
        return plataforma;
    }

    public async Task ExcluirAsync(int id)
    {
        await BuscarPorIdAsync(id);                                                 // existe?
        if (await repositorio.Plataforma.PossuiJogosAsync(id))                      // RN02
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_POSSUI_JOGOS);
        await repositorio.Plataforma.ExcluirAsync(id);
    }
}
