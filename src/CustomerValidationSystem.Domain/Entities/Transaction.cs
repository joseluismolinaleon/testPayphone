using CustomerValidationSystem.Domain.Enums;

namespace CustomerValidationSystem.Domain.Entities;
public class Transaction
{
    public int Id { get; private set; }
    public int UserId { get; private set; }  
    public decimal Amount { get; private set; }
    public int Score { get; private set; }
    public TransactionStatus Status { get; private set; }  // rejected // aproved
    public DateTime CreatedAt { get; private set; }

    private Transaction() { } // EF Core

    /// <summary>
    /// Navegación al usuario asociado (relación N-1).
    /// </summary>
    public User? User { get; private set; }


    public static Transaction Create(int userId, decimal amount, int score, TransactionStatus status)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("UserId debe ser mayor a 0", nameof(userId));
        }
            

        if (amount <= 0)
        {
            throw new ArgumentException("Amount debe ser mayor a 0", nameof(amount));
        }
            

        if (score < 0)
        {
            throw new ArgumentException("Score no puede ser negativo", nameof(score));
        }
            


        return new Transaction
        {
            UserId = userId,
            Amount = amount,
            Score = score,
            Status = status,
            CreatedAt = DateTime.UtcNow
        };
    }
    public void SetId(int id)
    {
        Id = id;
    }
}
