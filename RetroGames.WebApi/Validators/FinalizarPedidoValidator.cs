using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

public class FinalizarPedidoValidator : AbstractValidator<FinalizarPedidoRequestApiModel>
{
    public FinalizarPedidoValidator()
    {
        RuleFor(pedido => pedido.Itens)
            .NotNull()
            .Must(itens => itens is { Count: > 0 })
            .WithMessage("O pedido deve conter ao menos um item.");

        RuleForEach(pedido => pedido.Itens).ChildRules(item =>
        {
            item.RuleFor(jogo => jogo.IdJogo).GreaterThan(0);
            item.RuleFor(jogo => jogo.Quantidade).InclusiveBetween(1, 10);
        });
    }
}
