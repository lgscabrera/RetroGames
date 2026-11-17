using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Middlewares;

// Ponto único que transforma exceções em resposta HTTP, sempre no formato do envelope.
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private const string MensagemGenerica = "Ocorreu um erro inesperado. Tente novamente mais tarde.";

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        RetroGamesResponseApiModel resposta;

        if (exception is BusinessRuleException negocio)
        {
            // Regra violada: usa o status e o código do erro do catálogo (400, 404 ou 409).
            httpContext.Response.StatusCode = (int)negocio.Erro.Status;
            resposta = new RetroGamesResponseApiModel
            {
                ResponseCode = negocio.Erro.Code,
                ErrorMessage = negocio.Message,
            };
        }
        else
        {
            // Erro inesperado: o detalhe vai só para o log, nunca para quem chamou a API.
            logger.LogError(exception, "Erro inesperado em {Metodo} {Caminho}",
                httpContext.Request.Method, httpContext.Request.Path);
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            resposta = new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.ERRO,
                ErrorMessage = MensagemGenerica,
            };
        }

        await httpContext.Response.WriteAsJsonAsync(resposta, cancellationToken);
        return true;
    }
}
