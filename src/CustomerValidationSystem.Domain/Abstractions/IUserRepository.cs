
using CustomerValidationSystem.Domain.Entities;

namespace CustomerValidationSystem.Domain.Abstractions;
public interface IUserRepository
{

    /// <summary>
    /// Obtiene todos los usuarios del sistema.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Colección de usuarios</returns>
    public Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un usuario por su identificador único.
    /// </summary>
    /// <param name="id">Id del usuario</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Usuario encontrado o null si no existe</returns>
    public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega un nuevo usuario al repositorio.
    /// </summary>
    /// <param name="user">Usuario a agregar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Usuario agregado con Id asignado</returns>
    public Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
}

