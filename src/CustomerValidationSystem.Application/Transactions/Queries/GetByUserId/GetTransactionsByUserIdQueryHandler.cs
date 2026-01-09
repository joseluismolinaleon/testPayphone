using CustomerValidationSystem.Application.Transactions.DTOs;
using CustomerValidationSystem.Domain.Abstractions;
using MediatR;

namespace CustomerValidationSystem.Application.Transactions.Queries.GetByUserId;
public class GetTransactionsByUserIdQueryHandler
    : IRequestHandler<GetTransactionsByUserIdQuery, IEnumerable<TransactionDto>>
{
    private readonly ITransactionRepository _repository;

    public GetTransactionsByUserIdQueryHandler(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TransactionDto>> Handle(
        GetTransactionsByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        // Consultar relación 1-N: todas las transacciones del usuario
        var transactions = await _repository.GetByUserIdAsync(request.UserId, cancellationToken);
        return transactions.Select(TransactionDto.FromEntity);
    }
}
