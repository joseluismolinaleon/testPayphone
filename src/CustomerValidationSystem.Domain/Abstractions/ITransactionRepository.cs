
using CustomerValidationSystem.Domain.Entities;
namespace CustomerValidationSystem.Domain.Abstractions;
public interface ITransactionRepository
{
    /// <summary>
    /// Agrega una nueva transacción al repositorio.
    /// </summary>
    /// <param name="transaction">Transacción a agregar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Transacción agregada con Id asignado</returns>
    public Task<Transaction> AddAsync(Transaction transaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las transacciones del sistema.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Colección de transacciones</returns>
    public Task<IEnumerable<Transaction>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una transacción por su identificador único.
    /// </summary>
    /// <param name="id">Id de la transacción</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Transacción encontrada o null si no existe</returns>
    public Task<Transaction?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las transacciones de un usuario específico.
    /// Este método implementa la relación 1-N entre User y Transaction.
    /// </summary>
    /// <param name="userId">Id del usuario</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Colección de transacciones del usuario</returns>
    public Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}
