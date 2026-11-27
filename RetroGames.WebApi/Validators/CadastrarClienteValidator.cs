using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

public class CadastrarClienteRequestValidator : AbstractValidator<CadastrarClienteRequestApiModel>
{
    public CadastrarClienteRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Senha).NotEmpty().MinimumLength(8).MaximumLength(100);
    }
}
