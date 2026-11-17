using System.Net;

namespace RetroGames.Services;

// Única exceção de negócio. Guarda o erro do catálogo; a API usa o Status dele (400, 404 ou 409).
public class BusinessRuleException : Exception
{
    public BusinessRuleException((int Code, string DefaultMessage, HttpStatusCode Status) erro)
        : base(erro.DefaultMessage)
    {
        Erro = erro;
    }

    public (int Code, string DefaultMessage, HttpStatusCode Status) Erro { get; }
}
