using CustomerValidationSystem.Application.Users.DTOs;
using CustomerValidationSystem.Domain.Abstractions;
using MediatR;

namespace CustomerValidationSystem.Application.Users.Queries.GetAll;
public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly IUserRepository _repository;

    public GetAllUsersQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _repository.GetAllAsync(cancellationToken);
        return users.Select(UserDto.FromEntity);
    }
}
