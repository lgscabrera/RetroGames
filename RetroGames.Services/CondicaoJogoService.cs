using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public interface ICondicaoJogoService
{
    Task<List<CondicaoJogo>> ListarAsync();
}

public class CondicaoJogoService(IRepositorySet repositorio) : ICondicaoJogoService
{
    public Task<List<CondicaoJogo>> ListarAsync()
        => repositorio.CondicaoJogo.ListarAsync();
}
