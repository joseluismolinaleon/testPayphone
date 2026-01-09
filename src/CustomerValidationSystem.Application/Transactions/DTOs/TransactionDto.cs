using CustomerValidationSystem.Domain.Entities;

namespace CustomerValidationSystem.Application.Transactions.DTOs;
public class TransactionDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public int Score { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Mapea una entidad Transaction a TransactionDto.
    /// </summary>
    public static TransactionDto FromEntity(Transaction transaction)
    {

        ArgumentNullException.ThrowIfNull(transaction);

        return new TransactionDto
        {
            Id = transaction.Id,
            UserId = transaction.UserId,
            Amount = transaction.Amount,
            Score = transaction.Score,
            Status = transaction.Status.ToString(),  // Enum → String
            CreatedAt = transaction.CreatedAt
        };
    }
}
