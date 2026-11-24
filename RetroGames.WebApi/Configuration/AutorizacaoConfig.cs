using Microsoft.AspNetCore.Authorization;
using RetroGames.Model.Constantes;

namespace RetroGames.WebApi.Configuration;

public static class Politicas
{
    public const string Administrador = "SomenteAdministrador";
}

public static class AutorizacaoConfig
{
    // Seguro por padrão: toda rota exige login, exceto as marcadas com [AllowAnonymous] (RS03).
    public static IServiceCollection AddPoliticasDeAutorizacao(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
            .AddPolicy(Politicas.Administrador, politica => politica.RequireRole(Perfis.Administrador));
        return services;
    }
}
