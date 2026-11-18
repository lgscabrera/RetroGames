using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.Configuration;

public static class InjecaoDependenciaConfig
{
    // O RepositoryConfig é único (singleton); repositórios e RepositorySet, um por requisição (scoped).
    public static IServiceCollection AddRepositorios(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton(new RepositoryConfig { ConnectionString = connectionString });
        services.AddScoped<IPlataformaRepository, PlataformaRepository>();
        services.AddScoped<IJogoRepository, JogoRepository>();
        services.AddScoped<IJogoGeneroRepository, JogoGeneroRepository>();
        services.AddScoped<IGeneroRepository, GeneroRepository>();
        services.AddScoped<ICondicaoJogoRepository, CondicaoJogoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IRepositorySet, RepositorySet>();
        return services;
    }

    public static IServiceCollection AddServicosDeNegocio(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
        services.AddScoped<IPlataformaService, PlataformaService>();
        services.AddScoped<IJogoService, JogoService>();
        services.AddScoped<IGeneroService, GeneroService>();
        services.AddScoped<ICondicaoJogoService, CondicaoJogoService>();
        services.AddScoped<IAutenticacaoService, AutenticacaoService>();
        return services;
    }
}
