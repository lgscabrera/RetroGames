namespace RetroGames.Services;

// O Service não conhece HTTP: ele só recebe esta interface. A WebApi a implementa lendo o cookie.
public interface IUsuarioAtualService
{
    bool EstaLogado { get; }
    int? Id { get; }
    bool EhAdministrador { get; }
}
