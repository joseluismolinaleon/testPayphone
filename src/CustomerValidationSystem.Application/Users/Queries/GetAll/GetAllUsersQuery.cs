using CustomerValidationSystem.Application.Users.DTOs;
using MediatR;

namespace CustomerValidationSystem.Application.Users.Queries.GetAll;
public record GetAllUsersQuery : IRequest<IEnumerable<UserDto>>;
