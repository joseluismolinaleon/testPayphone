using CustomerValidationSystem.Application.Transactions.DTOs;
using MediatR;

namespace CustomerValidationSystem.Application.Transactions.Queries.GetByUserId;
public record GetTransactionsByUserIdQuery(int UserId) : IRequest<IEnumerable<TransactionDto>>;
