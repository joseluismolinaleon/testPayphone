using CustomerValidationSystem.Application.Users.DTOs;
using CustomerValidationSystem.Domain.Abstractions;
using MediatR;

namespace CustomerValidationSystem.Application.Users.Queries.GetById;
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _repository;

    public GetUserByIdQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)

    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return user != null ? UserDto.FromEntity(user) : null;
    }
}
