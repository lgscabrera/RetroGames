using System.Net;

namespace RetroGames.Services;

// Catálogo dos erros de negócio: (Code, DefaultMessage, Status). Códigos a partir de 100.
public static class BusinessErrors
{
    public static readonly (int Code, string DefaultMessage, HttpStatusCode Status) PLATAFORMA_NAO_ENCONTRADA =
        (100, "Plataforma não encontrada.", HttpStatusCode.NotFound);

    public static readonly (int Code, string DefaultMessage, HttpStatusCode Status) PLATAFORMA_NOME_DUPLICADO =
        (101, "Já existe uma plataforma com este nome.", HttpStatusCode.Conflict);

    public static readonly (int Code, string DefaultMessage, HttpStatusCode Status) PLATAFORMA_POSSUI_JOGOS =
        (102, "Não é possível excluir: existem jogos desta plataforma.", HttpStatusCode.Conflict);

    public static readonly (int Code, string DefaultMessage, HttpStatusCode Status) JOGO_NAO_ENCONTRADO =
        (103, "Jogo não encontrado.", HttpStatusCode.NotFound);

    public static readonly (int Code, string DefaultMessage, HttpStatusCode Status) CONDICAO_NAO_ENCONTRADA =
        (104, "Condição de jogo não encontrada.", HttpStatusCode.NotFound);

    public static readonly (int Code, string DefaultMessage, HttpStatusCode Status) GENERO_NAO_ENCONTRADO =
        (105, "Gênero não encontrado.", HttpStatusCode.NotFound);

    public static readonly (int Code, string DefaultMessage, HttpStatusCode Status) USUARIO_EMAIL_DUPLICADO =
        (106, "Já existe um usuário cadastrado com este e-mail.", HttpStatusCode.Conflict);

    public static readonly (int Code, string DefaultMessage, HttpStatusCode Status) JOGO_INDISPONIVEL =
        (110, "Jogo inexistente ou inativo.", HttpStatusCode.BadRequest);

    public static readonly (int Code, string DefaultMessage, HttpStatusCode Status) PEDIDO_ESTOQUE_INSUFICIENTE =
        (111, "Estoque insuficiente para concluir o pedido.", HttpStatusCode.Conflict);

    public static readonly (int Code, string DefaultMessage, HttpStatusCode Status) USUARIO_NAO_AUTENTICADO =
        (112, "Faça login para continuar.", HttpStatusCode.Unauthorized);

    public static readonly (int Code, string DefaultMessage, HttpStatusCode Status) PEDIDO_SEM_ITENS =
        (113, "O pedido deve conter ao menos um item.", HttpStatusCode.BadRequest);

    public static readonly (int Code, string DefaultMessage, HttpStatusCode Status) PEDIDO_QUANTIDADE_INVALIDA =
        (114, "A quantidade deve estar entre 1 e 10.", HttpStatusCode.BadRequest);
}
