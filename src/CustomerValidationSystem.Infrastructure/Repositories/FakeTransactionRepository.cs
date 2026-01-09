
using CustomerValidationSystem.Domain.Abstractions;
using CustomerValidationSystem.Domain.Entities;

namespace CustomerValidationSystem.Infrastructure.Repositories;


public class FakeTransactionRepository : ITransactionRepository
{
    private readonly List<Transaction> _transactions;
    private readonly IUserRepository _userRepository;
    private int _nextId = 1; // Empieza en 1 (no hay seeds de transacciones)

    /// <summary>
    /// Constructor que inyecta IUserRepository para validar relaciones.
    /// </summary>
    /// <param name="userRepository">Repositorio de usuarios para verificar FK</param>
    public FakeTransactionRepository(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _transactions = new List<Transaction>();  // Lista vacía, las transacciones se crean dinámicamente
    }

    /// <summary>
    /// Agrega una nueva transacción al repositorio.
    /// Valida que el UserId exista en el repositorio de usuarios.
    /// </summary>
    public async Task<Transaction> AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        // Validar que el usuario existe (integridad referencial)
        var user = await _userRepository.GetByIdAsync(transaction.UserId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException(
                $"Cannot add transaction: User with Id {transaction.UserId} does not exist");
        }

        // Asignar Id auto-incremental
        transaction.SetId(_nextId++);

        // Agregar a la lista en memoria
        _transactions.Add(transaction);

        return await Task.FromResult(transaction);
    }

    /// <summary>
    /// Obtiene todas las transacciones del repositorio.
    /// </summary>
    public async Task<IEnumerable<Transaction>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_transactions.AsEnumerable());
    }

    /// <summary>
    /// Busca una transacción por su Id.
    /// </summary>
    /// <returns>Transacción encontrada o null si no existe</returns>
    public async Task<Transaction?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var transaction = _transactions.FirstOrDefault(t => t.Id == id);
        return await Task.FromResult(transaction);
    }

    /// <summary>
    /// Obtiene todas las transacciones de un usuario específico.
    /// IMPLEMENTA LA RELACIÓN 1-N: Un User tiene muchas Transaction.
    /// </summary>
    /// <param name="userId">Id del usuario</param>
    /// <returns>Colección de transacciones del usuario (puede estar vacía)</returns>
    public async Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var userTransactions = _transactions.Where(t => t.UserId == userId).ToList();
        return await Task.FromResult(userTransactions);
    }
}
