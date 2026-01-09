using CustomerValidationSystem.Application.Common.Exceptions;
using CustomerValidationSystem.Application.Transactions.DTOs;
using CustomerValidationSystem.Domain.Abstractions;
using CustomerValidationSystem.Domain.Entities;
using TransactionStatus = CustomerValidationSystem.Domain.Enums.TransactionStatus;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CustomerValidationSystem.Application.Transactions.Commands.Create;
public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IReqresApiService _reqresApiService;
    private readonly ILogger<CreateTransactionCommandHandler> _logger;

    public CreateTransactionCommandHandler(
        IUserRepository userRepository,
        ITransactionRepository transactionRepository,
        IReqresApiService reqresApiService,
        ILogger<CreateTransactionCommandHandler> logger)
    {
        _userRepository = userRepository;
        _transactionRepository = transactionRepository;
        _reqresApiService = reqresApiService;
        _logger = logger;
    }

    public async Task<TransactionDto> Handle(
        CreateTransactionCommand request,
        CancellationToken cancellationToken)
    {

        ArgumentNullException.ThrowIfNull(request);

        // ===== PASO 1: Verificar que usuario existe =====
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Transaction creation failed: User {UserId} not found", request.UserId);
            throw new KeyNotFoundException($"User with Id {request.UserId} not found");
        }

        _logger.LogInformation(
            "Creating transaction for User {UserId} ({Name}), Amount: ${Amount}",
            user.Id, user.Name, request.Amount);

        // ===== PASO 2: Obtener score de API externa =====
        int score;
        TransactionStatus status;

        try
        {
            // Llamar API externa con nombre y cédula del usuario
            score = await _reqresApiService.GetCreditScoreAsync(
                user.Name,
                user.Job,
                cancellationToken);

            _logger.LogInformation(
                "Credit score retrieved: {Score} for User {UserId}",
                score, user.Id);

            // ===== PASO 3: Aplicar reglas de negocio =====
            status = DetermineTransactionStatus(score, request.Amount);

            _logger.LogInformation(
                "Transaction status determined: {Status} (Score: {Score}, Amount: ${Amount})",
                status, score, request.Amount);
        }
        catch (HttpRequestException ex)
        {
            // ===== PASO 3b: Manejo de error de API =====
            _logger.LogError(ex,
                "Failed to get credit score from API for User {UserId}. Marking transaction as Error.",
                user.Id);

            // Por seguridad, rechazar transacción si API falla
            score = 0;
            status = TransactionStatus.Rejected;
        }

        // ===== PASO 4: Crear y guardar transacción =====
        var transaction = Transaction.Create(user.Id, request.Amount, score, status);
        await _transactionRepository.AddAsync(transaction, cancellationToken);

        _logger.LogInformation(
            "Transaction {TransactionId} created successfully with status {Status}",
            transaction.Id, status);

        // Retornar DTO
        return TransactionDto.FromEntity(transaction);
    }

    /// <summary>
    /// Determina el estado de la transacción basado en score y monto.
    /// 
    /// REGLAS DE NEGOCIO:
    /// - Score >= 700: Success (cualquier monto)
    /// - Score 500-699 Y Amount < $1,000: Success
    /// - Score 500-699 Y Amount >= $1,000: Cancel
    /// - Score < 500: Cancel (cualquier monto)
    /// </summary>
    private TransactionStatus DetermineTransactionStatus(int score, decimal amount)
    {
        // Regla 1: Score alto → Aprobación automática
        if (score >= 700)
        {
            _logger.LogDebug("Rule: High score (>= 700) → Approved");
            return TransactionStatus.Approved;
        }

        // Regla 2: Score bajo → Rechazo automático
        if (score < 500)
        {
            _logger.LogDebug("Rule: Low score (< 500) → Rejected");
            return TransactionStatus.Rejected;
        }

        // Regla 3: Score medio (500-699) → Depende del monto
        if (amount < 1000)
        {
            _logger.LogDebug("Rule: Medium score (500-699) + Low amount (< $1000) → Approved");
            return TransactionStatus.Approved;
        }
        else
        {
            _logger.LogDebug("Rule: Medium score (500-699) + High amount (>= $1000) → Rejected");
            return TransactionStatus.Rejected;
        }
    }
}
