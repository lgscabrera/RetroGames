using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Filters;

// Roda antes da action: confere o formato dos dados de entrada com o validator do tipo.
public class FluentValidationFilter(IServiceProvider servicos) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var erros = new Dictionary<string, List<string>>();

        foreach (var argumento in context.ActionArguments.Values)
        {
            if (argumento is null)   // corpo ausente
            {
                context.Result = new BadRequestObjectResult(new RetroGamesResponseApiModel
                {
                    ResponseCode = ResponseCodes.MODEL_NULL.Code,
                    ErrorMessage = ResponseCodes.MODEL_NULL.DefaultMessage,
                });
                return;
            }

            var tipoDoValidator = typeof(IValidator<>).MakeGenericType(argumento.GetType());
            if (servicos.GetService(tipoDoValidator) is not IValidator validator)
                continue;   // este argumento (por exemplo, o id da rota) não tem validator

            var resultado = await validator.ValidateAsync(
                new ValidationContext<object>(argumento), context.HttpContext.RequestAborted);

            foreach (var falha in resultado.Errors)
            {
                var campo = Minuscula(falha.PropertyName);
                if (!erros.TryGetValue(campo, out var mensagens))
                    erros[campo] = mensagens = new List<string>();
                mensagens.Add(falha.ErrorMessage);
            }
        }

        if (erros.Count > 0)
        {
            context.Result = new BadRequestObjectResult(RespostaDeErros(
                erros.ToDictionary(e => e.Key, e => e.Value.ToArray())));
            return;
        }

        await next();
    }

    // Usada também quando o próprio ASP.NET recusa o corpo (JSON malformado ou tipo errado).
    // Não repassa a mensagem do framework, para não expor detalhes internos.
    public static IActionResult RespostaModeloInvalido(ActionContext contexto)
    {
        var erros = new Dictionary<string, string[]>();
        foreach (var (chave, estado) in contexto.ModelState)
        {
            if (estado.Errors.Count == 0)
                continue;
            var campo = Minuscula(chave.TrimStart('$', '.'));
            erros[string.IsNullOrEmpty(campo) ? "corpo" : campo] = new[] { "Valor inválido." };
        }
        return new BadRequestObjectResult(RespostaDeErros(erros));
    }

    private static RetroGamesResponseApiModel<Dictionary<string, string[]>> RespostaDeErros(
        Dictionary<string, string[]> erros) => new()
    {
        ResponseCode = ResponseCodes.MODEL_STATE_INVALIDO.Code,
        ErrorMessage = ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage,
        Data = erros,
    };

    private static string Minuscula(string nome)
        => nome.Length == 0 ? nome : char.ToLowerInvariant(nome[0]) + nome[1..];
}
