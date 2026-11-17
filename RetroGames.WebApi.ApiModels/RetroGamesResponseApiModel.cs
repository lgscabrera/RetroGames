namespace RetroGames.WebApi.ApiModels;

// Envelope de toda resposta: { responseCode, data, errorMessage }. Sem dados, é o envelope de erro.
public class RetroGamesResponseApiModel
{
    public int ResponseCode { get; set; } = ResponseCodes.SUCESSO;
    public string? ErrorMessage { get; set; }
}

public class RetroGamesResponseApiModel<T> : RetroGamesResponseApiModel
{
    public RetroGamesResponseApiModel()
    {
    }

    public RetroGamesResponseApiModel(T data)
    {
        Data = data;
    }

    public T? Data { get; set; }
}
