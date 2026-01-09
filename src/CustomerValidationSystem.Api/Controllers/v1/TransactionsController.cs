using Asp.Versioning;
using CustomerValidationSystem.Application.Common.Exceptions;
using CustomerValidationSystem.Application.Transactions.Commands.Create;
using CustomerValidationSystem.Application.Transactions.DTOs;
using CustomerValidationSystem.Application.Transactions.Queries.GetAll;
using CustomerValidationSystem.Application.Transactions.Queries.GetById;
using CustomerValidationSystem.Application.Transactions.Queries.GetByUserId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CustomerValidationSystem.API.Controllers.v1;

/// <summary>
/// Controlador para gestionar transacciones con validación de scoring.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(IMediator mediator, ILogger<TransactionsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Crea una nueva transacción con validación de scoring crediticio.
    /// ENDPOINT PRINCIPAL: Llama API externa para obtener score y aplica reglas de negocio.
    /// </summary>
    /// <param name="command">Datos de la transacción (userId, amount)</param>
    /// <returns>Transacción creada con status (Success/Cancel/Error)</returns>
    /// <response code="201">Transacción creada exitosamente</response>
    /// <response code="400">Datos de entrada inválidos</response>
    /// <response code="404">Usuario no encontrado</response>
    [HttpPost]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateTransactionCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        _logger.LogInformation(
            "Creating transaction for User {UserId}, Amount: ${Amount}",
            command.UserId, command.Amount);

        try
        {
            var transaction = await _mediator.Send(command);

            _logger.LogInformation(
                "Transaction {TransactionId} created with status {Status}",
                transaction.Id, transaction.Status);

            return CreatedAtAction(
                nameof(GetById),
                new { id = transaction.Id },
                transaction);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "User {UserId} not found", command.UserId);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene todas las transacciones del sistema.
    /// </summary>
    /// <returns>Lista de transacciones</returns>
    /// <response code="200">Retorna todas las transacciones</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Getting all transactions");

        var query = new GetAllTransactionsQuery();
        var transactions = await _mediator.Send(query);

        _logger.LogInformation("Retrieved {Count} transactions", transactions.Count());

        return Ok(transactions);
    }

    /// <summary>
    /// Obtiene una transacción por su identificador.
    /// </summary>
    /// <param name="id">Id de la transacción</param>
    /// <returns>Transacción encontrada</returns>
    /// <response code="200">Retorna la transacción solicitada</response>
    /// <response code="404">La transacción no existe</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("Getting transaction by Id: {TransactionId}", id);

        var query = new GetTransactionByIdQuery(id);
        var transaction = await _mediator.Send(query);

        if (transaction == null)
        {
            _logger.LogWarning("Transaction {TransactionId} not found", id);
            return NotFound(new { message = $"Transaction with Id {id} not found" });
        }

        return Ok(transaction);
    }

    /// <summary>
    /// Obtiene todas las transacciones de un usuario específico (historial).
    /// ENDPOINT DE RELACIÓN 1-N: Muestra todas las transacciones del usuario.
    /// </summary>
    /// <param name="userId">Id del usuario</param>
    /// <returns>Lista de transacciones del usuario</returns>
    /// <response code="200">Retorna las transacciones del usuario (puede estar vacía)</response>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<TransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        _logger.LogInformation("Getting transactions for User {UserId}", userId);

        var query = new GetTransactionsByUserIdQuery(userId);
        var transactions = await _mediator.Send(query);

        _logger.LogInformation(
            "Retrieved {Count} transactions for User {UserId}",
            transactions.Count(), userId);

        return Ok(transactions);
    }
}
