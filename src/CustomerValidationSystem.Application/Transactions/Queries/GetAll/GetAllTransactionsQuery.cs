using CustomerValidationSystem.Application.Transactions.DTOs;
using MediatR;

namespace CustomerValidationSystem.Application.Transactions.Queries.GetAll;
public record GetAllTransactionsQuery : IRequest<IEnumerable<TransactionDto>>;
