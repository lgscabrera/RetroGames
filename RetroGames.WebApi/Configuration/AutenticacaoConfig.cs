using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Configuration;

public static class AutenticacaoConfig
{
    // Cookie de sessão (RS01): HttpOnly, SameSite=Strict, 2 horas com renovação enquanto há uso.
    public static IServiceCollection AddAutenticacaoPorCookie(this IServiceCollection services, IHostEnvironment ambiente)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opcoes =>
            {
                opcoes.Cookie.Name = "retrogames.sessao";
                opcoes.Cookie.HttpOnly = true;
                opcoes.Cookie.SameSite = SameSiteMode.Strict;
                opcoes.Cookie.SecurePolicy = ambiente.IsDevelopment()
                    ? CookieSecurePolicy.SameAsRequest   // em desenvolvimento a API usa http
                    : CookieSecurePolicy.Always;         // em produção, só HTTPS
                opcoes.ExpireTimeSpan = TimeSpan.FromHours(2);
                opcoes.SlidingExpiration = true;

                // Sem redirecionar: 401 e 403 no envelope.
                opcoes.Events.OnRedirectToLogin = contexto =>
                    Responder(contexto.Response, StatusCodes.Status401Unauthorized, ResponseCodes.NAO_AUTENTICADO);
                opcoes.Events.OnRedirectToAccessDenied = contexto =>
                    Responder(contexto.Response, StatusCodes.Status403Forbidden, ResponseCodes.ACESSO_NEGADO);
            });
        return services;
    }

    private static Task Responder(HttpResponse resposta, int status, (int Code, string DefaultMessage) erro)
    {
        resposta.StatusCode = status;
        return resposta.WriteAsJsonAsync(new RetroGamesResponseApiModel
        {
            ResponseCode = erro.Code,
            ErrorMessage = erro.DefaultMessage,
        });
    }
}
