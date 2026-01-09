using CustomerValidationSystem.Application.Transactions.DTOs;
using MediatR;

namespace CustomerValidationSystem.Application.Transactions.Queries.GetById;
public record GetTransactionByIdQuery(int Id) : IRequest<TransactionDto?>;
