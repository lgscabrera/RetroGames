using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

// Só formato (RN06): nada de consultar o banco aqui. Referências existentes (RN05) são do Service.
public class InserirJogoRequestValidator : AbstractValidator<InserirJogoRequestApiModel>
{
    public InserirJogoRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Informe o nome.")
            .MaximumLength(150).WithMessage("O nome deve ter no máximo 150 caracteres.");
        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("A descrição deve ter no máximo 1000 caracteres.");
        RuleFor(x => x.AnoLancamento)
            .InclusiveBetween(1970, 2100).WithMessage("Ano de lançamento inválido.");
        RuleFor(x => x.Preco)
            .GreaterThan(0).WithMessage("O preço deve ser maior que zero.");
        RuleFor(x => x.Estoque)
            .GreaterThanOrEqualTo(0).WithMessage("O estoque não pode ser negativo.");
        RuleFor(x => x.IdPlataforma)
            .GreaterThan(0).WithMessage("Informe a plataforma.");
        RuleFor(x => x.IdCondicaoJogo)
            .GreaterThan(0).WithMessage("Informe a condição.");
        RuleFor(x => x.IdsGeneros)
            .NotEmpty().WithMessage("Informe ao menos um gênero.");
        RuleForEach(x => x.IdsGeneros)
            .GreaterThan(0).WithMessage("Gênero inválido.");
    }
}

public class AlterarJogoRequestValidator : AbstractValidator<AlterarJogoRequestApiModel>
{
    public AlterarJogoRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Informe o nome.")
            .MaximumLength(150).WithMessage("O nome deve ter no máximo 150 caracteres.");
        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("A descrição deve ter no máximo 1000 caracteres.");
        RuleFor(x => x.AnoLancamento)
            .InclusiveBetween(1970, 2100).WithMessage("Ano de lançamento inválido.");
        RuleFor(x => x.Preco)
            .GreaterThan(0).WithMessage("O preço deve ser maior que zero.");
        RuleFor(x => x.Estoque)
            .GreaterThanOrEqualTo(0).WithMessage("O estoque não pode ser negativo.");
        RuleFor(x => x.IdPlataforma)
            .GreaterThan(0).WithMessage("Informe a plataforma.");
        RuleFor(x => x.IdCondicaoJogo)
            .GreaterThan(0).WithMessage("Informe a condição.");
        RuleFor(x => x.IdsGeneros)
            .NotEmpty().WithMessage("Informe ao menos um gênero.");
        RuleForEach(x => x.IdsGeneros)
            .GreaterThan(0).WithMessage("Gênero inválido.");
    }
}
