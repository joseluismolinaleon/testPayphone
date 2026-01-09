using CustomerValidationSystem.Application.Users.DTOs;
using MediatR;

namespace CustomerValidationSystem.Application.Users.Queries.GetById;
public record GetUserByIdQuery(int Id) : IRequest<UserDto?>;
