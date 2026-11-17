namespace RetroGames.WebApi.ApiModels;

// Códigos da própria API (0 a 99). Os erros de negócio ficam no BusinessErrors (a partir de 100).
public static class ResponseCodes
{
    public const int SUCESSO = 0;
    public const int ERRO = 1;
}
