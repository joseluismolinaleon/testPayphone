using CustomerValidationSystem.Application.Transactions.DTOs;
using CustomerValidationSystem.Domain.Abstractions;
using MediatR;

namespace CustomerValidationSystem.Application.Transactions.Queries.GetById;
public class GetTransactionByIdQueryHandler
    : IRequestHandler<GetTransactionByIdQuery, TransactionDto?>
{
    private readonly ITransactionRepository _repository;

    public GetTransactionByIdQueryHandler(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<TransactionDto?> Handle(
        GetTransactionByIdQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var transaction = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return transaction != null ? TransactionDto.FromEntity(transaction) : null;
    }
}
