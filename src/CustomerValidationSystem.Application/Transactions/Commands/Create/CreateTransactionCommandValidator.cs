

using FluentValidation;

namespace CustomerValidationSystem.Application.Transactions.Commands.Create;
public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId debe ser mayor a 0");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a 0")
            .LessThanOrEqualTo(999999.99m).WithMessage("El monto no puede exceder $999,999.99");
    }
}
