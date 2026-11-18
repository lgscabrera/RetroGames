using FluentValidation;
using RetroGames.Services;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Filters;
using RetroGames.WebApi.Middlewares;

// Carrega o .env (procura na pasta atual e nas de cima). Precisa vir antes do CreateBuilder.
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.Error.WriteLine(
        "ConnectionStrings__Default não encontrada. Crie o arquivo .env na raiz do repositório (veja o .env.example).");
    return 1;
}

builder.Services.AddRepositorios(connectionString);
builder.Services.AddServicosDeNegocio();
builder.Services.AddAutenticacaoPorCookie(builder.Environment);
builder.Services.AddAuthorization();
builder.Services.AddControllers(opcoes => opcoes.Filters.Add<FluentValidationFilter>())
    .ConfigureApiBehaviorOptions(opcoes =>
        opcoes.InvalidModelStateResponseFactory = FluentValidationFilter.RespostaModeloInvalido);
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Administrador inicial (RS05): e-mail e senha vêm do .env e nunca vão para o log.
var emailAdministrador = builder.Configuration["AdminInicial:Email"];
var senhaAdministrador = builder.Configuration["AdminInicial:Senha"];
if (!string.IsNullOrWhiteSpace(emailAdministrador) && !string.IsNullOrWhiteSpace(senhaAdministrador))
{
    using var escopo = app.Services.CreateScope();
    var autenticacaoService = escopo.ServiceProvider.GetRequiredService<IAutenticacaoService>();
    await autenticacaoService.GarantirAdministradorInicialAsync(emailAdministrador, senhaAdministrador);
}
else
{
    app.Logger.LogWarning(
        "AdminInicial__Email e AdminInicial__Senha não definidos no .env: nenhum administrador foi criado.");
}

app.UseExceptionHandler();   // primeira etapa do pipeline

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opcoes => opcoes.SwaggerEndpoint("/openapi/v1.json", "RetroGames API v1"));
}

app.UseAuthentication();   // lê o cookie: quem é você?
app.UseAuthorization();    // políticas: você pode?

app.MapControllers();
app.Run();
return 0;
