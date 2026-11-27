using Microsoft.AspNetCore.Identity;
using Moq;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public class AutenticacaoServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarios = new();
    private readonly Mock<IRepositorySet> _repositorio = new();
    private readonly Mock<IPasswordHasher<Usuario>> _hasher = new();

    public AutenticacaoServiceTests()
    {
        _repositorio.SetupGet(r => r.Usuario).Returns(_usuarios.Object);
    }

    private AutenticacaoService CriarService() => new(_repositorio.Object, _hasher.Object);

    [Fact]
    public async Task CadastrarCliente_DadosValidos_DevolveClienteComIdGerado()
    {
        _hasher.Setup(h => h.HashPassword(It.IsAny<Usuario>(), "senha-segura"))
            .Returns("hash-gerado");
        _usuarios.Setup(r => r.InserirAsync(It.IsAny<Usuario>())).ReturnsAsync(12);

        var criado = await CriarService().CadastrarClienteAsync(
            " Cliente Retro ", " CLIENTE@EXEMPLO.COM ", "senha-segura");

        Assert.Equal(12, criado.Id);
        Assert.Equal("Cliente Retro", criado.Nome);
        Assert.Equal("cliente@exemplo.com", criado.Email);
        Assert.Equal("hash-gerado", criado.SenhaHash);
        Assert.True(criado.Ativo);
        Assert.Equal(PerfilIds.Cliente, criado.IdPerfil);
    }

    [Fact]
    public async Task CadastrarCliente_EmailDuplicado_LancaUsuarioEmailDuplicado()
    {
        _hasher.Setup(h => h.HashPassword(It.IsAny<Usuario>(), "senha-segura"))
            .Returns("hash-gerado");
        _usuarios.Setup(r => r.InserirAsync(It.IsAny<Usuario>()))
            .ThrowsAsync(new RegistroDuplicadoException("Duplicate entry", new Exception()));

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => CriarService().CadastrarClienteAsync(
                "Cliente Retro", "cliente@exemplo.com", "senha-segura"));

        Assert.Equal(BusinessErrors.USUARIO_EMAIL_DUPLICADO, erro.Erro);
    }
}
