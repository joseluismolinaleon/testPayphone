using CustomerValidationSystem.Application.Users.DTOs;
using MediatR;

namespace CustomerValidationSystem.Application.Users.Commands.Create.Commands.Create;
public record CreateUserCommand(string Name, string Job) : IRequest<UserDto>;
