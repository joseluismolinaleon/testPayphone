

namespace CustomerValidationSystem.Domain.Entities;
public class User 
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Job { get; private set; }  // Cédula
    public DateTime CreatedAt { get; private set; }

    public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();

    private User() { } // EF Core

    public static User Create(int id, string name, string job)
    {

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre no puede estar vacío", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(job))
        {
            throw new ArgumentException("La cédula no puede estar vacía", nameof(job));
        }

        return new User
        {
            Id = id,
            Name = name,
            Job = job,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void SetId(int id)
    {
        Id = id;
    }

}
