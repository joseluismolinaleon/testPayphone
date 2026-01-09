using CustomerValidationSystem.Application.Transactions.DTOs;
using CustomerValidationSystem.Domain.Abstractions;
using MediatR;

namespace CustomerValidationSystem.Application.Transactions.Queries.GetAll;
public class GetAllTransactionsQueryHandler
    : IRequestHandler<GetAllTransactionsQuery, IEnumerable<TransactionDto>>
{
    private readonly ITransactionRepository _repository;

    public GetAllTransactionsQueryHandler(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TransactionDto>> Handle(
        GetAllTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        var transactions = await _repository.GetAllAsync(cancellationToken);
        return transactions.Select(TransactionDto.FromEntity);
    }
}
