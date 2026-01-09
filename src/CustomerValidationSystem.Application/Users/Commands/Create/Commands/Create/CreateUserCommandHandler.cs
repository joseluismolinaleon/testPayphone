
using CustomerValidationSystem.Application.Users.DTOs;
using CustomerValidationSystem.Domain.Abstractions;
using CustomerValidationSystem.Domain.Entities;
using MediatR;

namespace CustomerValidationSystem.Application.Users.Commands.Create.Commands.Create;
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _repository;

    public CreateUserCommandHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {

        ArgumentNullException.ThrowIfNull(request);
        // Crear entidad User
        var user = User.Create(0, request.Name, request.Job);

        // Guardar en repositorio (asigna Id automáticamente)
        var savedUser = await _repository.AddAsync(user, cancellationToken);

        // Retornar DTO
        return UserDto.FromEntity(savedUser);
    }
}
