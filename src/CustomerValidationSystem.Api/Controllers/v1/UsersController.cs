using CustomerValidationSystem.Application.Users.Commands.Create.Commands.Create;
using CustomerValidationSystem.Application.Users.DTOs;
using CustomerValidationSystem.Application.Users.Queries.GetAll;
using CustomerValidationSystem.Application.Users.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CustomerValidationSystem.Api.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMediator mediator, ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los usuarios del sistema.
    /// </summary>
    /// <returns>Lista de usuarios</returns>
    /// <response code="200">Retorna la lista de usuarios (puede estar vacía)</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Getting all users");

        var query = new GetAllUsersQuery();
        var users = await _mediator.Send(query);

        _logger.LogInformation("Retrieved {Count} users", users.Count());

        return Ok(users);
    }

    /// <summary>
    /// Obtiene un usuario por su identificador.
    /// </summary>
    /// <param name="id">Id del usuario</param>
    /// <returns>Usuario encontrado</returns>
    /// <response code="200">Retorna el usuario solicitado</response>
    /// <response code="404">El usuario no existe</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("Getting user by Id: {UserId}", id);

        var query = new GetUserByIdQuery(id);
        var user = await _mediator.Send(query);

        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found", id);
            return NotFound(new { message = $"User with Id {id} not found" });
        }

        return Ok(user);
    }

    /// <summary>
    /// Crea un nuevo usuario en el sistema.
    /// </summary>
    /// <param name="command">Datos del usuario a crear</param>
    /// <returns>Usuario creado con Id asignado</returns>
    /// <response code="201">Usuario creado exitosamente</response>
    /// <response code="400">Datos de entrada inválidos</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {

        ArgumentNullException.ThrowIfNull(command);

        _logger.LogInformation("Creating new user: {Name}", command.Name);

        var user = await _mediator.Send(command);

        _logger.LogInformation("User created successfully with Id: {UserId}", user.Id);

        return CreatedAtAction(
            nameof(GetById),
            new { id = user.Id },
            user);
    }
}
