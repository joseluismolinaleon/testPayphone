
using FluentValidation;

//Fluent validation

namespace CustomerValidationSystem.Application.Users.Commands.Create.Commands.Create;
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Job)
            .NotEmpty().WithMessage("La cédula es requerida")
            .Length(10).WithMessage("La cédula debe tener 10 dígitos")
            .Matches(@"^\d{10}$").WithMessage("La cédula debe contener solo números");
    }
}
