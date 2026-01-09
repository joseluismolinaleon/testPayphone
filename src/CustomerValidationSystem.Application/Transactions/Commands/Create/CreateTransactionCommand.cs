using CustomerValidationSystem.Application.Transactions.DTOs;
using MediatR;

namespace CustomerValidationSystem.Application.Transactions.Commands.Create;
public record CreateTransactionCommand(int UserId, decimal Amount) : IRequest<TransactionDto>;
